using FileServer.FileProvider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileServer.Minio
{
    public class MinioBlobProvider : FileProviderHandler<MinioBlobOptions>
    {
        protected IMinioBlobNameCalculator MinioBlobNameCalculator { get; }

        public MinioBlobProvider(IMinioBlobNameCalculator minioBlobNameCalculator, IOptionsMonitor<MinioBlobOptions> options,
            ILoggerFactory logger)
            :base(options, logger)
        {
            MinioBlobNameCalculator = minioBlobNameCalculator;
        }

        public override async Task SaveAsync(BlobProviderSaveArgs args)
        {
            var blobName = MinioBlobNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = Options.BucketName;
            
            if (!args.OverrideExisting && await BlobExistsAsync(client, containerName, blobName))
            {
                throw new Exception($"Saving BLOB '{args.BlobName}' does already exists in the container '{containerName}'! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }

            if (Options.CreateBucketIfNotExists)
            {
                await CreateBucketIfNotExists(client, containerName);
            }

            await client.PutObjectAsync(containerName, blobName, args.BlobStream, args.BlobStream.Length);
        }

        public override async Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
        {
            var blobName = MinioBlobNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = Options.BucketName;

            if (await BlobExistsAsync(client, containerName, blobName))
            {
                await client.RemoveObjectAsync(containerName, blobName);
                return true;
            }

            return false;
        }

        public override async Task<bool> ExistsAsync(BlobProviderExistsArgs args)
        {
            var blobName = MinioBlobNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = Options.BucketName;

            return await BlobExistsAsync(client, containerName, blobName);
        }

        public override async Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
        {
            var blobName = MinioBlobNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = Options.BucketName;

            if (!await BlobExistsAsync(client, containerName, blobName))
            {
                return null;
            }

            var memoryStream = new MemoryStream();
            await client.GetObjectAsync(containerName, blobName, (stream) =>
            {
                if (stream != null)
                {
                    stream.CopyTo(memoryStream);
                }
                else
                {
                    memoryStream = null;
                }
            });

            return memoryStream;
        }

        public virtual MinioClient GetMinioClient(BlobProviderArgs args)
        {
            var client = new MinioClient(Options.EndPoint, Options.AccessKey, Options.SecretKey);

            if (Options.WithSSL)
            {
                client.WithSSL();
            }

            return client;
        }

        public virtual async Task CreateBucketIfNotExists(MinioClient client, string containerName)
        {
            if (!await client.BucketExistsAsync(containerName))
            {
                await client.MakeBucketAsync(containerName);
            }
        }

        public virtual async Task<bool> BlobExistsAsync(MinioClient client, string containerName, string blobName)
        {
            // Make sure Blob Container exists.
            if (await client.BucketExistsAsync(containerName))
            {
                try
                {
                    await client.StatObjectAsync(containerName, blobName);
                }
                catch (Exception e)
                {
                    if (e is ObjectNotFoundException)
                    {
                        return false;
                    }

                    throw;
                }

                return true;
            }

            return false;
        }

    }
}

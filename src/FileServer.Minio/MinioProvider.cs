using FileServer.Common.Extensions;
using FileServer.FileProvider;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileServer.Minio
{
    public class MinioProvider : FileProviderHandler
    {
        protected IMinioNameCalculator MinioBlobNameCalculator { get; }
        private readonly MinioOptions Options;

        public MinioProvider(IMinioNameCalculator minioBlobNameCalculator, IOptions<MinioOptions> options)
        {
            MinioBlobNameCalculator = minioBlobNameCalculator;
            Options = options?.Value;
        }

        public override async Task SaveAsync(BlobProviderSaveArgs args)
        {
            var blobName = MinioBlobNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = GetContainerName();
            
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
            var containerName = GetContainerName();

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
            var containerName = GetContainerName();

            return await BlobExistsAsync(client, containerName, blobName);
        }

        public override async Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
        {
            var blobName = MinioBlobNameCalculator.Calculate(args);
            var client = GetMinioClient(args);
            var containerName = GetContainerName();

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

            //必须将流的当前位置置0，否则将引发异常
            //如果不设置为0，则流的当前位置在流的末端1629，然后读流就会从索引1629开始读取，实际上流的最大索引是1628，就会引发无效操作异常System.InvalidOperationException
            //System.InvalidOperationException: Response Content-Length mismatch: too few bytes written (0 of 1628)
            memoryStream.Seek(0, SeekOrigin.Begin);
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

        public override Task<Stream> GetAsync(BlobProviderGetArgs args)
        {
            throw new NotImplementedException();
        }

        protected virtual string GetContainerName()
        {
            return Options.BucketName.IsNullOrWhiteSpace()
                ? MinioDefaults.BucketName : Options.BucketName;
        }
    }
}

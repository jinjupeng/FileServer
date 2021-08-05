using FileServer.Common.Helper;
using FileServer.FileProvider;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileServer.FileSystem
{
    public class FileSystemProvider : FileProviderHandler
    {
        protected IFilePathCalculator FilePathCalculator { get; }
        private readonly FileSystemOptions fileSystemOptions;

        public FileSystemProvider(IFilePathCalculator filePathCalculator, IOptions<FileSystemOptions> options)
        {
            FilePathCalculator = filePathCalculator;
            fileSystemOptions = options?.Value;
        }

        public override Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            return Task.FromResult(FileHelper.DeleteIfExists(filePath));
        }

        public override Task<bool> ExistsAsync(BlobProviderExistsArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);
            return ExistsAsync(filePath);
        }

        public override async Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);

            if (!File.Exists(filePath))
            {
                return null;
            }
            var memoryStream = new MemoryStream();
            using (var fileStream = File.OpenRead(filePath))
            {
                await fileStream.CopyToAsync(memoryStream, args.CancellationToken);
            }
            
            //必须将流的当前位置置0，否则将引发异常
            //如果不设置为0，则流的当前位置在流的末端1629，然后读流就会从索引1629开始读取，实际上流的最大索引是1628，就会引发无效操作异常System.InvalidOperationException
            //System.InvalidOperationException: Response Content-Length mismatch: too few bytes written (0 of 1628)
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        public override async Task SaveAsync(BlobProviderSaveArgs args)
        {
            var filePath = FilePathCalculator.Calculate(args);

            if (!args.OverrideExisting && await ExistsAsync(filePath))
            {
                throw new Exception($"Saving BLOB '{args.BlobName}' does already exists! Set {nameof(args.OverrideExisting)} if it should be overwritten.");
            }

            DirectoryHelper.CreateIfNotExists(Path.GetDirectoryName(filePath));

            var fileMode = args.OverrideExisting
                ? FileMode.Create
                : FileMode.CreateNew;

            using (var fileStream = File.Open(filePath, fileMode, FileAccess.Write))
            {
                await args.BlobStream.CopyToAsync(fileStream, args.CancellationToken);
                await fileStream.FlushAsync();
            }
        }

        protected virtual Task<bool> ExistsAsync(string filePath)
        {
            return Task.FromResult(File.Exists(filePath));
        }

        public override Task<Stream> GetAsync(BlobProviderGetArgs args)
        {
            throw new NotImplementedException();
        }
    }
}

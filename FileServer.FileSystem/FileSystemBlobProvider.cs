using FileServer.Common.Helper;
using FileServer.FileProvider;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileServer.FileSystem
{
    public class FileSystemBlobProvider : BlobProviderBase
    {
        protected IBlobFilePathCalculator FilePathCalculator { get; }

        public FileSystemBlobProvider(IBlobFilePathCalculator filePathCalculator)
        {
            FilePathCalculator = filePathCalculator;
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
            using (var fileStream = File.OpenRead(filePath))
            {
                var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream, args.CancellationToken);
                return memoryStream;
            }

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
                await args.BlobStream.CopyToAsync(
                    fileStream,
                    args.CancellationToken
                );

                await fileStream.FlushAsync();
            }
        }

        protected virtual Task<bool> ExistsAsync(string filePath)
        {
            return Task.FromResult(File.Exists(filePath));
        }
    }
}

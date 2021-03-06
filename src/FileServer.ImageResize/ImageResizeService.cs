using FileServer.ImageResize.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;

namespace FileServer.ImageResize
{
    public class ImageResizeService : IImageResizeService
    {
        private readonly IOptionsMonitor<ResizingOptions> _optionsMonitor;
        private readonly ILogger<ImageResizeService> _log;

        public ImageResizeService(IOptionsMonitor<ResizingOptions> optionsMonitor, ILogger<ImageResizeService> log)
        {
            _optionsMonitor = optionsMonitor;
            _log = log;
        }

        public virtual void Resize(Stream inputStream, Stream outputStream, ResizeInstructions instructions)
        {
            if (!inputStream.CanRead)
                throw new ArgumentException("inputStream is nor readable");
            if (!outputStream.CanWrite)
                throw new ArgumentException("outputStream is nor writable");
            var options = _optionsMonitor.CurrentValue;
            instructions.Join(options.DefaultInstructions);
            instructions.Join(options.MandatoryInstructions, true);

            using (var image = Image.Load(inputStream, out var imageInfo))
            {
                var targetSize = GetTargetSize(instructions, image.Width / (double)image.Height);
                _log.LogDebug("Resize {image_format_name}, original size {width}x{height}:{size} bytes, target size {width}x{height}", imageInfo.Name, image.Width, image.Height, inputStream.Length, targetSize.Width, targetSize.Height);
                image.Mutate(x => x.Resize(targetSize.Width, targetSize.Height));
                image.Save(outputStream, imageInfo); // Automatic encoder selected based on extension.
                _log.LogDebug("Resizing complete, output image size: {width}x{height}:{size} bytes", targetSize.Width, targetSize.Height, outputStream.Length);
            }
        }

        public (int Width, int Height, bool isResizable) Probe(Stream stream)
        {
            var info = Image.Identify(stream);
            return (info.Width, info.Height, true); //For now we consider everything as resizable
        }

        internal static (int Width, int Height) GetTargetSize(ResizeInstructions instructions, double imageRatio)
        {
            var targetWidth = Math.Min(instructions.Width ?? -1, instructions.MaxWidth);
            var targetHeight = Math.Min(instructions.Height ?? -1, instructions.MaxHeight);

            if (targetWidth == -1 && targetHeight == -1)
                throw new ArgumentException("Nigher width or height are defined");

            if (targetHeight == -1 || targetWidth == -1 || (instructions.KeepAspectRatio.HasValue && instructions.KeepAspectRatio.Value))
            {
                if (targetHeight != -1 && targetWidth != -1)
                {
                    if (imageRatio > 1)
                        targetHeight = (int)Math.Round(targetWidth / imageRatio);
                    else
                        targetWidth = (int)Math.Round(targetHeight * imageRatio);
                }
                else if (targetHeight != -1)
                {
                    targetWidth = (int)Math.Round(targetHeight * imageRatio);
                }
                else if (targetWidth != -1)
                {
                    targetHeight = (int)Math.Round(targetWidth / imageRatio);
                }
                else
                {
                    //That's generally impossible because of the same check above, but let's check it twice 
                    throw new ArgumentException("Nigher width or height are defined");
                }
            }
            return (targetWidth, targetHeight);
        }
    }
}

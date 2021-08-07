using FileServer.ImageResize.Utils;
using System.IO;

namespace FileServer.ImageResize
{
    public interface IImageResizeService
    {
        /// <summary>
        /// Apply instructions to an image.
        /// A good point to extra configuration of Image Resizer
        /// </summary>
        /// <param name="inputStream">Input image stream</param>
        /// <param name="outputStream">Output stream to write the result</param>
        /// <param name="instructions">Instructions to apply</param>
        /// <returns>Result image as a stream. Caller have to care about the stream disposing.</returns>
        void Resize(Stream inputStream, Stream outputStream, ResizeInstructions instructions);

        (int Width, int Height, bool isResizable) Probe(Stream stream);
    }
}

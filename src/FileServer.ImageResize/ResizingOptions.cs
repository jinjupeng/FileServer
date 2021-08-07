using FileServer.ImageResize.Utils;

namespace FileServer.ImageResize
{
    public class ResizingOptions
    {
        public ResizeInstructions DefaultInstructions { get; set; }
        public ResizeInstructions MandatoryInstructions { get; set; }
    }
}

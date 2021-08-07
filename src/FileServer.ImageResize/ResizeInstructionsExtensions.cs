using FileServer.ImageResize.Utils;

namespace FileServer.ImageResize
{
    public static class ResizeInstructionsExtensions
    {
        public static string BuildSizeKey(this ResizeInstructions instructions)
        {
            return "alt_size_"
                   + (instructions.Width?.ToString() ?? "-")
                   + 'x'
                   + (instructions.Height?.ToString() ?? "-");
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;

namespace Test
{
    public static class HttpRequestFactory
    {
        public static HttpRequest Create(string method, string contentType, IFormCollection formCollection)
        {
            var context = new DefaultHttpContext();
            var request = context.Request;
            var boundary = $"----------------------------{DateTime.Now.Ticks.ToString("x")}";

            request.Method = method;
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Content-Type", contentType);
            request.ContentType = $"{contentType}; boundary={boundary}";
            request.Form = formCollection;


            return request;
        }

        public static FormFileCollection GetFormFiles(string fileNames)
        {
            var formFileCollection = new FormFileCollection();
            foreach (var file in fileNames.Split(','))
            {
                formFileCollection.Add(GetFormFile(file));
            }
            return formFileCollection;
        }

        private static IFormFile GetFormFile(string fileName)
        {
            string fileExtension = fileName.Substring(fileName.IndexOf('.') + 1);
            string fileNameandPath = GetFilePathWithName(fileName);
            IFormFile formFile;
            var stream = File.OpenRead(fileNameandPath);

            switch (fileExtension)
            {
                case "jpg":
                    formFile = new FormFile(stream, 0, stream.Length,
                        fileName.Substring(0, fileName.IndexOf('.')),
                        fileName)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/jpeg"
                    };
                    break;

                case "png":
                    formFile = new FormFile(stream, 0, stream.Length,
                        fileName.Substring(0, fileName.IndexOf('.')),
                        fileName)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "image/png"
                    };
                    break;

                case "pdf":
                    formFile = new FormFile(stream, 0, stream.Length,
                        fileName.Substring(0, fileName.IndexOf('.')),
                        fileName)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = "application/pdf"
                    };
                    break;

                default:
                    formFile = null;
                    break;
            }

            return formFile;
        }

        private static string GetFilePathWithName(string filename)
        {
            var outputFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return $"{outputFolder.Substring(0, outputFolder.IndexOf("bin"))}testfiles\\{filename}";
        }

        private static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary);
            if (string.IsNullOrWhiteSpace(boundary.Value))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if (boundary.Length > lengthLimit)
            {
                throw new InvalidDataException(
                    $"Multipart boundary length limit {lengthLimit} exceeded.");
            }

            return boundary.Value;
        }

    }
}

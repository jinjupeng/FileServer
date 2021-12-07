using Autofac.Extras.Moq;
using FileServer.Hosted;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Test.Hosted
{
    public class UploadControllerTest
    {
        /// <summary>
        /// https://stackoverflow.com/questions/67517162/testing-a-multipart-file-upload-azure-function
        /// https://darchuk.net/2019/03/29/asp-net-core-unit-testing-a-file-upload/
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task StreamUploadTest()
        {
            //var fileNames = "testfile2.jpg";
            //var formFiles = HttpRequestFactory.GetFormFiles(fileNames);
            //var formCollection = new FormCollection(null, formFiles);

            // Get a loose automock
            var Mock = AutoMock.GetLoose();
            string expectedFileContents = "This is the expected file contents!";
            // Set up the form file with a stream containing the expected file contents
            Mock<IFormFile> formFiles = new Mock<IFormFile>();
            formFiles.Setup(ff => ff.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
              .Returns<Stream, CancellationToken>((s, ct) =>
              {
                  byte[] buffer = Encoding.Default.GetBytes(expectedFileContents);
                  s.Write(buffer, 0, buffer.Length);
                  return Task.CompletedTask;
              });

            // Set up the form collection with the mocked form
            Mock<IFormCollection> formCollection = new Mock<IFormCollection>();
            formCollection.Setup(f => f.Files[It.IsAny<int>()]).Returns(formFiles.Object);

            var request = HttpRequestFactory.Create("POST", "multipart/form-data", formCollection.Object);
            request.EnableBuffering();
            var response = await UploadHelper.StreamUpload(request);

            Assert.True(!string.IsNullOrEmpty(response));
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestTask1.Controllers;
using TestTask1.Data;
using TestTask1.Models;
using Xunit;

namespace TestProject1.Controllers
{
    public class PlatformsControllerControllerTest
    {
        private readonly Platform platform;
        private readonly PlatformsController controller;
        private string validParseData = $"Яндекс.Маркетинг:/ru{Environment.NewLine}Вкусно и точка:/ru/vkustoch,/ru/nevktch";
        private string validLocation = $"/ru/vkustoch";
        private string validNotFoundLocation = $"/notFound";

        public PlatformsControllerControllerTest()
        {
            platform = new Platform("/test");
            controller = new PlatformsController(platform);
        }


        [Fact]
        public async Task Get_ValidLocation_ReturnOk()
        {
            await Upload_ValidFile_ReturnOk();

            var result = controller.Get(validLocation);

            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("/")]
        [InlineData(" ")]
        [InlineData("/Invalid path/invalid//")]
        public async Task Get_InvalidLocation_ReturnBadRequest(string input)
        {
            await Upload_ValidFile_ReturnOk();

            var result = controller.Get(input);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Get_ValidLocation_ReturnNotFound()
        {
            await Upload_ValidFile_ReturnOk();

            var result = controller.Get(validNotFoundLocation);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Upload_ValidFile_ReturnOk()
        {
            var mockFile = CreateFile(validParseData);

            var fileModel = new UploadFileModel { File = mockFile.Object };

            var result = await controller.Upload(fileModel);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Upload_WithInvalidFile_ReturnBadRequest()
        {
            var fileModel = new UploadFileModel { File = null };

            var result = await controller.Upload(fileModel);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("Невозможный текст для разбора")]
        public async Task Upload_WithInvalidContent_ReturnsBadRequest(string? content)
        {
            var fileModel = new UploadFileModel();

            if (content != null)
            {
                var mockFile = CreateFile(content);

                fileModel.File = mockFile.Object;
            }

            var result = await controller.Upload(fileModel);
            
            Assert.IsType<BadRequestObjectResult>(result);
        }

        private Mock<IFormFile> CreateFile(string? content)
        {
            var mockFile = new Mock<IFormFile>();

            if (content != null)
            {
                var ms = new MemoryStream();
                var writer = new StreamWriter(ms);
                writer.Write(content);
                writer.Flush();
                ms.Position = 0;

                mockFile.Setup(f => f.OpenReadStream()).Returns(ms);
                mockFile.Setup(f => f.Length).Returns(ms.Length);
            }

            mockFile.Setup(f => f.FileName).Returns("test.txt");

            return mockFile;
        }
    }
}

using TestTask1.Data;
using Xunit;

namespace TestProject1.Data
{
    public class PlatformTest
    {
        private const string validPrefix = "/valid";
        private const string invalidPrefix = "/invalid/";
        private string validParseData = $"Яндекс.Маркетинг:/ru{Environment.NewLine}Вкусно и точка:/ru/vkustoch,/ru/nevktch";
        private const string validLocation = "/ru/vkustoch";
        private readonly Platform platform;

        public PlatformTest()
        {
            platform = new Platform("/test");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Невозможный текст для разбора")]
        public void Parse_InvalidData_Errors(string data)
        {
            var exception = Assert.ThrowsAny<Exception>(() =>
                platform.Parse(data));

            Assert.NotNull(exception);
        }


        [Fact]
        public void Parse_ValidData_NotErrors()
        {
            platform.Parse(validParseData);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("Неверный префикс")]
        [InlineData("\\prefix")]
        [InlineData("/prefix/")]
        public void Create_InvalidPrefix_Errors(string? prefix)
        {
            var exception = Assert.ThrowsAny<Exception>(() =>
                new Platform(prefix!));

            Assert.NotNull(exception);
        }


        [Fact]
        public void Create_ValidPrefix_NotErrors()
        {
            new Platform(validPrefix);
        }


        [Fact]
        public void Add_NotErrors()
        {
            var child = new Platform(validPrefix);

            int count = platform.Platforms.Count;

            platform.Add(child);

            Assert.True(count == platform.Platforms.Count - 1);
        }

        [Fact]
        public void Add_DuplicatePrefix_Errors()
        {
            platform.Add(new Platform(validPrefix));

            var exception = Assert.ThrowsAny<Exception>(() =>
                platform.Add(new Platform(validPrefix)));

            Assert.NotNull(exception);
        }

        [Fact]
        public void Add_Null_Errors()
        {
            var exception = Assert.ThrowsAny<Exception>(() =>
                platform.Add(null!));

            Assert.NotNull(exception);
        }


        [Fact]
        public void Get_InvalidPrefix_Errors()
        {
            var exception = Assert.ThrowsAny<Exception>(() => platform[invalidPrefix]);

            Assert.NotNull(exception);
        }

        [Fact]
        public void Get_NotErrors()
        {
            platform.Add(new Platform(validPrefix));

            var child = platform[validPrefix];

            Assert.NotNull(child);
        }

        [Fact]
        public void GetOrAdd_InvalidPrefix_Errors()
        {
            var exception = Assert.ThrowsAny<Exception>(() => platform.GetOrAdd(invalidPrefix));

            Assert.NotNull(exception);
        }


        [Fact]
        public void GetOrAdd_NotErrors()
        {
            Platform child = platform.GetOrAdd(validPrefix);

            Assert.Contains(child, platform.Platforms);
        }


        [Fact]
        public void Remove_NotErrors()
        {
            var child = new Platform(validPrefix);

            platform.Add(child);
        }

        [Fact]
        public void Remove_NotExist_Errors()
        {
            var exception = Assert.ThrowsAny<Exception>(() =>
                platform.Remove(validPrefix));

            Assert.NotNull(exception);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("/prefix/")]
        public void Remove_NotValidPrefix_Errors(string? input)
        {
            var exception = Assert.ThrowsAny<Exception>(() =>
                platform.Remove(input));

            Assert.NotNull(exception);
        }

        [Fact]
        public void Clear_NotErrors()
        {
            platform.GetOrAdd(validPrefix);

            int count = platform.Platforms.Count;

            platform.Clear();

            int actualCount = platform.Platforms.Count;

            Assert.True(count != actualCount && actualCount == 0);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("/prefix/adasd/")]
        public void FindPlatforms_NotValidLocation_Errors(string input)
        {
            platform.Parse(validParseData);

            var exception = Assert.ThrowsAny<Exception>(() =>
                platform.FindPlatforms(input));

            Assert.NotNull(exception);
        }

        [Fact]
        public void FindPlatforms_NotErrors()
        {
            platform.Parse(validParseData);

            var result = platform.FindPlatforms(validLocation);

            Assert.True(result.Count() > 0);
        }

    }
}

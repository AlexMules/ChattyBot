using ChattyBot.Server.Application.Services;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;
using System.Text;

namespace ChattyBot.Tests.Services
{
    public class ExportServiceTests
    {
        private readonly ExportService _sut;

        public ExportServiceTests()
        {
            _sut = new ExportService();
        }

        private ExportConversationDTO GetSampleData() => new ExportConversationDTO(
            "Test Chat",
            DateTime.UtcNow,
            new List<ExportMessageDTO>
            {
                new ExportMessageDTO("User", "Hello", DateTime.UtcNow),
                new ExportMessageDTO("Bot", "Hi!", DateTime.UtcNow)
            });

        [Fact]
        public void CreateExportFile_ShouldReturnJson_WhenFormatIsJson()
        {
            var data = GetSampleData();

            var result = _sut.CreateExportFile(data, "json");

            result.ContentType.Should().Be("application/json");
            result.FileName.Should().EndWith(".json");
            result.FileName.Should().StartWith("ChattyBot_Export_");

            var jsonString = Encoding.UTF8.GetString(result.Content);
            jsonString.Trim().Should().StartWith("{").And.EndWith("}");
            jsonString.Should().Contain("Test Chat");
        }

        [Fact]
        public void CreateExportFile_ShouldReturnXml_WhenFormatIsXml()
        {
            var data = GetSampleData();

            var result = _sut.CreateExportFile(data, "xml");

            result.ContentType.Should().Be("application/xml");
            result.FileName.Should().EndWith(".xml");

            var xmlString = Encoding.UTF8.GetString(result.Content);
            xmlString.Trim().Should().StartWith("<?xml").And.Contain("<ExportConversationDTO");
            xmlString.Should().Contain("Test Chat");
        }

        [Fact]
        public void CreateExportFile_ShouldDefaultToJson_WhenFormatIsUnknown()
        {
            var data = GetSampleData();

            var result = _sut.CreateExportFile(data, "random_format");

            result.ContentType.Should().Be("application/json");
            result.FileName.Should().EndWith(".json");
        }

        [Fact]
        public void CreateExportFile_ShouldGenerateCorrectTimestampInFileName()
        {
            var data = GetSampleData();
            var today = DateTime.Now.ToString("yyyyMMdd");

            var result = _sut.CreateExportFile(data, "json");

            result.FileName.Should().Contain(today);
        }
    }
}
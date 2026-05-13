using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using System.Text.Json;
using System.Xml.Serialization;

namespace ChattyBot.Server.Application.Services
{
    public class ExportService : IExportService
    {
        public ExportedFileDTO CreateExportFile(ExportConversationDTO data, string format)
        {
            byte[] fileBytes;
            string contentType;
            string fileExtension = format.ToLower() == "xml" ? "xml" : "json";
            string fileName = $"ChattyBot_Export_{DateTime.Now:yyyyMMdd}.{fileExtension}";

            if (fileExtension == "xml")
            {
                contentType = "application/xml";
                var serializer = new XmlSerializer(typeof(ExportConversationDTO));
                using var ms = new MemoryStream();
                serializer.Serialize(ms, data);
                fileBytes = ms.ToArray();
            }
            else
            {
                contentType = "application/json";
                fileBytes = JsonSerializer.SerializeToUtf8Bytes(data, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }

            return new ExportedFileDTO(fileBytes, contentType, fileName);
        }
    }
}

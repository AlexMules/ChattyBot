using ChattyBot.Shared.Contracts.DTO;

namespace ChattyBot.Server.Application.Interfaces
{
    public interface IExportService
    {
        ExportedFileDTO CreateExportFile(ExportConversationDTO data, string format);
    }
}

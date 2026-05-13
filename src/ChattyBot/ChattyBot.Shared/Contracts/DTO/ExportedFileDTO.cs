namespace ChattyBot.Shared.Contracts.DTO
{
    public record ExportedFileDTO(byte[] Content, string ContentType, string FileName);
}

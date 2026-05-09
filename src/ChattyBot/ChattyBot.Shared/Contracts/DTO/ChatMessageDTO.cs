using System;
using System.Collections.Generic;
using System.Text;

namespace ChattyBot.Shared.Contracts.DTO
{
    public record ChatMessageDTO(int Id, string Content, string Sender, DateTime Timestamp);
}

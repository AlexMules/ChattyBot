using System;
using System.Collections.Generic;
using System.Text;

namespace ChattyBot.Shared.Contracts.DTO
{
    public record ChatConversationDTO(int Id, string Title, DateTime CreatedAt);
}

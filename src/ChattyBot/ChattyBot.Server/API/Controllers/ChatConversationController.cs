using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using ChattyBot.Server.Infrastructure.Security;

namespace ChattyBot.Server.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatConversationController : ControllerBase
    {
        private readonly IChatConversationService _conversationService;

        public ChatConversationController(IChatConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpGet("conversations")]
        public async Task<ActionResult<List<ChatConversationDTO>>> GetUserConversations()
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized();
            }

            var conversations = await _conversationService.GetChatConversationsByUserIdAsync(userId);
            return Ok(conversations);
        }

        [HttpPost]
        public async Task<ActionResult<ChatConversationDTO>> CreateChatConversation([FromBody] CreateChatDTO dto)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized();
            }

            var newChat = await _conversationService.CreateChatConversationAsync(userId, dto);
            return Ok(newChat);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChatConversation(int id)
        {
            var userId = User.GetUserId();
            if (userId == 0)
            {
                return Unauthorized();
            }

            if (!await _conversationService.IsUserOwnerAsync(userId, id))
            {
                return Forbid();
            }

            var result = await _conversationService.DeleteChatConversationAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpPut("{id}/rename")]
        public async Task<IActionResult> Rename(int id, [FromBody] RenameChatDTO dto)
        {
            var userId = User.GetUserId();
            var result = await _conversationService.RenameConversationAsync(userId, id, dto.NewTitle);

            return result switch
            {
                true => Ok(),
                false => Forbid(),
                null => NotFound()
            };
        }
    }
}
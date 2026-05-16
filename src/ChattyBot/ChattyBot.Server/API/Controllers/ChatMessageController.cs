using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using ChattyBot.Server.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChattyBot.Server.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatMessageController : ControllerBase
    {
        private readonly IChatMessageService _messageService;

        public ChatMessageController(IChatMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("conversation/{chatId}")]
        public async Task<ActionResult<List<ChatMessageDTO>>> GetHistory(int chatId)
        {
            try
            {
                var userId = User.GetUserId();
                if (userId == 0)
                {
                    return Unauthorized();
                }

                var messages = await _messageService.GetChatMessagesByConversationIdAsync(userId, chatId);
                return Ok(messages);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (InvalidOperationException)
            {
                return NotFound(new { message = "Conversation not found!" });
            }
        }

        [HttpPost("{chatId}/send")]
        public async Task<ActionResult<List<ChatMessageDTO>>> SendMessage(int chatId, [FromBody] SendMessageDTO dto)
        {
            try
            {
                var userId = User.GetUserId();
                if (userId == 0)
                {
                    return Unauthorized();
                }

                var username = User.Identity?.Name ?? "User";

                if (string.IsNullOrWhiteSpace(dto.Content))
                {
                    return BadRequest("Message content cannot be empty!");
                }

                var result = await _messageService.AddChatMessageAsync(userId, chatId, dto, username);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
        }
    }
}
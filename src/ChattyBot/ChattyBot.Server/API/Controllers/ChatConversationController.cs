using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Server.Infrastructure.Security;
using ChattyBot.Shared.Contracts.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChattyBot.Server.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatConversationController : ControllerBase
    {
        private readonly IChatConversationService _conversationService;
        private readonly IExportService _exportService;

        public ChatConversationController(IChatConversationService conversationService, IExportService exportService    )
        {
            _conversationService = conversationService;
            _exportService = exportService;
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

            var username = User.Identity?.Name ?? "User";

            var newChat = await _conversationService.CreateChatConversationAsync(userId, username, dto);

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
            if (userId == 0)
            {
                return Unauthorized();
            }

            var result = await _conversationService.RenameConversationAsync(userId, id, dto.NewTitle);

            return result switch
            {
                true => Ok(),
                false => Forbid(),
                null => NotFound()
            };
        }

        [HttpGet("{id}/export")]
        public async Task<IActionResult> Export(int id, [FromQuery] string format = "json")
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

            var data = await _conversationService.GetConversationForExportAsync(id);
            if (data == null)
            {
                return NotFound();
            }

            var export = _exportService.CreateExportFile(data, format);

            return File(export.Content, export.ContentType, export.FileName);
        }
    }
}
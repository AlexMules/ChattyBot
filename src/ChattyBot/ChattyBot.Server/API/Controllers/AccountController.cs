using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ChattyBot.Server.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountManagerService _accountService;

        public AccountController(IAccountManagerService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            var result = await _accountService.ChangePasswordAsync(GetUserId(), dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("change-username")]
        public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameDTO dto)
        {
            var result = await _accountService.ChangeUsernameAsync(GetUserId(), dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDTO dto)
        {
            var result = await _accountService.ChangeEmailAsync(GetUserId(), dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        // helper method to extract user ID from JWT claims
        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
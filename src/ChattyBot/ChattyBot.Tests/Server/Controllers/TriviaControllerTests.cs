using ChattyBot.Server.API.Controllers;
using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace ChattyBot.Tests.Server.Controllers
{
    public class TriviaControllerTests
    {
        private readonly ITriviaService _triviaService;
        private readonly TriviaController _sut;

        public TriviaControllerTests()
        {
            _triviaService = Substitute.For<ITriviaService>();
            _sut = new TriviaController(_triviaService);
        }

        [Fact]
        public async Task Verify_ShouldReturnOk_WhenAnswerIsProcessedSuccessfully()
        {
            var request = new TriviaCheckRequestDTO(1, 2, 123);
            var expectedResponse = new TriviaCheckResponseDTO(true, 2);

            _triviaService.VerifyAnswerAsync(request, request.MessageId)
                .Returns(expectedResponse);

            var result = await _sut.Verify(request);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(expectedResponse);

            await _triviaService.Received(1).VerifyAnswerAsync(request, request.MessageId);
        }

        [Fact]
        public async Task Verify_ShouldReturnNotFound_WhenMessageIdDoesNotExist()
        {
            var request = new TriviaCheckRequestDTO(1, 2, 999);
            var errorMessage = "Trivia message not found.";

            _triviaService.VerifyAnswerAsync(request, request.MessageId)
                .Throws(new KeyNotFoundException(errorMessage));

            var result = await _sut.Verify(request);

            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;

            notFoundResult.Value.Should().BeEquivalentTo(new { message = errorMessage });
        }
    }
}
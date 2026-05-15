using ChattyBot.Server.Application.BotEngine.Commands;
using ChattyBot.Server.Application.Interfaces;
using ChattyBot.Server.Domain.Enums;
using ChattyBot.Shared.Contracts.DTO;
using ChattyBot.Shared.Contracts.Enums;
using FluentAssertions;
using NSubstitute;
using System.Text.Json;

namespace ChattyBot.Tests.Server.Commands
{
    public class TriviaCommandTests
    {
        private readonly ITriviaService _service;
        private readonly TriviaCommand _sut;

        public TriviaCommandTests()
        {
            _service = Substitute.For<ITriviaService>();
            _sut = new TriviaCommand(_service);
        }

        [Fact]
        public void CommandTrigger_ShouldBeCorrect()
        {
            _sut.CommandTrigger.Should().Be("/trivia");
        }

        [Fact]
        public void Description_ShouldNotBeNullOrEmpty()
        {
            _sut.Description.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task ExecuteAsync_NoParams_ShouldCallServiceWithNullCategory()
        {
            var dto = new TriviaQuestionDTO(1, "What is C#?", new List<string> { "Language", "Snake" }, null, 0);
            _service.GetQuestionAsync(null).Returns(dto);

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.Trivia);
            await _service.Received(1).GetQuestionAsync(null);
        }

        [Theory]
        [InlineData("-football", TriviaCategory.Football)]
        [InlineData("-gaming", TriviaCategory.Gaming)]
        [InlineData("-science", TriviaCategory.Science)]
        [InlineData("-history", TriviaCategory.History)]
        public async Task ExecuteAsync_ValidCategory_ShouldCallServiceWithCorrectEnum(string param, TriviaCategory expected)
        {
            var dto = new TriviaQuestionDTO(1, "Test", new List<string> { "Opt" }, null, 0);
            _service.GetQuestionAsync(expected).Returns(dto);

            await _sut.ExecuteAsync(param);
            await _service.Received(1).GetQuestionAsync(expected);
        }

        [Fact]
        public async Task ExecuteAsync_InvalidCategory_ShouldReturnErrorMessage()
        {
            var result = await _sut.ExecuteAsync("-invalid");

            result.Type.Should().Be(MessageType.Text);
            result.Content.Should().Contain("I don't recognize that category");
            await _service.DidNotReceive().GetQuestionAsync(Arg.Any<TriviaCategory?>());
        }

        [Fact]
        public async Task ExecuteAsync_ServiceReturnsNull_ShouldReturnErrorMessage()
        {
            _service.GetQuestionAsync(Arg.Any<TriviaCategory?>()).Returns((TriviaQuestionDTO)null!);

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.Text);
            result.Content.Should().Contain("No questions found");
        }

        [Fact]
        public async Task ExecuteAsync_Success_ShouldReturnSerializedDtoWithCorrectProperties()
        {
            var dto = new TriviaQuestionDTO(
                QuestionId: 42,
                QuestionText: "Who wrote C#?",
                Options: new List<string> { "Bill Gates", "Anders Hejlsberg" },
                UserAnswerIndex: null,
                CorrectAnswerIndex: 1
            );
            _service.GetQuestionAsync(null).Returns(dto);

            var result = await _sut.ExecuteAsync();

            result.Type.Should().Be(MessageType.Trivia);

            var payload = JsonDocument.Parse(result.Content).RootElement;

            payload.GetProperty("QuestionId").GetInt32().Should().Be(42);
            payload.GetProperty("QuestionText").GetString().Should().Be("Who wrote C#?");
            payload.GetProperty("Options").GetArrayLength().Should().Be(2);
            payload.GetProperty("CorrectAnswerIndex").GetInt32().Should().Be(1);
        }
    }
}
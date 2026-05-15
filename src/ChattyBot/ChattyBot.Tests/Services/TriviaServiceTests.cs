using ChattyBot.Server.Application.Services;
using ChattyBot.Server.Infrastructure.Persistence.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using ChattyBot.Server.Domain.Entities;
using ChattyBot.Server.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace ChattyBot.Tests.Services
{
    public class TriviaServiceTests
    {
        private readonly ITriviaRepository _triviaRepository;
        private readonly IChatMessageRepository _messageRepository;
        private readonly TriviaService _sut;

        public TriviaServiceTests()
        {
            _triviaRepository = Substitute.For<ITriviaRepository>();
            _messageRepository = Substitute.For<IChatMessageRepository>();
            _sut = new TriviaService(_triviaRepository, _messageRepository);
        }

        [Fact]
        public async Task GetQuestionAsync_ShouldReturnRandomQuestion_WhenNoCategoryProvided()
        {
            var mockQuestion = new TriviaQuestion
            {
                Id = 1,
                QuestionText = "Random Question?",
                Options = new List<string> { "A", "B" }
            };
            _triviaRepository.GetRandomAsync().Returns(mockQuestion);

            var result = await _sut.GetQuestionAsync(null);

            result.Should().NotBeNull();
            result!.QuestionText.Should().Be("Random Question?");
            await _triviaRepository.Received(1).GetRandomAsync();
        }

        [Theory]
        [InlineData(TriviaCategory.Science, "What is DNA?")]
        [InlineData(TriviaCategory.History, "Who was Napoleon?")]
        [InlineData(TriviaCategory.Football, "Who won the World Cup in 2018?")]
        public async Task GetQuestionAsync_ShouldReturnCorrectCategory_WhenSpecificCategoryIsProvided(
            TriviaCategory category,
            string expectedQuestionText)
        {
            var mockQuestion = new TriviaQuestion
            {
                Id = 10,
                QuestionText = expectedQuestionText,
                Options = new List<string> { "Option A", "Option B" },
                Category = category
            };

            _triviaRepository.GetRandomByCategoryAsync(category).Returns(mockQuestion);

            var result = await _sut.GetQuestionAsync(category);

            result.Should().NotBeNull();
            result!.QuestionText.Should().Be(expectedQuestionText);

            await _triviaRepository.Received(1).GetRandomByCategoryAsync(category);
        }

        [Fact]
        public async Task GetQuestionAsync_ShouldReturnNull_WhenNoQuestionIsFound()
        {
            _triviaRepository.GetRandomAsync().Returns((TriviaQuestion?)null);
            var result = await _sut.GetQuestionAsync();
            result.Should().BeNull();
        }

        [Fact]
        public async Task VerifyAnswerAsync_ShouldReturnIsCorrectTrue_WhenAnswerMatches()
        {
            int questionId = 1;
            int answerIndex = 0;
            int messageId = 1223;

            var request = new TriviaCheckRequestDTO(questionId, answerIndex, messageId);

            var mockQuestion = new TriviaQuestion
            {
                Id = questionId,
                CorrectAnswerIndex = answerIndex,
                QuestionText = "What is the capital of Romania?",
                Options = new List<string> { "Bucharest", "Cluj-Napoca", "Sibiu", "Iasi" }
            };

            var mockMessage = new ChatMessage { Id = messageId, Content = "Initial content" };

            _triviaRepository.GetByIdAsync(questionId).Returns(mockQuestion);
            _messageRepository.GetByIdAsync(messageId).Returns(mockMessage);

            var result = await _sut.VerifyAnswerAsync(request, messageId);

            result.IsCorrect.Should().BeTrue();
            result.CorrectIndex.Should().Be(answerIndex);
        }

        [Fact]
        public async Task VerifyAnswerAsync_ShouldReturnIsCorrectFalse_WhenAnswerIsWrong()
        {
            int questionId = 1;
            int userAnswer = 0;
            int correctAnswer = 1;
            int messageId = 1223;

            var request = new TriviaCheckRequestDTO(questionId, userAnswer, messageId);

            var mockQuestion = new TriviaQuestion
            {
                Id = questionId,
                CorrectAnswerIndex = correctAnswer,
                QuestionText = "Is 1+1=3?",
                Options = new List<string> { "Yes", "No" }
            };

            _triviaRepository.GetByIdAsync(questionId).Returns(mockQuestion);

            var result = await _sut.VerifyAnswerAsync(request, messageId);

            result.IsCorrect.Should().BeFalse();
            result.CorrectIndex.Should().Be(correctAnswer);
        }

        [Fact]
        public async Task VerifyAnswerAsync_ShouldThrowKeyNotFoundException_WhenQuestionDoesNotExist()
        {
            int invalidQuestionId = 999;
            var request = new TriviaCheckRequestDTO(invalidQuestionId, 0, 123);

            _triviaRepository.GetByIdAsync(invalidQuestionId).Returns((TriviaQuestion?)null);

            await _sut.Invoking(s => s.VerifyAnswerAsync(request, 123))
                .Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Question not found!");
        }

        [Fact]
        public async Task VerifyAnswerAsync_ShouldUpdateMessage_WhenMessageExists()
        {
            int questionId = 1;
            int messageId = 500;
            int answerIndex = 0;
            var request = new TriviaCheckRequestDTO(questionId, answerIndex, messageId);

            var mockQuestion = new TriviaQuestion
            {
                Id = questionId,
                CorrectAnswerIndex = 1,
                Options = new List<string> { "Option A", "Option B" },
                QuestionText = "Is this a test question?"
            };
            var mockMessage = new ChatMessage { Id = messageId, Content = "Old Content" };

            _triviaRepository.GetByIdAsync(questionId).Returns(mockQuestion);
            _messageRepository.GetByIdAsync(messageId).Returns(mockMessage);

            await _sut.VerifyAnswerAsync(request, messageId);

            await _messageRepository.Received(1).UpdateAsync(Arg.Is<ChatMessage>(m =>
                m.Id == messageId &&
                m.Content.Contains("0") && 
                m.Content.ToLower().Contains("answerindex")
            ));
        }

        [Fact]
        public async Task VerifyAnswerAsync_ShouldReturnResult_WhenMessageDoesNotExist()
        {
            int questionId = 1;
            int answerIndex = 0;
            int missingMessageId = 404; 
            var request = new TriviaCheckRequestDTO(questionId, answerIndex, missingMessageId);

            var mockQuestion = new TriviaQuestion
            {
                Id = questionId,
                CorrectAnswerIndex = answerIndex,
                QuestionText = "Is resilience important?",
                Options = new List<string> { "Yes", "No" }
            };

            _triviaRepository.GetByIdAsync(questionId).Returns(mockQuestion);

            _messageRepository.GetByIdAsync(missingMessageId).Returns((ChatMessage?)null);

            var result = await _sut.VerifyAnswerAsync(request, missingMessageId);

            result.Should().NotBeNull();
            result.IsCorrect.Should().BeTrue();
            result.CorrectIndex.Should().Be(answerIndex);

            await _messageRepository.DidNotReceiveWithAnyArgs().UpdateAsync(Arg.Any<ChatMessage>());
        }
    }
}
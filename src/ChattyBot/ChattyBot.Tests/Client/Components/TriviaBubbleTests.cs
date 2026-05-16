using Bunit;
using ChattyBot.Client.Components;
using ChattyBot.Client.Services.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Text.Json;

namespace ChattyBot.Tests.Client.Components
{
    public class TriviaBubbleTests : BunitContext
    {
        private readonly ITriviaClient _triviaClientMock;

        public TriviaBubbleTests()
        {
            _triviaClientMock = Substitute.For<ITriviaClient>();
            Services.AddSingleton(_triviaClientMock);
        }

        [Fact]
        public void Render_ShouldDisplayQuestionAndOptions_WhenNotYetAnswered()
        {
            var question = new TriviaQuestionDTO(1, "What is the capital of France?", new List<string> { "London", "Berlin", "Paris" }, null, null);
            string json = JsonSerializer.Serialize(question);

            var cut = Render<TriviaBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
                .Add(p => p.MessageId, 100)
            );

            cut.Find(".trivia-question").TextContent.Trim().Should().Be("What is the capital of France?");

            var buttons = cut.FindAll(".option-btn");
            buttons.Count.Should().Be(3);
            buttons[0].TextContent.Trim().Should().Be("London");
            buttons[1].TextContent.Trim().Should().Be("Berlin");
            buttons[2].TextContent.Trim().Should().Be("Paris");

            buttons.Any(b => b.HasAttribute("disabled")).Should().BeFalse();
            cut.FindAll(".feedback-area").Should().BeEmpty();
        }

        [Fact]
        public void Render_ShouldDisplayPreExistingCorrectAnswer_FromHistory()
        {
            var question = new TriviaQuestionDTO(1, "Question", new List<string> { "A", "B" }, 1, 1);
            string json = JsonSerializer.Serialize(question);

            var cut = Render<TriviaBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
            );

            var buttons = cut.FindAll(".option-btn");
            buttons[0].ClassList.Should().Contain("faded");
            buttons[1].ClassList.Should().Contain("correct");
            buttons.All(b => b.HasAttribute("disabled")).Should().BeTrue();

            var feedback = cut.Find(".feedback-area");
            feedback.ClassList.Should().Contain("feedback-correct");
            feedback.TextContent.Should().Contain("Correct!");
        }

        [Fact]
        public void Render_ShouldDisplayPreExistingWrongAnswer_FromHistory()
        {
            var question = new TriviaQuestionDTO(1, "Question", new List<string> { "A", "B" }, 0, 1);
            string json = JsonSerializer.Serialize(question);

            var cut = Render<TriviaBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
            );

            var buttons = cut.FindAll(".option-btn");
            buttons[0].ClassList.Should().Contain("wrong");
            buttons[1].ClassList.Should().Contain("correct");

            var feedback = cut.Find(".feedback-area");
            feedback.ClassList.Should().Contain("feedback-wrong");
            feedback.TextContent.Should().Contain("Wrong! The correct answer is: B");
        }

        [Fact]
        public async Task HandleOptionClick_ShouldCallApiTriggerCallbackAndShowCorrectFeedback()
        {
            var question = new TriviaQuestionDTO(1, "Question", new List<string> { "A", "B" }, null, null);
            string json = JsonSerializer.Serialize(question);
            bool callbackFired = false;

            var apiResponse = new TriviaCheckResponseDTO(true, 1);
            _triviaClientMock.VerifyAnswerAsync(Arg.Any<TriviaCheckRequestDTO>()).Returns(Task.FromResult(apiResponse));

            var cut = Render<TriviaBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
                .Add(p => p.MessageId, 100)
                .Add(p => p.OnAnswerProvided, () => callbackFired = true)
            );

            await cut.FindAll(".option-btn")[1].ClickAsync();

            cut.WaitForAssertion(() =>
            {
                callbackFired.Should().BeTrue();
                cut.Find(".feedback-area").ClassList.Should().Contain("feedback-correct");
                cut.FindAll(".option-btn")[1].ClassList.Should().Contain("correct");
            });
        }

        [Fact]
        public async Task HandleOptionClick_ShouldShowWrongFeedback_WhenAnswerIsIncorrect()
        {
            var question = new TriviaQuestionDTO(1, "Question", new List<string> { "A", "B" }, null, null);
            string json = JsonSerializer.Serialize(question);

            var apiResponse = new TriviaCheckResponseDTO(false, 1);
            _triviaClientMock.VerifyAnswerAsync(Arg.Any<TriviaCheckRequestDTO>()).Returns(Task.FromResult(apiResponse));

            var cut = Render<TriviaBubble>(parameters => parameters
                .Add(p => p.RawContent, json)
                .Add(p => p.MessageId, 100)
            );

            await cut.FindAll(".option-btn")[0].ClickAsync();

            cut.WaitForAssertion(() =>
            {
                var buttons = cut.FindAll(".option-btn");
                buttons[0].ClassList.Should().Contain("wrong");
                buttons[1].ClassList.Should().Contain("correct");
                cut.Find(".feedback-area").ClassList.Should().Contain("feedback-wrong");
            });
        }

        [Fact]
        public void Render_ShouldHideContainer_WhenJsonIsMalformed()
        {
            string invalidJson = "{ invalid }";

            var cut = Render<TriviaBubble>(parameters => parameters
                .Add(p => p.RawContent, invalidJson)
            );

            cut.FindAll(".trivia-container").Should().BeEmpty();
        }
    }
}
using Bunit;
using ChattyBot.Client.Pages;
using ChattyBot.Client.Services.Interfaces;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Security.Claims;

namespace ChattyBot.Tests.Client.Pages
{
    public class RegisterTests : BunitContext
    {
        private readonly IAuthClient _authClientMock;
        private readonly AuthenticationStateProvider _authProviderMock;
        private readonly NavigationManager _navManager;

        public RegisterTests()
        {
            _authClientMock = Substitute.For<IAuthClient>();
            _authProviderMock = Substitute.For<AuthenticationStateProvider>();

            Services.AddSingleton(_authClientMock);
            Services.AddSingleton(_authProviderMock);

            _navManager = Services.GetRequiredService<NavigationManager>();
        }

        private void SetupAuthState(bool isAuthenticated)
        {
            var identity = isAuthenticated
                ? new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestUser") }, "TestAuth")
                : new ClaimsIdentity();

            var user = new ClaimsPrincipal(identity);

            _authProviderMock.GetAuthenticationStateAsync()
                .Returns(Task.FromResult(new AuthenticationState(user)));
        }

        [Fact]
        public void OnInitialized_ShouldRedirectToHome_WhenUserIsAlreadyAuthenticated()
        {
            SetupAuthState(true);
            Render<Register>();
            _navManager.Uri.Should().EndWith("home");
        }

        [Fact]
        public async Task HandleRegister_ShouldShowSuccessAndRedirect_WhenApiSucceeds()
        {
            SetupAuthState(false);
            var apiResult = new AuthResponseDTO { IsSuccess = true };
            _authClientMock.RegisterAsync(Arg.Any<RegisterDTO>()).Returns(Task.FromResult(apiResult));

            var cut = Render<Register>();

            cut.Find("input[placeholder='Choose a username...']").Change("TestUser");
            cut.Find("input[placeholder='user@domain.com']").Change("testuser@domain.com");
            cut.Find("input[placeholder='Enter new password...']").Change("Password123!");

            await cut.Find("form").SubmitAsync();

            cut.Find(".alert-success").TextContent.Should().Contain("Account created successfully");

            cut.WaitForAssertion(() => _navManager.Uri.Should().EndWith("/login"), TimeSpan.FromSeconds(3));
        }

        [Fact]
        public async Task HandleRegister_ShouldShowError_WhenApiReturnsFailure()
        {
            SetupAuthState(false);
            var apiResult = new AuthResponseDTO { IsSuccess = false, ErrorMessage = "Email already in use" };
            _authClientMock.RegisterAsync(Arg.Any<RegisterDTO>()).Returns(Task.FromResult(apiResult));

            var cut = Render<Register>();

            cut.Find("input[placeholder='Choose a username...']").Change("TestUser");
            cut.Find("input[placeholder='user@domain.com']").Change("testuser@domain.com");
            cut.Find("input[placeholder='Enter new password...']").Change("Password123!");

            await cut.Find("form").SubmitAsync();

            cut.Find(".alert-danger").TextContent.Should().Contain("Email already in use");
            _navManager.Uri.Should().NotEndWith("/login");
        }

        [Fact]
        public async Task HandleRegister_ShouldShowGenericError_WhenExceptionOccurs()
        {
            SetupAuthState(false);
            _authClientMock.RegisterAsync(Arg.Any<RegisterDTO>()).Returns(Task.FromException<AuthResponseDTO>(new Exception("Database down")));

            var cut = Render<Register>();

            cut.Find("input[placeholder='Choose a username...']").Change("TestUser");
            cut.Find("input[placeholder='user@domain.com']").Change("testuser@domain.com");
            cut.Find("input[placeholder='Enter new password...']").Change("Password123!");

            await cut.Find("form").SubmitAsync();

            cut.Find(".alert-danger").TextContent.Should().Contain("Something went wrong");
        }

        [Fact]
        public async Task SubmitButton_ShouldBeDisabled_DuringSubmission()
        {
            SetupAuthState(false);

            var tcs = new TaskCompletionSource<AuthResponseDTO>();
            _authClientMock.RegisterAsync(Arg.Any<RegisterDTO>()).Returns(tcs.Task);

            var cut = Render<Register>();

            cut.Find("input[placeholder='Choose a username...']").Change("TestUser");
            cut.Find("input[placeholder='user@domain.com']").Change("testuser@domain.com");
            cut.Find("input[placeholder='Enter new password...']").Change("Password123!");

            var submitTask = cut.Find("form").SubmitAsync();

            cut.WaitForAssertion(() =>
            {
                var button = cut.Find("button[type='submit']");
                button.HasAttribute("disabled").Should().BeTrue("The button should be disabled while waiting for the server");
                button.InnerHtml.Should().Contain("spinner-border");
            }, TimeSpan.FromSeconds(2));

            tcs.SetResult(new AuthResponseDTO { IsSuccess = true });
            await submitTask;
        }

        [Fact]
        public void AvatarSelection_ShouldApplySelectedClass()
        {
            SetupAuthState(false);
            var cut = Render<Register>();

            cut.FindAll(".avatar-img")[1].Click();

            cut.FindAll(".avatar-img")[1].ClassList.Should().Contain("selected");
            cut.FindAll(".avatar-img")[0].ClassList.Should().NotContain("selected");
        }

        [Fact]
        public void TogglePassword_ShouldWork()
        {
            SetupAuthState(false);
            var cut = Render<Register>();
            var input = cut.Find("input[placeholder='Enter new password...']");

            cut.Find(".btn-eye").Click();
            input.GetAttribute("type").Should().Be("text");

            cut.Find(".btn-eye").Click();
            input.GetAttribute("type").Should().Be("password");
        }
    }
}
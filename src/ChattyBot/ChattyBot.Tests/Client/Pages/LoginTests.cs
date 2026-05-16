using Blazored.LocalStorage;
using Bunit;
using ChattyBot.Client.Pages;
using ChattyBot.Client.Services.Interfaces;
using ChattyBot.Client.Services.State;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;

namespace ChattyBot.Tests.Client.Pages
{
    public class LoginTests : BunitContext
    {
        private readonly IAuthClient _authClientMock;
        private readonly CustomAuthStateProvider _authProviderMock;
        private readonly NavigationManager _navManager;

        public LoginTests()
        {
            _authClientMock = Substitute.For<IAuthClient>();

            var localStorageMock = Substitute.For<ILocalStorageService>();
            _authProviderMock = Substitute.For<CustomAuthStateProvider>(localStorageMock, new HttpClient());

            Services.AddSingleton(_authClientMock);
            Services.AddSingleton<AuthenticationStateProvider>(_authProviderMock);

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
            SetupAuthState(isAuthenticated: true);

            Render<Login>();

            _navManager.Uri.Should().EndWith("home");
        }

        [Fact]
        public async Task HandleLogin_ShouldAuthenticateAndRedirect_WhenApiSucceeds()
        {
            SetupAuthState(isAuthenticated: false);

            string dummyJwt = "header.eyJuYW1lIjoiVGVzdFVzZXIifQ==.signature";
            var apiResult = new AuthResponseDTO { IsSuccess = true, Token = dummyJwt };
            _authClientMock.LoginAsync(Arg.Any<LoginDTO>()).Returns(Task.FromResult(apiResult));

            var cut = Render<Login>();

            cut.FindAll("input")[0].Change("testuser@domain.com");
            cut.FindAll("input")[1].Change("ValidPassword123!");

            await cut.Find("form").SubmitAsync();

            cut.WaitForAssertion(() => _navManager.Uri.Should().EndWith("/home"));
        }

        [Fact]
        public async Task HandleLogin_ShouldShowError_WhenApiReturnsFailure()
        {
            SetupAuthState(isAuthenticated: false);
            var apiResult = new AuthResponseDTO { IsSuccess = false, ErrorMessage = "Invalid credentials" };
            _authClientMock.LoginAsync(Arg.Any<LoginDTO>()).Returns(Task.FromResult(apiResult));

            var cut = Render<Login>();

            cut.FindAll("input")[0].Change("testuser@domain.com");
            cut.FindAll("input")[1].Change("WrongPassword123!");

            await cut.Find("form").SubmitAsync();

            cut.WaitForAssertion(() =>
            {
                var alert = cut.Find(".alert-danger");
                alert.Should().NotBeNull();
                alert.TextContent.Should().Contain("Invalid credentials");
            });
        }

        [Fact]
        public async Task HandleLogin_ShouldShowGenericError_WhenExceptionOccurs()
        {
            SetupAuthState(isAuthenticated: false);

            _authClientMock.LoginAsync(Arg.Any<LoginDTO>())
                .Returns(Task.FromException<AuthResponseDTO>(new Exception("Network down")));

            var cut = Render<Login>();

            cut.FindAll("input")[0].Change("testuser@domain.com");
            cut.FindAll("input")[1].Change("ValidPassword123!");

            await cut.Find("form").SubmitAsync();

            cut.WaitForAssertion(() =>
            {
                var alert = cut.Find(".alert-danger");
                alert.Should().NotBeNull();
                alert.TextContent.Should().Contain("Login failed. Please check your connection");
            });
        }

        [Fact]
        public async Task SubmitButton_ShouldBeDisabled_DuringSubmission()
        {
            SetupAuthState(isAuthenticated: false);

            var tcs = new TaskCompletionSource<AuthResponseDTO>();
            _authClientMock.LoginAsync(Arg.Any<LoginDTO>()).Returns(tcs.Task);

            var cut = Render<Login>();

            cut.FindAll("input")[0].Change("testuser@domain.com");
            cut.FindAll("input")[1].Change("ValidPassword123!");

            var submitTask = cut.Find("form").SubmitAsync();

            cut.WaitForAssertion(() =>
            {
                var button = cut.Find("button[type='submit']");
                button.HasAttribute("disabled").Should().BeTrue();
                button.InnerHtml.Should().Contain("spinner-border");
            }, TimeSpan.FromSeconds(2));

            tcs.SetResult(new AuthResponseDTO { IsSuccess = true, Token = "header.eyJuYW1lIjoiVGVzdFVzZXIifQ==.signature" });
            await submitTask;
        }
    }
}

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

namespace ChattyBot.Tests.Client.Pages
{
    public class ManageAccountTests : BunitContext
    {
        private readonly IManageAccountClient _accountApiMock;
        private readonly CustomAuthStateProvider _authProviderMock;

        public ManageAccountTests()
        {
            _accountApiMock = Substitute.For<IManageAccountClient>();

            var localStorageMock = Substitute.For<ILocalStorageService>();
            _authProviderMock = Substitute.For<CustomAuthStateProvider>(localStorageMock, new HttpClient());

            Services.AddSingleton(_accountApiMock);
            Services.AddSingleton<AuthenticationStateProvider>(_authProviderMock);
        }

        private void SetupAuthState()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.Email, "testuser@domain.com")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            _authProviderMock.GetAuthenticationStateAsync()
                .Returns(Task.FromResult(new AuthenticationState(user)));
        }

        [Fact]
        public void OnInitialized_ShouldPreFillUsernameAndEmail_FromClaims()
        {
            SetupAuthState();

            var cut = Render<ManageAccount>();

            var inputs = cut.FindAll("input");
            inputs[0].GetAttribute("value").Should().Be("TestUser");
            inputs[1].GetAttribute("value").Should().Be("testuser@domain.com");
        }


        [Fact]
        public async Task HandleChangeUsername_ShouldShowSuccess_WhenApiSucceeds()
        {
            SetupAuthState();

            string dummyJwt = "header.eyJuYW1lIjoiVGVzdFVzZXIifQ==.signature";
            var apiResult = new AuthResponseDTO { IsSuccess = true, Token = dummyJwt };
            _accountApiMock.ChangeUsernameAsync(Arg.Any<ChangeUsernameDTO>()).Returns(Task.FromResult(apiResult));

            var cut = Render<ManageAccount>();

            cut.FindAll("input")[0].Change("NewTestUser");
            await cut.FindAll("form")[0].SubmitAsync();

            cut.WaitForAssertion(() => cut.Find(".alert-success").TextContent.Should().Contain("Username updated!"));
        }

        [Fact]
        public async Task HandleChangeUsername_ShouldShowError_WhenApiFails()
        {
            SetupAuthState();
            var apiResult = new AuthResponseDTO { IsSuccess = false, ErrorMessage = "Username taken" };
            _accountApiMock.ChangeUsernameAsync(Arg.Any<ChangeUsernameDTO>()).Returns(Task.FromResult(apiResult));

            var cut = Render<ManageAccount>();

            cut.FindAll("input")[0].Change("NewTestUser");
            await cut.FindAll("form")[0].SubmitAsync();

            cut.WaitForAssertion(() => cut.Find(".alert-danger").TextContent.Should().Contain("Username taken"));
        }

        [Fact]
        public async Task HandleChangeEmail_ShouldShowSuccessAndClearPassword_WhenApiSucceeds()
        {
            SetupAuthState();

            string dummyJwt = "header.eyJuYW1lIjoiVGVzdFVzZXIifQ==.signature";
            var apiResult = new AuthResponseDTO { IsSuccess = true, Token = dummyJwt };
            _accountApiMock.ChangeEmailAsync(Arg.Any<ChangeEmailDTO>()).Returns(Task.FromResult(apiResult));

            var cut = Render<ManageAccount>();

            cut.FindAll("input")[1].Change("newemail@domain.com");
            cut.FindAll("input")[2].Change("MyPassword123!");

            await cut.FindAll("form")[1].SubmitAsync();

            cut.WaitForAssertion(() =>
            {
                cut.Find(".alert-success").TextContent.Should().Contain("Email updated successfully!");
                cut.FindAll("input")[2].GetAttribute("value").Should().BeNullOrEmpty();
            });
        }

        [Fact]
        public async Task HandleChangeEmail_ShouldShowError_WhenApiFails()
        {
            SetupAuthState();
            var apiResult = new AuthResponseDTO { IsSuccess = false, ErrorMessage = "Incorrect current password" };
            _accountApiMock.ChangeEmailAsync(Arg.Any<ChangeEmailDTO>()).Returns(Task.FromResult(apiResult));

            var cut = Render<ManageAccount>();

            cut.FindAll("input")[1].Change("wrongemail@domain.com");
            cut.FindAll("input")[2].Change("WrongPassword!");

            await cut.FindAll("form")[1].SubmitAsync();

            cut.WaitForAssertion(() =>
            {
                var errorAlert = cut.FindAll("form")[1].QuerySelector(".alert-danger");
                errorAlert.Should().NotBeNull();
                errorAlert!.TextContent.Should().Contain("Incorrect current password");
            });
        }

        [Fact]
        public async Task HandleChangePassword_ShouldShowSuccessAndClearPasswords_WhenApiSucceeds()
        {
            SetupAuthState();
            var apiResult = new AuthResponseDTO { IsSuccess = true };
            _accountApiMock.ChangePasswordAsync(Arg.Any<ChangePasswordDTO>()).Returns(Task.FromResult(apiResult));

            var cut = Render<ManageAccount>();

            cut.FindAll("input")[3].Change("OldPassword123!");
            cut.FindAll("input")[4].Change("NewPassword123!");

            await cut.FindAll("form")[2].SubmitAsync();

            cut.WaitForAssertion(() =>
            {
                cut.Find(".alert-success").TextContent.Should().Contain("Password changed successfully!");
                cut.FindAll("input")[3].GetAttribute("value").Should().BeNullOrEmpty();
                cut.FindAll("input")[4].GetAttribute("value").Should().BeNullOrEmpty();
            });
        }

        [Fact]
        public async Task HandleChangePassword_ShouldShowError_WhenApiFails()
        {
            SetupAuthState();
            var apiResult = new AuthResponseDTO { IsSuccess = false, ErrorMessage = "New password is too weak" };
            _accountApiMock.ChangePasswordAsync(Arg.Any<ChangePasswordDTO>()).Returns(Task.FromResult(apiResult));

            var cut = Render<ManageAccount>();

            cut.FindAll("input")[3].Change("OldPassword123!");

            cut.FindAll("input")[4].Change("WeakPass1!");

            await cut.FindAll("form")[2].SubmitAsync(); 

            cut.WaitForAssertion(() =>
            {
                var errorAlert = cut.FindAll("form")[2].QuerySelector(".alert-danger");
                errorAlert.Should().NotBeNull();
                errorAlert!.TextContent.Should().Contain("New password is too weak");
            });
        }

        [Fact]
        public async Task SubmitButtons_ShouldBeDisabled_DuringProcessing()
        {
            SetupAuthState();

            var tcs = new TaskCompletionSource<AuthResponseDTO>();
            _accountApiMock.ChangeUsernameAsync(Arg.Any<ChangeUsernameDTO>()).Returns(tcs.Task);

            var cut = Render<ManageAccount>();

            cut.FindAll("input")[0].Change("ProcessingTest");
            var submitTask = cut.FindAll("form")[0].SubmitAsync();

            cut.WaitForAssertion(() =>
            {
                var button = cut.FindAll("form")[0].QuerySelector("button[type='submit']");
                button!.HasAttribute("disabled").Should().BeTrue();
            }, TimeSpan.FromSeconds(2));

            tcs.SetResult(new AuthResponseDTO { IsSuccess = true });
            await submitTask;
        }
    }
}
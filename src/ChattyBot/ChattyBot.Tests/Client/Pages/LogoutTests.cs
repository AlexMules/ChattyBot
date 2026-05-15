using Blazored.LocalStorage;
using Bunit;
using ChattyBot.Client.Services.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using NSubstitute;
using System.Security.Claims;
using ChattyBot.Client.Pages;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace ChattyBot.Client.Tests.Pages
{
    public class LogoutTests : BunitContext
    {
        private readonly CustomAuthStateProvider _authProviderMock;
        private readonly NavigationManager _navManager;

        public LogoutTests()
        {
            var localStorageMock = Substitute.For<ILocalStorageService>();

            _authProviderMock = Substitute.For<CustomAuthStateProvider>(localStorageMock, new HttpClient());

            Services.AddSingleton<AuthenticationStateProvider>(_authProviderMock);

            _navManager = Services.GetRequiredService<NavigationManager>();
        }

        [Fact]
        public void OnInitialized_ShouldExecuteLogoutAndRedirectToLogin()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "TestUser") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            _authProviderMock.GetAuthenticationStateAsync()
                .Returns(Task.FromResult(new AuthenticationState(user)));

            var cut = Render<Logout>();

            _navManager.Uri.Should().EndWith("login");
        }
    }
}

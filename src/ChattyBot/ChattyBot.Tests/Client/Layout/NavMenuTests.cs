using Bunit;
using ChattyBot.Client.Layout;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Security.Claims;

namespace ChattyBot.Tests.Client.Layout
{
    public class NavMenuTests : BunitContext
    {
        private void SetupAuthentication(bool isAuthorized, string username = "")
        {
            Services.AddAuthorizationCore();

            var authProviderMock = Substitute.For<AuthenticationStateProvider>();
            ClaimsPrincipal user;

            if (isAuthorized)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, username) };
                var identity = new ClaimsIdentity(claims, "TestAuth");
                user = new ClaimsPrincipal(identity);
            }
            else
            {
                user = new ClaimsPrincipal(new ClaimsIdentity());
            }

            var authState = new AuthenticationState(user);
            var authStateTask = Task.FromResult(authState);
            authProviderMock.GetAuthenticationStateAsync().Returns(authStateTask);
            Services.AddSingleton(authProviderMock);

            var authServiceMock = Substitute.For<IAuthorizationService>();
            var authResult = isAuthorized ? AuthorizationResult.Success() : AuthorizationResult.Failed();

            authServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(), Arg.Any<string>())
                .Returns(Task.FromResult(authResult));
            authServiceMock.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
                .Returns(Task.FromResult(authResult));

            Services.AddSingleton(authServiceMock);

            RenderTree.Add<CascadingValue<Task<AuthenticationState>>>(parameters => parameters
                .Add(p => p.Value, authStateTask)
            );
        }

        [Fact]
        public void Render_ShouldShowAuthorizedLinks_WhenUserIsAuthenticated()
        {
            SetupAuthentication(true, "Alexandru");

            var cut = Render<NavMenu>();

            cut.Find("a[href='home']").TextContent.Trim().Should().Contain("Profile");
            cut.Find("a[href='chat']").TextContent.Trim().Should().Contain("Chats");
            cut.Find("a[href='account/manage']").TextContent.Trim().Should().Contain("Manage Account");
            cut.Find("a[href='logout']").TextContent.Trim().Should().Contain("Logout");

            cut.FindAll("a[href='login']").Should().BeEmpty();
            cut.FindAll("a[href='register']").Should().BeEmpty();
        }

        [Fact]
        public void Render_ShouldShowAnonymousLinks_WhenUserIsNotAuthenticated()
        {
            SetupAuthentication(false);

            var cut = Render<NavMenu>();

            cut.Find("a[href='login']").TextContent.Trim().Should().Contain("Login");
            cut.Find("a[href='register']").TextContent.Trim().Should().Contain("Register");

            cut.FindAll("a[href='home']").Should().BeEmpty();
            cut.FindAll("a[href='chat']").Should().BeEmpty();
            cut.FindAll("a[href='account/manage']").Should().BeEmpty();
            cut.FindAll("a[href='logout']").Should().BeEmpty();
        }

        [Fact]
        public void ToggleNavMenu_ShouldToggleCollapseClass_WhenInteracted()
        {
            SetupAuthentication(false);
            var cut = Render<NavMenu>();

            cut.Find(".nav-scrollable").ClassList.Should().Contain("collapse");

            cut.Find(".navbar-toggler").Click();

            cut.Find(".nav-scrollable").ClassList.Should().NotContain("collapse");
            cut.Find(".nav-scrollable").Click();

            cut.Find(".nav-scrollable").ClassList.Should().Contain("collapse");
        }
    }
}
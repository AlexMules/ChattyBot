using Bunit;
using Bunit.TestDoubles;
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
    public class MainLayoutTests : BunitContext
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
        public void Render_ShouldShowLogoutButton_WhenUserIsAuthorized()
        {
            SetupAuthentication(true, "Alexandru");
            ComponentFactories.AddStub<NavMenu>();

            var cut = Render<MainLayout>(parameters => parameters
                .Add(p => p.Body, (RenderFragment)(builder => builder.AddContent(0, "<h1>Welcome Home</h1>")))
            );

            cut.Find("a[href='logout']").TextContent.Trim().Should().Be("Logout");

            cut.FindAll("a[href='login']").Should().BeEmpty();
            cut.FindAll("a[href='register']").Should().BeEmpty();

            cut.Find("article.content").TextContent.Should().Contain("Welcome Home");
        }

        [Fact]
        public void Render_ShouldShowLoginAndRegisterButtons_WhenUserIsAnonymous()
        {
            SetupAuthentication(false);
            ComponentFactories.AddStub<NavMenu>();

            var cut = Render<MainLayout>();

            cut.Find("a[href='login']").TextContent.Trim().Should().Be("Login");
            cut.Find("a[href='register']").TextContent.Trim().Should().Be("Register");

            cut.FindAll("a[href='logout']").Should().BeEmpty();
        }

        [Fact]
        public void Render_ShouldIncludeNavMenuComponentInSidebar()
        {
            SetupAuthentication(false);
            ComponentFactories.AddStub<NavMenu>();

            var cut = Render<MainLayout>();

            cut.FindComponents<Stub<NavMenu>>().Should().NotBeEmpty();
            cut.Find(".sidebar").Should().NotBeNull();
        }
    }
}
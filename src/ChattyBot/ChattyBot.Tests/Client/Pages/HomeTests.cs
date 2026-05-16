using Bunit;
using ChattyBot.Client.Pages;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Security.Claims;

namespace ChattyBot.Tests.Client.Pages
{
    public class HomeTests : BunitContext
    {
        private readonly AuthenticationStateProvider _authProviderMock;
        private readonly NavigationManager _navManager;

        public HomeTests()
        {
            _authProviderMock = Substitute.For<AuthenticationStateProvider>();

            Services.AddSingleton(_authProviderMock);
            _navManager = Services.GetRequiredService<NavigationManager>();
        }

        private void SetupAuthState(string? username = "TestUser", string? email = "testuser@domain.com", string? avatar = "avatar2.png")
        {
            var claims = new List<Claim>();

            if (username != null)
            {
                claims.Add(new Claim(ClaimTypes.Name, username));
            }
            if (email != null)
            {
                claims.Add(new Claim(ClaimTypes.Email, email));
            }
            if (avatar != null)
            {
                claims.Add(new Claim("avatar", avatar));
            }

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            _authProviderMock.GetAuthenticationStateAsync()
                .Returns(Task.FromResult(new AuthenticationState(user)));
        }

        [Fact]
        public void OnInitialized_ShouldLoadUserData_FromClaims()
        {
            SetupAuthState(username: "testuser", email: "testuser@domain.com", avatar: "avatar2.png");

            var cut = Render<Home>();

            cut.Find(".welcome-on-banner").TextContent.Should().Contain("Welcome, testuser!");
            cut.Find(".display-name").TextContent.Should().Be("testuser");

            cut.Find(".profile-picture").GetAttribute("src").Should().EndWith("avatar2.png");

            var usernameRow = cut.FindAll(".detail-row")[0];
            usernameRow.QuerySelector(".detail-value")!.TextContent.Should().Be("testuser");

            var emailRow = cut.FindAll(".detail-row")[1];
            emailRow.QuerySelector(".detail-value")!.TextContent.Should().Contain("********@domain.com");
        }

        [Fact]
        public void OnInitialized_ShouldUseFallbackValues_WhenClaimsAreMissing()
        {
            SetupAuthState(username: null, email: null, avatar: null);

            var cut = Render<Home>();

            cut.Find(".display-name").TextContent.Should().Be("Unknown User");
            cut.Find(".profile-picture").GetAttribute("src").Should().EndWith("avatar1.png");

            var emailRow = cut.FindAll(".detail-row")[1];
            emailRow.QuerySelector(".detail-value")!.TextContent.Should().Contain("No email found");
        }

        [Fact]
        public void ToggleReveal_ShouldToggleEmailMasking_WhenClicked()
        {
            SetupAuthState(username: "TestUser", email: "testuser@domain.com", avatar: "avatar1.png");
            var cut = Render<Home>();

            cut.FindAll(".detail-row")[1].QuerySelector(".detail-value")!.TextContent.Should().Contain("********@domain.com");
            cut.Find(".reveal-link").TextContent.Should().Be("Reveal");

            cut.Find(".reveal-link").Click();

            cut.FindAll(".detail-row")[1].QuerySelector(".detail-value")!.TextContent.Should().Contain("testuser@domain.com");
            cut.Find(".reveal-link").TextContent.Should().Be("Hide");

            cut.Find(".reveal-link").Click();

            cut.FindAll(".detail-row")[1].QuerySelector(".detail-value")!.TextContent.Should().Contain("********@domain.com");
            cut.Find(".reveal-link").TextContent.Should().Be("Reveal");
        }

        [Fact]
        public void GoToManage_ShouldNavigateToAccountManage_WhenEditButtonIsClicked()
        {
            SetupAuthState();
            var cut = Render<Home>();

            var editButtons = cut.FindAll("button").Where(b => b.TextContent.Contains("Edit")).ToList();

            editButtons.Count.Should().Be(3);

            editButtons[0].Click();

            _navManager.Uri.Should().EndWith("/account/manage");
        }
    }
}
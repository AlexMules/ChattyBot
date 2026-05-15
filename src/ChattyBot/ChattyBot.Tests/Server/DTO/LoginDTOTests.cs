using System.ComponentModel.DataAnnotations;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;

namespace ChattyBot.Tests.Server.DTO
{
    public class LoginDTOTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void LoginDTO_ShouldBeValid_WithCorrectData()
        {
            var dto = new LoginDTO("user@email.com", "Password123!");

            var results = ValidateModel(dto);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Email_IsRequired(string? invalidEmail)
        {
            var dto = new LoginDTO(invalidEmail!, "Password123!");

            var results = ValidateModel(dto);

            results.Should().Contain(r => r.ErrorMessage == "Email is required!");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Password_IsRequired(string? invalidPassword)
        {
            var dto = new LoginDTO("user@email.com", invalidPassword!);

            var results = ValidateModel(dto);

            results.Should().Contain(r => r.ErrorMessage == "Password is required!");
        }
    }
}
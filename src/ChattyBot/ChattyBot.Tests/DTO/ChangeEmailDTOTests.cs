using System.ComponentModel.DataAnnotations;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;

namespace ChattyBot.Tests.DTO
{
    public class ChangeEmailDTOTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("user.name+bot@tucluj.ro")]
        [InlineData("user123@subdomain.example.org")]
        public void ChangeEmailDTO_ShouldBeValid_WithCorrectData(string validEmail)
        {
            var dto = new ChangeEmailDTO("password123", validEmail);

            var results = ValidateModel(dto);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NewEmail_IsRequired(string? email)
        {
            var dto = new ChangeEmailDTO("password123", email!);

            var results = ValidateModel(dto);

            results.Should().Contain(r => r.ErrorMessage == "New email address is required!");
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("user@domain")]
        [InlineData("@domain.com")]
        [InlineData("user@domain..com")]
        public void NewEmail_Regex_ShouldRejectInvalidFormats(string email)
        {
            var dto = new ChangeEmailDTO("password123", email);

            var results = ValidateModel(dto);

            results.Should().Contain(r => r.ErrorMessage == "Please enter a valid email address! (ex: user@domain.com)");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CurrentPassword_IsRequired(string? password)
        {
            var dto = new ChangeEmailDTO(password!, "user@example.com");

            var results = ValidateModel(dto);

            results.Should().Contain(r => r.ErrorMessage == "Current password is required!");
        }
    }
}
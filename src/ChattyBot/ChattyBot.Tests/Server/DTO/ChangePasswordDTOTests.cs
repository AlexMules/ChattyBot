using System.ComponentModel.DataAnnotations;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;

namespace ChattyBot.Tests.Server.DTO
{
    public class ChangePasswordDTOTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void ChangePasswordDTO_ShouldBeValid_WithStrongPassword()
        {
            var dto = new ChangePasswordDTO("OldPass123!", "NewStrongPass123!");

            var results = ValidateModel(dto);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CurrentPassword_IsRequired(string? password)
        {
            var dto = new ChangePasswordDTO(password!, "ValidPass123!");

            var results = ValidateModel(dto);

            results.Should().Contain(r => r.ErrorMessage == "Current password is required!");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NewPassword_IsRequired(string? password)
        {
            var dto = new ChangePasswordDTO("OldPass123!", password!);

            var results = ValidateModel(dto);

            results.Should().Contain(r => r.ErrorMessage == "New password is required!");
        }

        [Fact]
        public void NewPassword_ShouldBeAtLeast8Characters()
        {
            var dto = new ChangePasswordDTO("OldPass123!", "Ab1!def");

            var results = ValidateModel(dto);

            results.Should().Contain(r => r.ErrorMessage == "New password must be at least 8 characters long!");
        }

        [Theory]
        [InlineData("password123!", "Missing uppercase")]
        [InlineData("PASSWORD123!", "Missing lowercase")]
        [InlineData("Password!", "Missing digit")]
        [InlineData("Password123", "Missing special character")]
        public void NewPassword_Regex_ShouldRejectWeakPasswords(string weakPassword, string reason)
        {
            var dto = new ChangePasswordDTO("OldPass123!", weakPassword);

            var results = ValidateModel(dto);

            results.Should().Contain(r => r.ErrorMessage == "New password must contain at least one uppercase letter, one digit, and one special character!",
                because: $"it failed due to: {reason}");
        }
    }
}
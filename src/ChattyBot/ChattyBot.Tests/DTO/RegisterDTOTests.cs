using System.ComponentModel.DataAnnotations;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;

namespace ChattyBot.Tests.DTO
{
    public class RegisterDTOTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void RegisterDTO_ShouldBeValid_WithCorrectData()
        {
            var dto = new RegisterDTO(
                "user@tucluj.ro",
                "user_name",
                "StrongPass123!",
                "avatar2.png");

            var results = ValidateModel(dto);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Email_IsRequired(string? invalidEmail)
        {
            var dto = new RegisterDTO(invalidEmail!, "user", "Pass123!");
            var results = ValidateModel(dto);
            results.Should().Contain(r => r.ErrorMessage == "Email field is mandatory!");
        }

        [Theory]
        [InlineData("plainaddress")]
        [InlineData("#@%^%#$@#$@#.com")]
        [InlineData("@example.com")]
        [InlineData("user@domain..com")] 
        public void Email_Regex_ShouldRejectInvalidFormats(string invalidEmail)
        {
            var dto = new RegisterDTO(invalidEmail, "user", "Pass123!");
            var results = ValidateModel(dto);
            results.Should().Contain(r => r.ErrorMessage == "Please enter a valid email address! (ex: user@domain.com)");
        }

        [Fact]
        public void Password_IsRequired()
        {
            var dto = new RegisterDTO("test@email.com", "user", "");
            var results = ValidateModel(dto);
            results.Should().Contain(r => r.ErrorMessage == "Password field is mandatory!");
        }

        [Fact]
        public void Password_ShouldBeAtLeast8Characters()
        {
            var dto = new RegisterDTO("test@email.com", "user", "Ab1!");
            var results = ValidateModel(dto);
            results.Should().Contain(r => r.ErrorMessage == "Password must be at least 8 characters long!");
        }

        [Theory]
        [InlineData("password123!", "Missing uppercase")]
        [InlineData("PASSWORD123!", "Missing lowercase")]
        [InlineData("Password!", "Missing digit")]
        [InlineData("Password123", "Missing special character")]
        public void Password_Regex_ShouldRejectWeakPasswords(string weakPassword, string reason)
        {
            var dto = new RegisterDTO("test@email.com", "user", weakPassword);
            var results = ValidateModel(dto);
            results.Should().Contain(r => r.ErrorMessage == "Password must contain at least one uppercase letter, one digit, and one special character!",
                because: reason);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Username_IsRequired(string? invalidUsername)
        {
            var dto = new RegisterDTO("test@email.com", invalidUsername!, "Pass123!");
            var results = ValidateModel(dto);
            results.Should().Contain(r => r.ErrorMessage == "Username field is mandatory!");
        }

        [Theory]
        [InlineData("user-name")] 
        [InlineData("user name")]
        [InlineData("user@")]     
        public void Username_Regex_ShouldRejectInvalidCharacters(string invalidUsername)
        {
            var dto = new RegisterDTO("test@email.com", invalidUsername, "Pass123!");
            var results = ValidateModel(dto);
            results.Should().Contain(r => r.ErrorMessage == "Username can only contain letters, numbers and underscores!");
        }

        [Fact]
        public void AvatarPath_ShouldHaveDefaultValue_WhenNotProvided()
        {
            var dto = new RegisterDTO(); 

            dto.AvatarPath.Should().Be("avatar1.png");
        }
    }
}
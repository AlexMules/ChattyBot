using System.ComponentModel.DataAnnotations;
using ChattyBot.Shared.Contracts.DTO;
using FluentAssertions;

namespace ChattyBot.Tests.Server.DTO
{
    public class ChangeUsernameDTOTests
    {
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        [Theory]
        [InlineData("user_name")]
        [InlineData("User123")]
        [InlineData("bot_99")]
        public void ChangeUsernameDTO_ShouldBeValid_WithCorrectData(string validUsername)
        {
            var dto = new ChangeUsernameDTO(validUsername);

            var results = ValidateModel(dto);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void NewUsername_IsRequired(string? username)
        {
            var dto = new ChangeUsernameDTO(username!);

            var results = ValidateModel(dto);

            results.Should().Contain(r => r.ErrorMessage == "New username is required!");
        }

        [Theory]
        [InlineData("user.name")]
        [InlineData("user-name")] 
        [InlineData("user!")]     
        [InlineData("user name")]
        public void NewUsername_Regex_ShouldRejectInvalidCharacters(string invalidUsername)
        {
            var dto = new ChangeUsernameDTO(invalidUsername);

            var results = ValidateModel(dto);

            results.Should().Contain(r => r.ErrorMessage == "Username can only contain letters, numbers and underscores!");
        }
    }
}
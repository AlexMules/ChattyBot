using ChattyBot.Server.Application.BotEngine.Utils;
using ChattyBot.Shared.Contracts.Enums;
using System.Text.RegularExpressions;

namespace ChattyBot.Server.Application.BotEngine.Commands
{
    public class CalcCommand : IBotCommand
    {
        public string CommandTrigger => "/calc";
        public string Description => "Calculates math expressions (+, -, *, /). Supports integers and decimals.";
        private static readonly Regex InvalidCharsRegex = new(@"[^0-9+\-*/().\s,]", RegexOptions.Compiled);
        private static readonly Regex InvalidStructureRegex = new(@"[\+\-\*/]{2,}|^[+\*/]|[\+\-\*/]$|\(\)", RegexOptions.Compiled);

        public async Task<BotResponse> ExecuteAsync(string? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(parameters))
            {
                return new BotResponse("I need a math expression! Example: /calc (10+5)*2", MessageType.Text);
            }

            if (InvalidCharsRegex.IsMatch(parameters))
            {
                return new BotResponse("That's not a valid math expression! Use numbers, parentheses, and operators (+, -, *, /).", MessageType.Text);
            }

            string expression = parameters.Replace(",", ".");
            expression = expression.Replace(" ", "");

            if (InvalidStructureRegex.IsMatch(expression))
            {
                return new BotResponse("Error: That looks like an invalid expression structure! Check your operators.", MessageType.Text);
            }

            string result = MathEngine.Compute(expression);

            string finalMessage = result switch
            {
                "DIV_ZERO" => "Error: Division by zero is not allowed!",
                "MATH_ERR" => "Error: That looks like an invalid expression!",
                _ => $"Result: {result}"
            };

            return new BotResponse(finalMessage, MessageType.Text);
        }
    }
}

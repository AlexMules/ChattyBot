using ChattyBot.Server.Application.BotEngine.Utils;
using FluentAssertions;

namespace ChattyBot.Tests.Utils
{
    public class RandomGeneratorTests
    {
        [Fact]
        public void GetNext_ShouldAlwaysStayWithinRange()
        {
            int min = 1;
            int max = 10;
            int iterations = 1000;


            for (int i = 0; i < iterations; i++)
            {
                int result = RandomGenerator.GetNext(min, max);
                result.Should().BeInRange(min, max, "because the generator should respect the boundaries");
            }
        }

        [Fact]
        public void GetNext_ShouldBeInclusiveOfMax()
        {
            int min = 1;
            int max = 2; 
            bool hitMax = false;
            int maxAttempts = 100;

            for (int i = 0; i < maxAttempts; i++)
            {
                if (RandomGenerator.GetNext(min, max) == max)
                {
                    hitMax = true;
                    break;
                }
            }

            hitMax.Should().BeTrue("because the 'max' value should be inclusive due to the +1 logic");
        }

        [Fact]
        public void GetNext_ShouldReturnMin_WhenMinAndMaxAreEqual()
        {
            int min = 5;
            int max = 5;

            int result = RandomGenerator.GetNext(min, max);

            result.Should().Be(5);
        }

        [Fact]
        public void GetNext_ShouldThrowException_WhenMinIsGreaterThanMax()
        {
            int min = 10;
            int max = 5;

            Action act = () => RandomGenerator.GetNext(min, max);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
using Xunit;

namespace UiPath.ChatbotSamples.BotFramework.Utils.Test
{
    public class OrdinalEqualTest
    {
        [Fact]
        public void EqualWhenSame()
        {
            var testString = "test string";
            Assert.True(testString.OrdinalEquals(testString));
        }

        [Fact]
        public void EqualWhenCaseDifferent()
        {
            var testString1 = "test string";
            var testString2 = "Test String";
            Assert.True(testString1.OrdinalEquals(testString2));
        }

        [Fact]
        public void NotEqualWhenDifferent()
        {
            var testString1 = "test string 1";
            var testString2 = "Test String 2";
            Assert.False(testString1.OrdinalEquals(testString2));
        }
    }
}

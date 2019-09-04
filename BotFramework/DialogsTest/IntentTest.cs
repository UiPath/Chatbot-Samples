using System.Linq;
using UiPath.ChatbotSamples.BotFramework.Dialogs;
using Xunit;

namespace UiPath.ChatbotSamples.BotFramework.DialogsTest
{
    public class IntentTest
    {
        private readonly MyIntents intents = new MyIntents();

        [Fact]
        public void DefaultIntentShouldBeDefined()
        {
            Assert.NotNull(intents.DefaultIntent);
        }

        [Fact]
        public void FallbackActivityShouldBeDefined()
        {
            Assert.NotNull(intents.FallbackActivity);
        }

        [Fact]
        public void WelcomActivityShouldBeDefined()
        {
            Assert.NotNull(intents.WelcomeActivity);
        }

        [Fact]
        public void ShouldHaveAtLeastOneIntent()
        {
            Assert.NotNull(intents.Intents);
            Assert.True(intents.Intents.Keys.ToList().Count > 1);
        }
    }
}

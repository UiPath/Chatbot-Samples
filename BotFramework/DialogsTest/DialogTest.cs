using System.Linq;
using System.Reflection;
using UiPath.ChatbotSamples.BotFramework.Dialogs;
using Xunit;

namespace UiPath.ChatbotSamples.BotFramework.DialogsTest
{
    public class DialogTest
    {
        [Fact]
        public void ShouldHaveAtLeastOneDialog()
        {
            string name_space = "UiPath.ChatbotSamples.BotFramework.Dialogs.SapDialog";
            var q = from t in Assembly.GetAssembly(typeof(SapBot)).GetTypes()
                    where t.IsClass && t.Namespace == name_space && !t.IsNested
                    select t;
            var result = q.ToList();
            Assert.True(result.Count >= 1);
        }
    }
}

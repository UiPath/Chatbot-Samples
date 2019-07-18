using System;
using Xunit;

namespace UiPath.ChatbotSamples.BotFramework.Utils.Test
{
    public class CheckNullReferenceTest
    {
        [Fact]
        public void ReturnNullWhenNull()
        {
            TestType t = null;
            Assert.Throws<ArgumentNullException>(() => t.CheckNullReference());
        }

        [Fact]
        public void ReturnValueWhenNotNull()
        {
            TestType origin = new TestType()
            {
                Test = "test string",
            };
            var result = origin.CheckNullReference();
            Assert.Equal(origin, result);
            Assert.Equal(origin.Test, result.Test);
        }
    }
}

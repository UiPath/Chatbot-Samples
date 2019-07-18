using System;

namespace UiPath.ChatbotSamples.BotFramework.Utils
{
    public static class CommonExtensions
    {
        public static T CheckNullReference<T>(this T t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }

            return t;
        }

        public static bool OrdinalEquals(this string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }
    }
}

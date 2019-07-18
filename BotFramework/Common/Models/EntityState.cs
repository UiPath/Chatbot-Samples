using System.Collections.Generic;

namespace UiPath.ChatbotSamples.BotFramework.Common.Models
{
    public class EntityState
    {
        public IDictionary<string, string> Entities { get; } = new Dictionary<string, string>();

        public void AddOrUpdate(string key, string value)
        {
            if (Entities.ContainsKey(key))
            {
                Entities[key] = value;
            }
            else
            {
                Entities.Add(key, value);
            }
        }

        public string TryGet(string key)
        {
            if (Entities.ContainsKey(key))
            {
                return Entities[key];
            }
            else
            {
                return string.Empty;
            }
        }
    }
}

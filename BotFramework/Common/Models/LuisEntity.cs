using System;
using System.Collections.Generic;

namespace UiPath.ChatbotSamples.BotFramework.Common.Models
{
    public class LuisEntity
    {
        private Func<string, string> _postProcess;
        public LuisEntity(List<string> patterns, Func<string, string> postProcess = null)
        {
            Patterns.AddRange(patterns);
            _postProcess = postProcess;
        }

        // entity names in luis that map in bot entity
        public List<string> Patterns { get; } = new List<string>();

        // postproces the entity value and customize it for bot use
        public string PostProcess(string value)
        {
            if (_postProcess != null)
            {
                return _postProcess(value);
            }
            return value;
        }
    }
}

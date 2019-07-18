using System;

namespace UiPath.ChatbotSamples.BotFramework.Actions.Models
{
    public class Item
    {
        public string ItemId { get; set; }

        public string ItemName { get; set; }

        public string Quantity { get; set; }

        public string OrderId { get; set; }

        public DateTime DeliveryDate { get; set; }

        public string DeliveryStatus { get; set; }
    }
}

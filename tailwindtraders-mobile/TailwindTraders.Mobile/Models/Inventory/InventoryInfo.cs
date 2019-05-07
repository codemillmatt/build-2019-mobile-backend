namespace TailwindTraders.Mobile
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class InventoryInfo
    {
        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("modified")]
        public DateTimeOffset Modified { get; set; }
    }
}

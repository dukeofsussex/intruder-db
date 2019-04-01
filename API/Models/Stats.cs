// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using API.Utils.Converters;
    using Newtonsoft.Json;

    [JsonConverter(typeof(StatsConverter))]
    public class Stats : IStats
    {
        public string Identifier { get; set; }

        public StatsItem Item { get; set; }

        dynamic IStats.Item
        {
            get { return this.Item; }
            set { this.Item = value as StatsItem; }
        }
    }
}

// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System.ComponentModel;
    using Newtonsoft.Json;

    public class StatsItem
    {
        [DefaultValue(0)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int ID { get; set; }

        public string Name { get; set; }

        public float Value { get; set; }
    }
}

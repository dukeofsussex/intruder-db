// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System;
    using System.ComponentModel;
    using Newtonsoft.Json;

    public class Activity
    {
        [DefaultValue(0)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int ServerID { get; set; }

        [DefaultValue(0)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int MapID { get; set; }

        public int Agents { get; set; }

        public DateTime Timestamp { get; set; }
    }
}

// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System.ComponentModel;
    using Newtonsoft.Json;

    public class AgentBadge : Badge
    {
        [DefaultValue(0)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int AgentID { get; set; }

        public decimal Progress { get; set; }
    }
}

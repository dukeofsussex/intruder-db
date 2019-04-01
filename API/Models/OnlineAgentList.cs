// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class OnlineAgentList
    {
        public OnlineAgentList()
        {
            this.Agents = new List<OnlineAgent>();
        }

        // Only used when deserializing the list from SBG
        [JsonProperty("players")]
        public List<OnlineAgent> Agents { get; private set; }
    }
}

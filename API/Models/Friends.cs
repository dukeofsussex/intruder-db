// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Friends
    {
        public Friends()
        {
            this.List = new List<AgentProfile>();
        }

        [JsonProperty("friends")]
        public List<AgentProfile> List { get; private set; }
    }
}

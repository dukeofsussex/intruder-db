// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ServerList
    {
        public ServerList()
        {
            this.Servers = new List<Server>();
        }

        // Only used when deserializing the list from SBG
        [JsonProperty("rooms")]
        public List<Server> Servers { get; private set; }
    }
}

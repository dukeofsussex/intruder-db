// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using Newtonsoft.Json;

    public class AgentServerProfile : AgentProfile
    {
        [JsonIgnore]
        public string ServerUUID { get; set; }
    }
}

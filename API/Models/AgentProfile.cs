// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class AgentProfile
    {
        public int ID { get; set; }

        public long SteamID { get; set; }

        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AgentRole Role { get; set; }

        public Uri AvatarURL { get; set; }
    }
}

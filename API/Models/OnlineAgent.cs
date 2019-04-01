// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using API.Utils;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class OnlineAgent
    {
        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData { get; set; }

        [JsonInput("player")]
        public string Name { get; set; }

        [JsonInput("servername")]
        public string CurrentLocation { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (string.IsNullOrEmpty(this.CurrentLocation))
            {
                this.CurrentLocation = this.AdditionalData["levelname"].ToString();
            }
        }
    }
}

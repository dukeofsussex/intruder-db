// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class Tuning
    {
        public Tuning()
        {
            this.Ratings = new List<int>();
        }

        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("author")]
        public AgentProfile Author { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("averageRating")]
        public decimal AverageRating { get; set; }

        [JsonProperty("settings")]
        public JRaw Settings { get; set; }

        [JsonProperty("share")]
        public bool Share { get; set; }

        [JsonProperty("lastUpdate")]
        public DateTime LastUpdate { get; set; }

        [JsonProperty("ratings")]
        public List<int> Ratings { get; private set; }
    }
}

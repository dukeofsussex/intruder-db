// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

#pragma warning disable CA1707 // Identifiers should not contain underscores
    public enum MapStage
    {
        None = 0,
        Official_Release = 1,
        Featured_Release = 2,
        Featured_Update = 3,
        Polished_Release = 4,
        Polished_Update = 5,
        Playable_Release = 6,
        Playable_Update = 7,
        Training = 8,
        Silly_Other = 9,
        AUG_Only = 10,
        In_Dev = 11,
        Unknown = 12
    }
#pragma warning restore CA1707 // Identifiers should not contain underscores

    public class Map
    {
        public Map()
        {
            this.Ratings = new List<int>();
        }

        public int ID { get; set; }

        public string UID { get; set; }

        public string Name { get; set; }

        public AgentProfile Author { get; set; }

        [JsonIgnore]
        public Uri URL { get; set; }

        public int Version { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MapStage Stage { get; set; }

        public decimal AverageRating { get; set; }

        public int Played { get; set; }

        public bool HasFloorplan { get; set; }

        public int Images { get; set; }

        public DateTime LastUpdate { get; set; }

        public List<int> Ratings { get; private set; }
    }
}

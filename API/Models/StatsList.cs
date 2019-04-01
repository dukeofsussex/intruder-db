// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System.Collections.Generic;
    using API.Utils.Converters;
    using Newtonsoft.Json;

    [JsonConverter(typeof(StatsListConverter))]
    public class StatsList
    {
        public StatsList()
        {
            this.Stats = new List<IStats>();
        }

        public List<IStats> Stats { get; private set; }
    }
}

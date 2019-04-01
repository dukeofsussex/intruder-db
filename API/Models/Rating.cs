// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public enum RatingType
    {
        None = 0,
        Map = 1,
        Tuning = 2
    }

    public class Rating
    {
        public int AgentID { get; set; }

        public int TypeID { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RatingType Type { get; set; }

        public int Value { get; set; }
    }
}

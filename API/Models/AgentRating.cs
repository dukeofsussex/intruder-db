// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public enum AgentRatingType
    {
        General = 0,
        Communication = 1,
        Niceness = 2,
        Teamplayer = 3,
        Fairness = 4
    }

    public class AgentRating
    {
        public int AgentID { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
#pragma warning disable CA1721 // Property names should not match get methods
        public AgentRatingType Type { get; set; }
#pragma warning restore CA1721 // Property names should not match get methods

        public int Positive { get; set; }

        public int Negative { get; set; }
    }
}

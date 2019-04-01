// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public enum AgentRole
    {
        None = 0,
        Demoted = 1,
        Agent = 2,
        AUG = 3,
        Dev = 4
    }

#pragma warning disable CA1717 // Only FlagsAttribute enums should have plural names
    public enum Status
#pragma warning restore CA1717 // Only FlagsAttribute enums should have plural names
    {
        None = 0,
        Offline = 1,
        Online = 2
    }

    public class Agent
    {
        public Agent()
        {
            this.Ratings = new List<AgentRating>();
        }

        public int ID { get; set; }

        public long SteamID { get; set; }

        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AgentRole Role { get; set; }

        public Uri AvatarURL { get; set; }

        public int XP { get; set; }

        public int TimePlayed { get; set; }

        public int MatchesPlayed { get; set; }

        public int MatchesWon { get; set; }

        public int MatchesLost { get; set; }

        public int MatchesTied { get; set; }

        public int MatchesSurvived { get; set; }

        public int Arrests { get; set; }

        public int Captures { get; set; }

        public decimal WinRate { get; set; }

        public decimal SurvivalRate { get; set; }

        public int TimePerMatch { get; set; }

        public float ArrestsPerMatch { get; set; }

        public float CapturesPerMatch { get; set; }

        public int XPPerMatch { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Status Status { get; set; }

        public Location CurrentLocation { get; set; }

        public DateTime LastUpdate { get; set; }

        public DateTime LastSeen { get; set; }

        public DateTime Registered { get; set; }

        public bool Flagged { get; set; }

        public List<AgentRating> Ratings { get; }
    }
}

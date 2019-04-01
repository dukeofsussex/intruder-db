// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Models
{
    using System;
    using System.Collections.Generic;
    using MySql.Data.MySqlClient;
    using Newtonsoft.Json.Linq;

    public static class Create
    {
        public static Activity Activity(MySqlDataReader reader) => new Activity()
        {
            ServerID = reader.GetInt32("server_id"),
            Agents = reader.GetInt32("agents"),
            Timestamp = reader.GetDateTime("timestamp")
        };

        public static Agent Agent(MySqlDataReader reader, bool useTablePrefix = false) => new Agent()
        {
            ID = reader.GetInt32(useTablePrefix ? "agent_id" : "id"),
            SteamID = reader.GetInt64("steam_id"),
            Name = reader.GetString("name"),
            Role = (AgentRole)Enum.Parse(typeof(AgentRole), reader.GetString("role")),
            AvatarURL = new Uri(reader.GetString("avatar_url")),
            XP = reader.GetInt32("xp"),
            XPPerMatch = reader.GetInt32("xp_per_match"),
            TimePlayed = reader.GetInt32("time_played"),
            MatchesPlayed = reader.GetInt32("matches_played"),
            MatchesWon = reader.GetInt32("matches_won"),
            MatchesLost = reader.GetInt32("matches_lost"),
            MatchesTied = reader.GetInt32("matches_tied"),
            MatchesSurvived = reader.GetInt32("matches_survived"),
            Arrests = reader.GetInt32("arrests"),
            WinRate = reader.GetDecimal("win_rate"),
            SurvivalRate = reader.GetDecimal("survival_rate"),
            TimePerMatch = reader.GetInt32("time_per_match"),
            ArrestsPerMatch = reader.GetFloat("arrests_per_match"),
            CapturesPerMatch = reader.GetFloat("captures_per_match"),
            Captures = reader.GetInt32("captures"),
            Status = (Status)Enum.Parse(typeof(Status), reader.GetString("status")),
            CurrentLocation = Location(reader),
            LastUpdate = reader.GetDateTime("last_update"),
            LastSeen = reader.GetDateTime("last_seen"),
            Registered = reader.GetDateTime("registered"),
            Flagged = reader.GetBoolean("flagged")
        };

        public static AgentBadge AgentBadge(MySqlDataReader reader) => new AgentBadge()
        {
            ID = reader.GetInt32("id"),
            Title = reader.GetString("title"),
            Description = reader.GetString("description"),
            AgentID = reader.GetInt32("agent_id"),
            Progress = reader.GetDecimal("progress")
        };

        public static AgentJobDetails AgentJobDetails(MySqlDataReader reader) => new AgentJobDetails()
        {
            ID = reader.GetInt32("id"),
            Name = reader.GetString("name"),
            Flagged = reader.GetBoolean("flagged")
        };

        public static AgentProfile AgentProfile(MySqlDataReader reader) => new AgentProfile()
        {
            ID = reader.GetInt32("profile_id"),
            SteamID = reader.GetInt64("profile_steam_id"),
            Name = reader.GetString("profile_name"),
            Role = (AgentRole)Enum.Parse(typeof(AgentRole), reader.GetString("profile_role")),
            AvatarURL = new Uri(reader.GetString("profile_avatar_url"))
        };

        public static AgentServerProfile AgentServerProfile(MySqlDataReader reader) => new AgentServerProfile()
        {
            ID = reader.GetInt32("profile_id"),
            SteamID = reader.GetInt64("profile_steam_id"),
            Name = reader.GetString("profile_name"),
            Role = (AgentRole)Enum.Parse(typeof(AgentRole), reader.GetString("profile_role")),
            AvatarURL = new Uri(reader.GetString("profile_avatar_url")),
            ServerUUID = reader.GetString("server_uuid")
        };

        public static AgentRating AgentRating(MySqlDataReader reader) => new AgentRating()
        {
            AgentID = reader.GetInt32("agent_id"),
            Type = (AgentRatingType)Enum.Parse(typeof(AgentRatingType), reader.GetString("type")),
            Positive = reader.GetInt32("positive"),
            Negative = reader.GetInt32("negative")
        };

        public static Badge Badge(MySqlDataReader reader) => new Badge()
        {
            ID = reader.GetInt32("id"),
            Title = reader.GetString("title"),
            Description = reader.GetString("description")
        };

        public static bool Bool(MySqlDataReader reader, string columnName) => reader.GetBoolean(columnName);

        public static Claim Claim(MySqlDataReader reader) => new Claim()
        {
            Claimer = AgentProfile(reader),
            UID = reader.GetGuid("claim_uid"),
            Sent = reader.GetBoolean("claim_sent"),
            Timestamp = reader.GetDateTime("claim_timestamp")
        };

        public static int IDOnly(MySqlDataReader reader) => reader.GetInt32("id");

        public static StorageObject StorageObject(MySqlDataReader reader) => new StorageObject()
        {
            Name = reader.GetString("Name"),
            Value = reader.GetInt32("Value")
        };

        public static Location Location(MySqlDataReader reader) => new Location()
        {
            ServerID = reader.GetInt32("location_server_id"),
            Description = reader.GetString("location_description")
        };

        public static Map Map(MySqlDataReader reader, bool useTablePrefix = false)
        {
            Map map = new Map()
            {
                ID = reader.GetInt32(useTablePrefix ? "map_id" : "id"),
                UID = reader.GetString(useTablePrefix ? "map_uid" : "uid"),
                Name = reader.GetString(useTablePrefix ? "map_name" : "name"),
                Author = AgentProfile(reader),
                URL = new Uri(reader.GetString("url")),
                Version = reader.GetInt32(useTablePrefix ? "map_version" : "version"),
                Stage = (MapStage)Enum.Parse(typeof(MapStage), reader.GetString("stage")),
                AverageRating = reader.GetDecimal("average_rating"),
                Images = reader.GetInt16("images"),
                Played = reader.GetInt32("played"),
                HasFloorplan = reader.GetBoolean("has_floorplan"),
                LastUpdate = reader.GetDateTime(useTablePrefix ? "map_last_update" : "last_update")
            };

            map.Ratings.AddRange(new List<int>()
            {
                reader.GetInt32("rating_1"),
                reader.GetInt32("rating_2"),
                reader.GetInt32("rating_3"),
                reader.GetInt32("rating_4"),
                reader.GetInt32("rating_5")
            });

            return map;
        }

        public static MapActivity MapActivity(MySqlDataReader reader) => new MapActivity()
        {
            ID = reader.GetInt32("id"),
            Name = reader.GetString("name"),
            Count = reader.GetInt32("count")
        };

        public static Rating Rating(MySqlDataReader reader) => new Rating()
        {
            AgentID = reader.GetInt32("agent_id"),
            TypeID = reader.GetInt32("type_id"),
            Type = (RatingType)Enum.Parse(typeof(RatingType), reader.GetString("type")),
            Value = reader.GetInt32("rating")
        };

        public static Server Server(MySqlDataReader reader, bool useTablePrefix = false) => new Server()
        {
            ID = reader.GetInt32(useTablePrefix ? "server_id" : "id"),
            UUID = reader.GetString("uuid"),
            Name = reader.GetString("name"),
            FancyName = reader.GetString("fancy_name"),
            Description = reader.GetString("description"),
            Region = reader.GetString("region"),
            Type = reader.GetString("type"),
            Style = reader.GetString("style"),
            Version = reader.GetInt32("version"),
            Map = Map(reader, true),
            MapType = reader.GetString("map_type"),
            Agents = reader.GetInt16("agents"),
            MaxAgents = reader.GetInt16("max_agents"),
            MaxSpectators = reader.GetInt16("max_spectators"),
            Gamemode = reader.GetString("gamemode"),
            Timemode = reader.GetString("timemode"),
            Time = reader.GetInt32("time"),
            InProgress = reader.GetBoolean("in_progress"),
            Ranked = reader.GetBoolean("ranked"),
            JoinableBy = (JoinableBy)Enum.Parse(typeof(JoinableBy), reader.GetString("joinable_by")),
            VisibleBy = (VisibleBy)Enum.Parse(typeof(VisibleBy), reader.GetString("visible_by")),
            Passworded = reader.GetBoolean("passworded"),
            MasterAgent = reader.GetString("master_agent"),
            AgentNames = reader.GetString("agent_names"),
            MasterIP = reader.GetString("master_ip"),
            ServerIP = reader.GetString("server_ip"),
            UpdateIP = reader.GetString("update_ip"),
            LastUpdate = reader.GetDateTime("last_update"),
            Rules = new JRaw(reader.GetString("rules")),
            Tuning = new JRaw(reader.GetString("tuning"))
        };

        public static Stats Stats(MySqlDataReader reader) => new Stats()
        {
            Identifier = reader.GetString("identifier"),
            Item = new StatsItem()
            {
                ID = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Value = reader.IsDBNull(reader.GetOrdinal("value")) ? 0 : reader.GetFloat("value")
            }
        };

        public static Tuning Tuning(MySqlDataReader reader)
        {
            Tuning tuning = new Tuning()
            {
                ID = reader.GetInt32("tuning_id"),
                Author = AgentProfile(reader),
                Name = reader.GetString("tuning_name"),
                Description = reader.GetString("tuning_description"),
                AverageRating = reader.GetDecimal("average_rating"),
                Settings = new JRaw(reader.GetString("tuning_settings")),
                Share = reader.GetBoolean("tuning_share"),
                LastUpdate = reader.GetDateTime("tuning_last_update"),
            };

            tuning.Ratings.AddRange(new List<int>()
            {
                reader.GetInt32("rating_1"),
                reader.GetInt32("rating_2"),
                reader.GetInt32("rating_3"),
                reader.GetInt32("rating_4"),
                reader.GetInt32("rating_5")
            });

            return tuning;
        }
    }
}

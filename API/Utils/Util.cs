// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using API.Models;
    using API.Services;
    using MySql.Data.MySqlClient;
    using Newtonsoft.Json.Linq;
    using NLog;

    public static class Util
    {
        private static readonly Dictionary<string, AgentRole> RoleMappings = new Dictionary<string, AgentRole>()
        {
            { "Demoted", AgentRole.Demoted },
            { "Agent",  AgentRole.Agent },
            { "Advanced User", AgentRole.AUG },
            { "Developer", AgentRole.Dev }
        };

        private static readonly Dictionary<int, Tuple<string, dynamic>> BadgeMapping = new Dictionary<int, Tuple<string, dynamic>>()
        {
            { 1, new Tuple<string, dynamic>("XP", 1000) },
            { 2, new Tuple<string, dynamic>("XP", 5000) },
            { 3, new Tuple<string, dynamic>("XP", 10000) },
            { 4, new Tuple<string, dynamic>("XP", 50000) },
            { 5, new Tuple<string, dynamic>("TimePlayed", 7560) },
            { 6, new Tuple<string, dynamic>("TimePlayed", 86400) },
            { 7, new Tuple<string, dynamic>("TimePlayed", 604800) },
            { 8, new Tuple<string, dynamic>("TimePlayed", 756000) },
            { 9, new Tuple<string, dynamic>("MatchesPlayed", 1) },
            { 10, new Tuple<string, dynamic>("MatchesPlayed", 100) },
            { 11, new Tuple<string, dynamic>("MatchesPlayed", 500) },
            { 12, new Tuple<string, dynamic>("MatchesPlayed", 1000) },
            { 13, new Tuple<string, dynamic>("MatchesPlayed", 5000) },
            { 14, new Tuple<string, dynamic>("MatchesTied", 1) },
            { 15, new Tuple<string, dynamic>("MatchesTied", 50) },
            { 16, new Tuple<string, dynamic>("MatchesTied", 100) },
            { 17, new Tuple<string, dynamic>("Arrests", 1) },
            { 18, new Tuple<string, dynamic>("Arrests", 100) },
            { 19, new Tuple<string, dynamic>("Arrests", 500) },
            { 20, new Tuple<string, dynamic>("Arrests", 1000) },
            { 21, new Tuple<string, dynamic>("Captures", 1) },
            { 22, new Tuple<string, dynamic>("Captures", 100) },
            { 23, new Tuple<string, dynamic>("Captures", 500) },
            { 24, new Tuple<string, dynamic>("Captures", 1000) },
            { 25, new Tuple<string, dynamic>("MatchesSurvived", 1) },
            { 26, new Tuple<string, dynamic>("MatchesSurvived", 100) },
            { 27, new Tuple<string, dynamic>("MatchesSurvived", 500) },
            { 28, new Tuple<string, dynamic>("MatchesSurvived", 1000) },
            { 29, new Tuple<string, dynamic>("Ratings", 1) },
            { 30, new Tuple<string, dynamic>("Ratings", 100) },
            { 31, new Tuple<string, dynamic>("Ratings", 500) },
            { 32, new Tuple<string, dynamic>("Registered", DateTime.Now) }
        };

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static decimal CalculateBadgeProgress(Agent agent, int badgeID)
        {
            if (!BadgeMapping.Keys.Contains(badgeID))
            {
                logger.Error($"Badge mapping doesn't exist for badge id: {badgeID}");
                return 0;
            }

            dynamic agentValue;
            dynamic requiredValue = BadgeMapping[badgeID].Item2;
            decimal progress = 0;

            if (BadgeMapping[badgeID].Item1 == "Ratings")
            {
                AgentRating rating = agent.Ratings.Where(r => r.Type == AgentRatingType.General).FirstOrDefault();

                // Occurs if the rating list hasn't got the necessary rating
                if (rating == null)
                {
                    return 0;
                }

                agentValue = rating.Positive - rating.Negative;
            }
            else
            {
                agentValue = agent.GetType().GetProperty(BadgeMapping[badgeID].Item1).GetValue(agent);
            }

            if (requiredValue.GetType() == typeof(DateTime))
            {
                progress = agentValue <= requiredValue ? 1 : 0;
            }
            else
            {
                progress = Math.Round((decimal)agentValue / (decimal)requiredValue, 2);
            }

            return progress > 1 ? 1 : progress;
        }

        public static int GetMapAuthorID(MapUID mapUID)
        {
            int authorID = 0;

            if (mapUID.Author == "Official")
            {
                authorID = 1; // Hand over official maps to Rob
            }
            else
            {
                authorID = DB.Get(
                    "SELECT id FROM agent WHERE name = @name",
                    new Dictionary<string, dynamic>()
                    {
                            { "@name", mapUID.Author }
                    },
                    Create.IDOnly);
            }

            return authorID;
        }

        public static MapUID GetSeparatedUID(string uid)
        {
            string[] mapDetails = uid.Split('@');

            // Official maps haven't got an author
            if (mapDetails.Length == 1)
            {
                mapDetails = new string[] { "Official", uid };
            }

            return new MapUID()
            {
                Author = mapDetails[0],
                Name = mapDetails[1]
            };
        }

        public static AgentRole GetConvertedRole(string forumGroup)
        {
            AgentRole role = AgentRole.Agent;

            if (RoleMappings.ContainsKey(forumGroup))
            {
                role = RoleMappings[forumGroup];
            }

            return role;
        }

        public static List<JToken> GetUpdatedMapCycle(JEnumerable<JToken> maps)
        {
            Dictionary<string, Map> dbMaps = DB.GetList("SELECT id, uid, name FROM map", null, CreateMap)
                .SelectMany(m => m)
                .ToDictionary(d => d.Key.ToUpperInvariant(), d => d.Value);

            List<JToken> convertedMaps = new List<JToken>();

            foreach (string map in maps)
            {
                if (dbMaps.ContainsKey(map.ToUpperInvariant()))
                {
                    convertedMaps.Add(JToken.Parse($"{{ \"id\": {dbMaps[map.ToUpperInvariant()].ID}, \"name\": \"{dbMaps[map.ToUpperInvariant()].Name}\" }}"));
                }
                else
                {
                    MapUID separatedUID = GetSeparatedUID(map);

                    int authorID = GetMapAuthorID(separatedUID);

                    int mapID = DB.Insert(
                        "INSERT INTO map(uid, name, author_id, url, version, stage, played, last_update)"
                            + $" VALUES ('{map}', '{separatedUID.Name}', '{authorID}', 'https://intruder-db.info/maps/not-found', 1, '{MapStage.Unknown.ToString()}', 0, NOW())",
                        null,
                        true);
                    convertedMaps.Add(JToken.Parse($"{{ \"id\": {mapID}, \"name\": \"{separatedUID.Name}\" }}"));
                }
            }

            return convertedMaps;
        }

        private static Dictionary<string, Map> CreateMap(MySqlDataReader reader)
        {
            return new Dictionary<string, Map>()
            {
                {
                    reader.GetString("uid"),
                    new Map
                    {
                        ID = reader.GetInt32("id"),
                        Name = reader.GetString("name")
                    }
                }
            };
        }
    }
}

// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using API.Models;
    using API.Services;
    using Microsoft.AspNetCore.Mvc;

    [Route("stats")]
    public class StatsController : Controller
    {
        private const string MedianQuery = "SELECT 'medianPlaytime' AS identifier, 0 AS id, 'medianPlaytime' AS name, ROUND(AVG(t1.time_played)) AS value FROM("
                + " SELECT @`rownum`:= @`rownum` + 1 as `row_number`, a.time_played "
                + " FROM agent a, (SELECT @`rownum`:= 0) r ORDER BY a.time_played"
            + ") as t1, ("
                + " SELECT count(*) as total_rows"
                + " FROM agent a"
            + ") as t2"
            + " WHERE t1.row_number in (floor((total_rows + 1) / 2), floor((total_rows + 2) / 2))";

        private static readonly Dictionary<string, string> AgentCategories = new Dictionary<string, string>()
        {
            { "xp", "xp" },
            { "timePlayed", "time_played" },
            { "matchesPlayed", "matches_played" },
            { "matchesWon", "matches_won" },
            { "matchesLost", "matches_lost" },
            { "matchesTied", "matches_played - matches_won - matches_lost" },
            { "matchesSurvived", "matches_survived" },
            { "arrests", "arrests" },
            { "captures", "captures" },
            { "winRate", "matches_won / matches_played" },
            { "survivalRate", "matches_survived / matches_played" },
            { "timePerMatch", "time_played / matches_played" },
            { "arrestsPerMatch", "arrests / matches_played" },
            { "capturesPerMatch", "captures / matches_played" },
            { "xpPerMatch", "xp / matches_played" }
        };

        private static readonly string[] MapCategories = { "mapper", "played", "rating" };

        private static readonly string[] DecimalCategories = { "winRate", "survivalRate", "arrestsPerMatch", "capturesPerMatch" };

        private static readonly string[] SupportedStats =
        {
            "trackedAgents",
            "gameVersion",
            "alltimePeak",
            "dayPeak",
            "currentlyPlaying",
            "averagePlaytime",
            "medianPlaytime",
            "dayUnique",
            "monthUnique"
        };

        private static readonly string[] SupportedTop = { "maps", "agents" };

        /// <summary>
        /// Retrieves all available stats
        /// </summary>
        /// <response code="200">Stats retrieved</response>
        /// <response code="500">Something broke internally</response>
        // GET: stats
        [HttpGet]
        [ProducesResponseType(typeof(Dictionary<string, StatsItem>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult Get()
        {
            return this.Ok(
                DB.GetList(
                        "SELECT identifier, id, name, value FROM base_stats"
                            + $" UNION {MedianQuery}",
                        null,
                        Create.Stats)
                    .ToDictionary(x => x.Identifier, y => y.Item));
        }

        /// <summary>
        /// Retrieves a single stats entry
        /// </summary>
        /// <remarks>
        /// Available stats groups:
        ///
        ///     trackedAgents,
        ///     gameVersion,
        ///     alltimePeak,
        ///     dayPeak,
        ///     currentlyPlaying,
        ///     averagePlaytime,
        ///     medianPlaytime
        /// </remarks>
        /// <param name="group">(Required) Stats group identifier</param>
        /// <response code="200">Stats retrieved</response>
        /// <response code="400">Invalid stats group</response>
        /// <response code="500">Something broke internally</response>
        // GET: stats/<group>
        [HttpGet("{group}")]
        [ProducesResponseType(typeof(StatsItem), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetSingle(string group)
        {
            if (!SupportedStats.Contains(group))
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = $"Stats group '{group}' doesn't exist"
                });
            }

            string queryString = string.Empty;
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();

            if (group == "medianPlaytime")
            {
                queryString = MedianQuery;
            }
            else
            {
                queryString = "SELECT identifier, id, name, value FROM base_stats WHERE name = @name";
                parameters.Add("@name", group);
            }

            return this.Ok(DB.Get(queryString, parameters, Create.Stats));
        }

        /// <summary>
        /// Retrieves agent averages
        /// </summary>
        /// <remarks>
        /// Available categories:
        ///
        ///     xp,
        ///     timePlayed,
        ///     matchesPlayed,
        ///     matchesWon,
        ///     matchesLost,
        ///     matchesTied,
        ///     matchesSurvived,
        ///     arrests,
        ///     captures,
        ///     winRate,
        ///     survivalRate,
        ///     timePerMatch,
        ///     arrestsPerMatch,
        ///     capturesPerMatch,
        ///     xpPerMatch,
        ///     ratings // Returns the best agents of every rating type or only of one type,
        ///             // depending on whether a type parameter is provided
        ///
        /// Available rating types:
        ///
        ///     // Check agent ratings for available values
        /// </remarks>
        /// <param name="category">(Optional) Subcategory for agents `(Default: empty)`</param>
        /// <param name="type">(Required/Optional) Specifies the rating type `(Default: empty)`</param>
        /// <response code="200">Stats retrieved</response>
        /// <response code="400">Invalid query parameters</response>
        /// <response code="500">Something broke internally</response>
        // GET: stats/averages
        [HttpGet("averages")]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetAverages([FromQuery]string category, [FromQuery]string type)
        {
            if (category != null && category != "ratings" && !AgentCategories.Keys.Contains(category))
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = $"Unknown stats category '{category}'"
                });
            }
            else if (category == "ratings" && type != null && !Enum.TryParse(type, true, out AgentRatingType ratingType))
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = $"Unknown rating type '{type}'"
                });
            }

            return this.GetAggregatedStats(category, type, "AVG");
        }

        /// <summary>
        /// Retrieves top stats for various groups
        /// </summary>
        /// <remarks>
        /// Available stats groups:
        ///
        ///     maps,
        ///     agents
        ///
        /// Available categories:
        ///
        ///     // Check averages for the available categories
        ///
        /// Available map categories:
        ///
        ///     mapper,
        ///     played,
        ///     rating
        ///
        /// Available rating types:
        ///
        ///     // Check agent ratings for available values
        /// </remarks>
        /// <param name="group">(Required) Stats group identifier</param>
        /// <param name="category">(R/O) Subcategory for agents `(Default: empty)`</param>
        /// <param name="type">(R/O) Specifies the rating type `(Default: empty)`</param>
        /// <response code="200">Stats retrieved</response>
        /// <response code="400">Invalid query parameters</response>
        /// <response code="500">Something broke internally</response>
        // GET: stats/top/<group>
        [HttpGet("top/{group}")]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetTop(string group, [FromQuery]string category, [FromQuery]string type)
        {
            if (!SupportedTop.Contains(group))
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = $"Unknown stats group '{group}'"
                });
            }
            else if (category != null && category != "ratings" && !AgentCategories.Keys.Contains(category) && !MapCategories.Contains(category))
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = $"Unknown stats category '{category}'"
                });
            }
            else if (category == "ratings" && type != null && !Enum.TryParse(type, true, out AgentRatingType ratingType))
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = $"Unknown rating type '{type}'"
                });
            }

            if (group == "maps")
            {
                if (category != null)
                {
                    return this.Ok(DB.Get(
                        $"SELECT identifier, id, name, value FROM top_map WHERE identifier = @category",
                        new Dictionary<string, dynamic>()
                        {
                            { "@category", category }
                        },
                        Create.Stats));
                }

                StatsList mapsStats = new StatsList();
                mapsStats.Stats.AddRange(DB.GetList("SELECT identifier, id, name, value FROM top_map", null, Create.Stats));

                return this.Ok(mapsStats);
            }

            return this.GetAggregatedStats(category, type, "MAX");
        }

        private IActionResult GetAggregatedStats(string category, string type, string aggregator)
        {
            StatsList statsList = new StatsList();

            string ratingTable = aggregator == "AVG" ? "average_rating" : "top_rating";
            string agentTable = aggregator == "AVG" ? "average_agent" : "top_agent";

            if (category == null)
            {
                statsList.Stats.AddRange(DB.GetList($"SELECT identifier, id, name, value FROM {agentTable}", null, Create.Stats));
            }

            if (category == null || (category == "ratings" && type == null))
            {
                StatsList ratings = new StatsList();
                ratings.Stats.AddRange(DB.GetList($"SELECT identifier, id, name, value FROM {ratingTable}", null, Create.Stats));

                statsList.Stats.Add(new NestedStats()
                {
                    Identifier = "ratings",
                    Item = ratings
                });
            }
            else if (category != "ratings")
            {
                int decimals = DecimalCategories.Contains(category) ? 2 : 0;
                string queryString = aggregator == "AVG"
                    ? $"SELECT @category AS identifier, 0 AS id, @category AS name, ROUND(AVG({AgentCategories[category]}), {decimals}) AS value FROM agent"
                    : $"SELECT @category AS identifier, agent.id AS id, name, ROUND({AgentCategories[category]}, {decimals}) AS value"
                        + $" FROM agent WHERE {AgentCategories[category]} = (SELECT MAX({AgentCategories[category]}) FROM agent)";

                return this.Ok(DB.Get(
                    queryString,
                    new Dictionary<string, dynamic>()
                    {
                        { "@category", category }
                    },
                    Create.Stats));
            }
            else
            {
                string queryString = aggregator == "AVG"
                    ? "SELECT @type AS identifier, 0 AS id, @type AS name, ROUND(AVG(positive - negative)) AS value FROM agent_rating WHERE type = @type"
                    : "SELECT @type AS identifier, agent_id AS id, agent.name AS name, (agent_rating.positive - agent_rating.negative) AS value"
                        + " FROM agent INNER JOIN agent_rating ON agent.id = agent_rating.agent_id"
                        + " WHERE agent_rating.positive - agent_rating.negative = (SELECT MAX(positive - negative) FROM agent_rating WHERE type = @type)"
                        + " AND type = @type";

                return this.Ok(DB.Get(
                    "SELECT @type AS identifier, 0 AS id, @type AS name, ROUND(AVG(positive - negative)) AS value FROM agent_rating WHERE type = @type",
                    new Dictionary<string, dynamic>()
                    {
                        { "@type", CultureInfo.InvariantCulture.TextInfo.ToTitleCase(type) }
                    },
                    Create.Stats));
            }

            return this.Ok(statsList);
        }
    }
}

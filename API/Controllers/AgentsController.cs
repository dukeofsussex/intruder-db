// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using API.Models;
    using API.Services;
    using API.Services.Jobs;
    using API.Utils.Resolvers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;

    [Route("agents")]
    public class AgentsController : Controller
    {
        private const string BaseQuery = "SELECT agent.id, steam_id, agent.name, role, avatar_url, xp, time_played, matches_played, matches_won, matches_lost,"
            + " matches_played - matches_won - matches_lost AS matches_tied, matches_survived,  arrests, captures,"
            + " COALESCE(ROUND(matches_won / matches_played, 2), 0) AS win_rate, COALESCE(ROUND(matches_survived / matches_played, 2), 0) AS survival_rate,"
            + " COALESCE(ROUND(time_played / matches_played), 0) AS time_per_match, COALESCE(ROUND(arrests / matches_played, 2), 0) AS arrests_per_match,"
            + " COALESCE(ROUND(captures / matches_played, 2), 0) AS captures_per_match, COALESCE(ROUND(xp / matches_played), 0) AS xp_per_match, status,"
            + " COALESCE(server.id, 0) AS location_server_id, COALESCE(server.name, current_location) AS location_description,"
            + " agent.last_update, last_seen, registered, flagged FROM agent LEFT JOIN server ON server.uuid = agent.current_location";

        private static readonly Dictionary<string, string> ValidOrderingColumns = new Dictionary<string, string>()
        {
            { "default", "agent.name" },
            { "id", "agent.id" },
            { "steamID", "steam_id" },
            { "name", "agent.name" },
            { "role", "role" },
            { "xp", "xp" },
            { "timePlayed", "time_played" },
            { "matchesPlayed", "matches_played" },
            { "matchesWon", "matches_won" },
            { "matchesLost", "matches_lost" },
            { "matchesTied", "matches_tied" },
            { "matchesSurvived", "matches_survived" },
            { "arrests", "arrests" },
            { "captures", "captures" },
            { "winRate", "win_rate" },
            { "survivalRate", "survival_rate" },
            { "timePerMatch", "time_per_match" },
            { "arrestsPerMatch", "arrests_per_match" },
            { "capturesPerMatch", "captures_per_match" },
            { "xpPerMatch", "xp_per_match" },
            { "status", "status" },
            { "lastUpdate", "agent.last_update" },
            { "lastSeen", "last_seen" },
            { "registered", "registered" }
        };

        private readonly AgentJobs agentJobs;

        private readonly IConfiguration config;

        public AgentsController(IConfiguration config, AgentJobs agentJobs)
        {
            this.agentJobs = agentJobs;
            this.config = config;
        }

        /// <summary>
        /// Retrieves a list of agents
        /// </summary>
        /// <remarks>
        /// Ordering columns:
        ///
        ///         id,
        ///         steamID,
        ///         name,
        ///         role,
        ///         xp,
        ///         timePlayed,
        ///         matchesPlayed,
        ///         matchesWon,
        ///         matchesLost,
        ///         matchesTied,
        ///         matchesSurvived,
        ///         arrests,
        ///         captures,
        ///         winRate,
        ///         survivalRate,
        ///         timePerMatch,
        ///         arrestsPerMatch,
        ///         capturesPerMatch,
        ///         xpPerMatch,
        ///         status,
        ///         lastUpdate,
        ///         lastSeen,
        ///         registered
        /// </remarks>
        /// <param name="name">(Optional) Filter results by agent name `(Default: empty)`</param>
        /// <param name="column">(Optional) Column to order the results by `(Default: name)`</param>
        /// <param name="order">(Optional) 0 = ASC, 1 = DESC `(Default: 0)`</param>
        /// <param name="page">(Optional) Instead of providing `limit` and `offset`, passing a page parameter will do all the calculations for you `(Default: 1)`</param>
        /// <response code="200">Agents retrieved</response>
        /// <response code="400">Invalid ordering column</response>
        /// <response code="500">Something broke internally</response>
        // GET: agents
        [HttpGet]
        [ProducesResponseType(typeof(List<Agent>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetList([FromQuery]string name, [FromQuery]string column = "default", [FromQuery]int order = 0, [FromQuery]int page = 1)
        {
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            string queryString = BaseQuery;
            string countLimitingQuery = string.Empty;

            if (!string.IsNullOrEmpty(name))
            {
                countLimitingQuery = " WHERE agent.name LIKE @name";
                queryString += countLimitingQuery;
                parameters.Add("@name", name + '%');
            }

            if (!ValidOrderingColumns.ContainsKey(column))
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = $"Ordering column '{column}' not found"
                });
            }

            DB.GetOrderingStatement(column, order, ValidOrderingColumns, ref queryString);

            Dictionary<string, dynamic> agents = DB.GetPage(queryString, parameters, page, countLimitingQuery, "agent", (x) => Create.Agent(x));

            List<Agent> agentData = agents["data"];

            if (agentData.Count == 0)
            {
                return this.Ok(agents);
            }

            List<AgentRating> ratings = DB.GetList($"SELECT * FROM agent_rating where agent_id IN ({string.Join(',', agentData.Select(a => a.ID).ToList())})", parameters, Create.AgentRating);

            ILookup<int, AgentRating> ratingsLookup = ratings.ToLookup(r => r.AgentID);

            foreach (Agent agent in agentData)
            {
                agent.Ratings.AddRange(ratingsLookup[agent.ID].ToList());
            }

            return this.Ok(agents);
        }

        /// <summary>
        /// Retrieve an agent's list of Steam friends that have connected their Intruder accounts to Steam
        /// </summary>
        /// <response code="200">Friends retrieved</response>
        /// <response code="204">Steam didn't return a friends list (private profile)</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Something went wrong</response>
        // GET: agents/friends
        [Authorize]
        [ProducesResponseType(typeof(List<AgentProfile>), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [HttpGet("friends")]
        public async Task<IActionResult> GetFriends()
        {
            long steamID = long.Parse(this.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "SteamID").Value, CultureInfo.InvariantCulture);

            string friends = await Services.Request
                .GetAsync(new Uri($"https://api.steampowered.com/ISteamUser/GetFriendList/v0001/?key={this.config["Settings:SteamAPIKey"]}&steamid={steamID}&relationship=friend"))
                .ConfigureAwait(false);

            // Private profile
            if (string.IsNullOrEmpty(friends))
            {
                return this.NoContent();
            }

            JsonSerializerSettings deserializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new InputResolver(typeof(AgentProfile)),
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            FriendsList friendsList = JsonConvert.DeserializeObject<FriendsList>(friends, deserializationSettings);

            return this.Ok(DB.GetList(
                "SELECT agent.id AS profile_id, agent.steam_id AS profile_steam_id, agent.name AS profile_name,"
                    + " agent.role AS profile_role, agent.avatar_url AS profile_avatar_url FROM agent WHERE agent.steam_id IN"
                    + $" ({string.Join(',', friendsList.Friends.List.Select(a => a.SteamID).ToList())})",
                null,
                Create.AgentProfile));
        }

        /// <summary>
        /// Retrieves a single agent with ratings
        /// </summary>
        /// <param name="id">(Required) Agent identifier</param>
        /// <param name="update">(Optional) Request an update for the agent's stats `(Default: false)`</param>
        /// <response code="200">Agent retrieved</response>
        /// <response code="400">Requesting an update too soon</response>
        /// <response code="404">Agent not found</response>
        /// <response code="500">Something went wrong</response>
        // GET: agents/<id>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Agent), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> GetSingle(int id, [FromQuery]bool update = false)
        {
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>
            {
                { "@id", id }
            };

            Agent agent = DB.Get(BaseQuery + " WHERE agent.id = @id", parameters, (x) => Create.Agent(x));

            if (agent == null)
            {
                return this.NotFound(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Agent not found"
                });
            }

            if (update && agent.LastUpdate.AddMinutes(Convert.ToDouble(this.config["Settings:AgentUpdateInterval"], CultureInfo.InvariantCulture)) > DateTime.Now)
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = $"Agents can only be updated every {this.config["Settings:AgentUpdateInterval"]} minutes"
                });
            }
            else if (update)
            {
                await this.agentJobs.GetAgentAsync(agent.ID, agent.Name, agent.Flagged).ConfigureAwait(false);
                agent = DB.Get(BaseQuery + " WHERE agent.id = @id", parameters, (x) => Create.Agent(x));
            }

            agent.Ratings.AddRange(GetAgentRatings(id));

            return this.Ok(agent);
        }

        /// <summary>
        /// Retrieves a single agent's badges
        /// </summary>
        /// <param name="id">(Required) Agent identifier</param>
        /// <response code="200">Agent badges retrieved</response>
        /// <response code="404">Agent not found</response>
        /// <response code="500">Something went wrong</response>
        // GET: agents/<id>/badges
        [HttpGet("{id}/badges")]
        [ProducesResponseType(typeof(List<AgentBadge>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetBadges(int id)
        {
            // Check if the agent exists
            int agentID = DB.Get(
                "SELECT id FROM agent WHERE id = @id",
                new Dictionary<string, dynamic>
                {
                    { "@id", id }
                },
                Create.IDOnly);

            if (agentID == 0)
            {
                return this.NotFound(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Agent badges not found"
                });
            }

            return this.Ok(DB.GetList(
                "SELECT * FROM (SELECT badge.id, badge.title, badge.description, 0 AS agent_id, agent_badge.progress FROM `agent_badge`"
                    + " INNER JOIN badge ON badge.id = agent_badge.badge_id WHERE agent_id = @id"
                    + " UNION SELECT badge.id, badge.title, badge.description, 0 AS agent_id, 0 AS progress FROM badge) badges"
                    + " GROUP BY id ",
                new Dictionary<string, dynamic>
                {
                    { "@id", id }
                },
                Create.AgentBadge));
        }

        /// <summary>
        /// Retrieves a single agent's ratings
        /// </summary>
        /// <param name="id">(Required) Agent identifier</param>
        /// <response code="200">Agent ratings retrieved</response>
        /// <response code="404">Agent or agent's ratings not found</response>
        /// <response code="500">Something went wrong</response>
        // GET: agents/<id>/ratings
        [HttpGet("{id}/ratings")]
        [ProducesResponseType(typeof(List<AgentRating>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetRatings(int id)
        {
            List<AgentRating> ratings = GetAgentRatings(id);

            if (ratings.Count == 0)
            {
                return this.NotFound(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Agent ratings not found"
                });
            }

            return this.Ok(ratings);
        }

        /// <summary>
        /// Retrieves a single agent's maps
        /// </summary>
        /// <param name="id">(Required) Agent identifier</param>
        /// <response code="200">Agent maps retrieved</response>
        /// <response code="500">Something went wrong</response>
        // GET: agents/<id>/maps
        [HttpGet("{id}/maps")]
        [ProducesResponseType(typeof(List<Map>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetMaps(int id)
        {
            return this.Ok(DB.GetList(
                "SELECT agent.id AS profile_id, agent.steam_id AS profile_steam_id, agent.name AS profile_name,"
                    + " agent.role AS profile_role, agent.avatar_url AS profile_avatar_url, map.*,"
                    + " COALESCE(((AVG(rating) DIV 0.5) * 0.5 + IF(AVG(rating) MOD 0.5 < 0.25, 0, 0.5)), 0) AS average_rating,"
                    + " SUM(CASE WHEN rating.rating = 1 THEN 1 ELSE 0 END) AS 'rating_1', SUM(CASE WHEN rating.rating = 2 THEN 1 ELSE 0 END) AS 'rating_2',"
                    + " SUM(CASE WHEN rating.rating = 3 THEN 1 ELSE 0 END) AS 'rating_3', SUM(CASE WHEN rating.rating = 4 THEN 1 ELSE 0 END) AS 'rating_4',"
                    + " SUM(CASE WHEN rating.rating = 5 THEN 1 ELSE 0 END) AS 'rating_5' FROM map"
                    + " INNER JOIN agent ON agent.id = map.author_id"
                    + " LEFT JOIN rating ON map.id = rating.type_id AND rating.type = 'Map'"
                    + " WHERE agent.id = @id GROUP BY map.id",
                new Dictionary<string, dynamic>
                {
                    { "@id", id }
                },
                (x) => Create.Map(x)));
        }

        /// <summary>
        /// Retrieves a list of agent's that have recently been updated
        /// </summary>
        /// <response code="200">Agents retrieved</response>
        /// <response code="500">Something went wrong</response>
        // GET: agents/recent
        [HttpGet("recent")]
        [ProducesResponseType(typeof(List<Agent>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetRecent()
        {
            return this.Ok(DB.GetList(BaseQuery + " ORDER BY last_seen DESC LIMIT 25", null, (x) => Create.Agent(x)));
        }

        /// <summary>
        /// Search for an agent by his or her name (the backend for typeahead)
        /// </summary>
        /// <param name="q">(Required) The query string containing (parts of) the agent's name</param>
        /// <param name="deep">(Optional) Specifies whether the agent should be imported into the database `(Default: false)`</param>
        /// <response code="200">Agents retrieved</response>
        /// <response code="400">Missing or malformed query string</response>
        /// <response code="404">Agent not found or not imported</response>
        /// <response code="500">Something went wrong</response>
        // GET: agents/search
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<AgentProfile>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> Search([FromQuery]string q, [FromQuery]bool deep = false)
        {
            if (string.IsNullOrEmpty(q))
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Search query is missing"
                });
            }

            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>()
            {
                { "@name", q + (deep ? string.Empty : "%") }
            };

            List<AgentProfile> agents = DB.GetList(
                "SELECT id AS profile_id, steam_id AS profile_steam_id, name AS profile_name, role AS profile_role, avatar_url AS profile_avatar_url"
                + $" FROM agent WHERE name {(deep ? "=" : "LIKE")} @name {(deep ? string.Empty : "LIMIT 10")}",
                parameters,
                Create.AgentProfile);

            if (deep && agents.Count == 0)
            {
                int agentID = await this.agentJobs.GetAgentAsync(0, q, false).ConfigureAwait(false);

                if (agentID == 0)
                {
                    return this.NotFound(new ErrorResponse()
                    {
                        Code = (int)HttpStatusCode.NotFound,
                        Message = "Unable to find an agent with that name or insufficient xp"
                    });
                }

                agents.Add(DB.Get(
                    $"SELECT id AS profile_id, steam_id AS profile_steam_id, name AS profile_name, role AS profile_role, avatar_url AS profile_avatar_url FROM agent WHERE id = @id",
                    new Dictionary<string, dynamic>()
                    {
                        { "@id", agentID }
                    },
                    Create.AgentProfile));
            }

            return this.Ok(agents);
        }

        /// <summary>
        /// Returns the amount of new agents per month (only tracked agents are counted)
        /// </summary>
        /// <response code="200">Agents retrieved</response>
        /// <response code="500">Something went wrong</response>
        // GET: agents/newPerMonth
        [HttpGet("newPerMonth")]
        [ProducesResponseType(typeof(List<Activity>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetMonthlyPurchases()
        {
            return this.Ok(DB.GetList(
                "SELECT 0 AS server_id, COUNT(*) AS agents, DATE_FORMAT(registered, '%Y-%m-01 00:00:00') AS timestamp"
                + " FROM agent GROUP BY YEAR(registered), MONTH(registered)",
                null,
                Create.Activity));
        }

        // PUT: agents/<id>/claim/<uid>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("{id}/claim/{uid}")]
        public IActionResult ConfirmClaim(int id, Guid uid)
        {
            if (id < 1)
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Agent id is invalid"
                });
            }

            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>()
            {
                { "@agentID", id },
                { "@uid", uid }
            };

            Claim claim = DB.Get(
                "SELECT claim.uid AS claim_uid, claim.sent AS claim_sent, claim.timestamp AS claim_timestamp,"
                    + " id AS profile_id, claim.steam_id AS profile_steam_id, name AS profile_name, role AS profile_role, avatar_url AS profile_avatar_url"
                    + " FROM claim INNER JOIN agent ON agent.id = claim.agent_id WHERE claim.agent_id = @agentID AND claim.uid = @uid",
                parameters,
                Create.Claim);

            if (claim == null)
            {
                return this.NotFound(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Claim not found or expired"
                });
            }

            parameters.Add("@steamID", claim.Claimer.SteamID);

            DB.Update("UPDATE agent SET steam_id = @steamID WHERE id = @agentID", parameters);

            DB.Delete("DELETE FROM claim WHERE agent_id = @agentID", parameters);

            return this.Ok(new { Code = (int)HttpStatusCode.OK, Message = "Agent claimed" });
        }

        // PUT: agents/<id>/claim
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize]
        [HttpPut("{id}/claim")]
        public IActionResult Claim(int id)
        {
            System.Security.Claims.ClaimsPrincipal claimer = this.HttpContext.User;

            if (!claimer.HasClaim(c => c.Type == "SteamID"))
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "SteamID is invalid"
                });
            }
            else if (id < 1)
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "Agent id is invalid"
                });
            }

            long claimerSteamID = long.Parse(claimer.Claims.FirstOrDefault(c => c.Type == "SteamID").Value, CultureInfo.InvariantCulture);

            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>()
            {
                { "@id", id },
                { "@steamID", claimerSteamID }
            };

            int claimed = DB.Get("SELECT id FROM agent WHERE steam_id = @steamID", parameters, Create.IDOnly);

            if (claimed != 0)
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "You have already claimed an agent"
                });
            }

            AgentProfile claim = DB.Get(
                "SELECT id AS profile_id, steam_id AS profile_steam_id, name AS profile_name, role AS profile_role, avatar_url AS profile_avatar_url"
                + " FROM agent WHERE id = @id",
                parameters,
                Create.AgentProfile);

            if (claim != null && claim.SteamID != 0 && claim.SteamID != claimerSteamID)
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "This agent has already been claimed"
                });
            }

            if (claim != null && claim.Role == AgentRole.Dev)
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "This agent is a developer and cannot be claimed"
                });
            }

            int agentID = DB.Get("SELECT agent_id AS id FROM claim WHERE steam_id = @steamID", parameters, Create.IDOnly);

            if (agentID != 0)
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = "You already have an open claim for an agent! A new claim can be made after 24 hours"
                });
            }

            parameters.Add("uid", Guid.NewGuid());
            parameters.Add("sent", false);

            DB.Insert(
                "INSERT INTO claim(agent_id, steam_id, uid, sent, timestamp) VALUES (@id, @steamID, @uid, @sent, NOW())",
                parameters,
                false);

            return this.Ok(new { Code = (int)HttpStatusCode.OK, Message = "Claim stored" });
        }

        private static List<AgentRating> GetAgentRatings(int id)
        {
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>
            {
                { "@id", id }
            };

            return DB.GetList("SELECT * FROM agent_rating where agent_id = @id", parameters, Create.AgentRating);
        }
    }
}

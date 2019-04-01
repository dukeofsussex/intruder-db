// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using API.Models;
    using API.Services;
    using Microsoft.AspNetCore.Mvc;

    [Route("servers")]
    public class ServersController : Controller
    {
        private const string BaseQuery = "SELECT agent.id AS profile_id, agent.steam_id AS profile_steam_id, agent.name AS profile_name,"
            + " agent.role AS profile_role, agent.avatar_url AS profile_avatar_url, server.*, activity.agents, map.id AS map_id, map.uid AS map_uid,"
            + " map.name AS map_name, url, map.version AS map_version, stage, COALESCE(((AVG(rating) DIV 0.5) * 0.5 + IF(AVG(rating) MOD 0.5 < 0.25, 0, 0.5)), 0) AS average_rating,"
            + " SUM(CASE WHEN rating.rating = 1 THEN 1 ELSE 0 END) AS 'rating_1', SUM(CASE WHEN rating.rating = 2 THEN 1 ELSE 0 END) AS 'rating_2',"
            + " SUM(CASE WHEN rating.rating = 3 THEN 1 ELSE 0 END) AS 'rating_3', SUM(CASE WHEN rating.rating = 4 THEN 1 ELSE 0 END) AS 'rating_4',"
            + " SUM(CASE WHEN rating.rating = 5 THEN 1 ELSE 0 END) AS 'rating_5', images, played, has_floorplan, map.last_update as map_last_update FROM server"
            + " INNER JOIN activity ON server.id = activity.server_id"
            + " INNER JOIN map ON activity.map_id = map.id"
            + " INNER JOIN agent ON map.author_id = agent.id"
            + " LEFT JOIN rating ON map.id = rating.type_id AND rating.type = 'Map'"
            + " WHERE activity.timestamp = server.last_update";

        private static readonly Dictionary<string, string> ValidOrderingColumns = new Dictionary<string, string>()
        {
            { "default", "name" },
            { "id", "id" },
            { "uuid", "uuid" },
            { "name", "name" },
            { "fancyName", "fancy_name" },
            { "description", "description" },
            { "region", "region" },
            { "type", "type" },
            { "style", "style" },
            { "map", "map_name" },
            { "mapType", "map_type" },
            { "agents", "agents" },
            { "maxAgents", "max_agents" },
            { "maxSpectators", "max_spectators" },
            { "gamemode", "gamemode" },
            { "timemode", "timemode" },
            { "time", "time" },
            { "inProgress", "in_progress" },
            { "ranked", "ranked" },
            { "joinableBy", "joinable_by" },
            { "visibleBy", "visible_by" },
            { "passworded", "passworded" },
            { "lastUpdate", "last_update" }
        };

        /// <summary>
        /// Retrieves a list of servers
        /// </summary>
        /// <remarks>
        /// Ordering columns:
        ///
        ///         id,
        ///         uid,
        ///         name,
        ///         fancyName,
        ///         description,
        ///         region,
        ///         type,
        ///         style,
        ///         map,
        ///         mapType,
        ///         agents,
        ///         maxAgents,
        ///         maxSpectators,
        ///         gamemode,
        ///         timemode,
        ///         time,
        ///         inProgress,
        ///         ranked,
        ///         joinableBy,
        ///         visibleBy,
        ///         passworded,
        ///         lastUpdate
        /// </remarks>
        /// <param name="region">(Optional) Filter results by region `(Default: empty)`</param>
        /// <param name="column">(Optional) Column to order the results by `(Default: name)`</param>
        /// <param name="order">(Optional) 0 = ASC, 1 = DESC `(Default: 0)`</param>
        /// <param name="page">(Optional) Instead of providing `limit` and `offset`, passing a page parameter will do all the calculations for you `(Default: 1)`</param>
        /// <response code="200">Servers retrieved</response>
        /// <response code="400">Invalid ordering column</response>
        /// <response code="500">Something broke internally</response>
        // GET: servers
        [HttpGet]
        [ProducesResponseType(typeof(List<Server>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetList([FromQuery]string region, [FromQuery]string column = "default", [FromQuery]int order = 0, [FromQuery]int page = 1)
        {
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            string queryString = BaseQuery;
            string countLimitingQuery = string.Empty;

            if (!string.IsNullOrEmpty(region))
            {
                countLimitingQuery = " WHERE region = @region";
                queryString += " AND region = @region";
                parameters.Add("@region", region);
            }

            queryString += " GROUP BY server.id";

            if (!ValidOrderingColumns.ContainsKey(column))
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = $"Ordering column '{column}' not found"
                });
            }

            DB.GetOrderingStatement(column, order, ValidOrderingColumns, ref queryString);

            Dictionary<string, dynamic> servers = DB.GetPage(queryString, parameters, page, countLimitingQuery, "server", (x) => Create.Server(x));

            List<Server> serverData = servers["data"];

            if (serverData.Count == 0)
            {
                return this.Ok(servers);
            }

            List<AgentServerProfile> agents = DB.GetList(
                "SELECT agent.id AS profile_id, agent.steam_id AS profile_steam_id, agent.name AS profile_name, agent.role AS profile_role, agent.avatar_url AS profile_avatar_url,"
                + $" current_location AS server_uuid FROM agent WHERE current_location IN ('{string.Join("','", serverData.Select(s => s.UUID).ToList())}')",
                null,
                Create.AgentServerProfile);

            ILookup<string, AgentServerProfile> agentsLookup = agents.ToLookup(r => r.ServerUUID);

            foreach (Server server in serverData)
            {
                server.OnlineAgents.AddRange(agentsLookup[server.UUID].ToList());
            }

            return this.Ok(servers);
        }

        /// <summary>
        /// Retrieves a single server
        /// </summary>
        /// <param name="id">(Required) Server identifier</param>
        /// <response code="200">Server retrieved</response>
        /// <response code="404">Server not found</response>
        /// <response code="500">Something broke internally</response>
        // GET: servers/<id>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Server), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetSingle(int id)
        {
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>
            {
                { "@id", id }
            };

            Server server = DB.Get(
                BaseQuery + " AND server.id = @id GROUP BY server.id",
                parameters,
                (x) => Create.Server(x));

            if (server == null)
            {
                return this.NotFound(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Server not found"
                });
            }

            server.OnlineAgents.AddRange(DB.GetList(
                "SELECT id AS profile_id, steam_id AS profile_steam_id, name AS profile_name, role AS profile_role, avatar_url AS profile_avatar_url"
                + " FROM agent WHERE status = @status AND current_location = @currentLocation",
                new Dictionary<string, dynamic>()
                {
                    { "@status", Status.Online },
                    { "@currentLocation", server.UUID }
                },
                Create.AgentProfile));

            return this.Ok(server);
        }

        /// <summary>
        /// Retrieves a single server's activity profile
        /// </summary>
        /// <param name="id">(Required) Server identifier</param>
        /// <param name="period">(Optional) The period to fetch the activity for (check activity for the possible parameter values) `(Default: day)`</param>
        /// <response code="200">Server activity profile retrieved</response>
        /// <response code="404">Server activity profile not found</response>
        /// <response code="500">Something broke internally</response>
        // GET: servers/<id>/activity
        [HttpGet("{id}/activity")]
        [ProducesResponseType(typeof(List<Activity>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetActivity(int id, [FromQuery]string period)
        {
            List<Activity> activityEntries = ActivityController.GetActivity(id, period);

            if (activityEntries.Count == 0)
            {
                return this.NotFound(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "No activity for this period"
                });
            }

            return this.Ok(activityEntries);
        }

        /// <summary>
        /// Retrieves a single server's map popularity profile
        /// </summary>
        /// <param name="id">(Required) Server identifier</param>
        /// <param name="period">(Optional) The period to fetch the activity for (check map activity for the possible parameter values) `(Default: day)`</param>
        /// <response code="200">Server map popularity profile retrieved</response>
        /// <response code="404">Server map popularity profile not found</response>
        /// <response code="500">Something broke internally</response>
        // GET: servers/<id>/activity/maps
        [HttpGet("{id}/activity/maps")]
        [ProducesResponseType(typeof(List<MapActivity>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetMapActivity(int id, [FromQuery]string period)
        {
            List<MapActivity> mapActivityEntries = ActivityController.GetMapActivity(id, period);

            if (mapActivityEntries.Count == 0)
            {
                return this.NotFound(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "No map activity for this period"
                });
            }

            return this.Ok(mapActivityEntries);
        }
    }
}

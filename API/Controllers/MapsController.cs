// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using API.Models;
    using API.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("maps")]
    public class MapsController : Controller
    {
        private const string BaseQuery = "SELECT agent.id AS profile_id, agent.steam_id AS profile_steam_id, agent.name AS profile_name,"
            + " agent.role AS profile_role, agent.avatar_url AS profile_avatar_url, map.*,"
            + " COALESCE(((AVG(rating) DIV 0.5) * 0.5 + IF(AVG(rating) MOD 0.5 < 0.25, 0, 0.5)), 0) AS average_rating,"
            + " SUM(CASE WHEN rating.rating = 1 THEN 1 ELSE 0 END) AS 'rating_1', SUM(CASE WHEN rating.rating = 2 THEN 1 ELSE 0 END) AS 'rating_2',"
            + " SUM(CASE WHEN rating.rating = 3 THEN 1 ELSE 0 END) AS 'rating_3', SUM(CASE WHEN rating.rating = 4 THEN 1 ELSE 0 END) AS 'rating_4',"
            + " SUM(CASE WHEN rating.rating = 5 THEN 1 ELSE 0 END) AS 'rating_5' FROM map"
            + " INNER JOIN agent ON agent.id = map.author_id"
            + " LEFT JOIN rating ON map.id = rating.type_id AND rating.type = 'Map'";

        private static readonly Dictionary<string, string> ValidOrderingColumns = new Dictionary<string, string>()
        {
            { "default", "name" },
            { "id", "id" },
            { "uid", "uid" },
            { "name", "name" },
            { "author", "profile_name" },
            { "version", "version" },
            { "stage", "stage" },
            { "played", "played" },
            { "averageRating", "average_rating" },
            { "hasFloorplan", "has_floorplan" },
            { "images", "images" },
            { "lastUpdate", "last_update" }
        };

        /// <summary>
        /// Retrieves a list of maps
        /// </summary>
        /// <remarks>
        /// Responses:
        ///
        ///     GET /maps
        ///     200 OK
        ///     {
        ///         "page": 1, // Current page
        ///         "totalPages": 1, // Total pages
        ///         "data": [ ... ] // Array of maps
        ///     }
        ///
        ///     GET /maps?column=bogus
        ///     400 Bad Request
        ///     {
        ///         "code": 400,
        ///         "message": "Ordering column 'bogus' not found"
        ///     }
        ///
        /// Ordering columns:
        ///
        ///         id,
        ///         uid,
        ///         name,
        ///         author,
        ///         version,
        ///         stage,
        ///         played,
        ///         hasFloorplan,
        ///         images,
        ///         lastUpdate
        /// </remarks>
        /// <param name="name">(Optional) Filter results by map name or author `(Default: empty)`</param>
        /// <param name="column">(Optional) Column to order the results by `(Default: name)`</param>
        /// <param name="order">(Optional) 0 = ASC, 1 = DESC `(Default: 0)`</param>
        /// <param name="page">(Optional) Instead of providing `limit` and `offset`, passing a page parameter will do all the calculations for you `(Default: 1)`</param>
        /// <response code="200">Maps retrieved</response>
        /// <response code="400">Invalid ordering column</response>
        /// <response code="500">Something broke internally</response>
        // GET: maps
        [HttpGet]
        [ProducesResponseType(typeof(List<Map>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetList([FromQuery]string name, [FromQuery]string column = "default", [FromQuery]int order = 0, [FromQuery]int page = 1)
        {
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            string queryString = BaseQuery;
            string countLimitingQuery = string.Empty;

            if (!string.IsNullOrEmpty(name))
            {
                countLimitingQuery = " INNER JOIN agent ON agent.id = map.author_id WHERE map.name LIKE @name OR agent.name LIKE @name";
                queryString += " WHERE map.name LIKE @name OR agent.name LIKE @name";
                parameters.Add("@name", name + '%');
            }

            queryString += " GROUP BY map.id";

            if (!ValidOrderingColumns.ContainsKey(column))
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = $"Ordering column '{column}' not found"
                });
            }

            DB.GetOrderingStatement(column, order, ValidOrderingColumns, ref queryString);

            return this.Ok(DB.GetPage(queryString, parameters, page, countLimitingQuery, "map", (x) => Create.Map(x)));
        }

        /// <summary>
        /// Retrieves a single map
        /// </summary>
        /// <param name="id">(Required) Map identifier</param>
        /// <response code="200">Map retrieved</response>
        /// <response code="404">Map not found</response>
        /// <response code="500">Something broke internally</response>
        // GET: maps/<id>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Map), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetSingle(int id)
        {
            Map map = DB.Get(
                BaseQuery + " WHERE map.id = @id GROUP BY map.id",
                new Dictionary<string, dynamic>
                {
                    { "@id", id }
                },
                (x) => Create.Map(x));

            if (map == null)
            {
                return this.NotFound(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Map not found"
                });
            }

            return this.Ok(map);
        }

        /// <summary>
        /// Retrieves a user's rating of a single map
        /// </summary>
        /// <param name="id">(Required) Map identifier</param>
        /// <response code="200">Rating retrieved</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Rating not found</response>
        /// <response code="500">Something broke internally</response>
        // GET: maps/<id>/ratings
        [Authorize]
        [HttpGet("{id}/ratings")]
        [ProducesResponseType(typeof(Rating), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetRating(int id)
        {
            int agentID = int.Parse(this.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentID").Value, CultureInfo.InvariantCulture);

            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>
            {
                { "@id", id },
                { "@agentID", agentID }
            };

            Rating rating = DB.Get("SELECT * FROM rating WHERE agent_id = @agentID AND type_id = @id AND type = 'Map'", parameters, Create.Rating);

            if (rating == null)
            {
                return this.NotFound(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Rating not found"
                });
            }

            return this.Ok(rating);
        }

        /// <summary>
        /// Adds (POST) or updates (PUT) a user's rating of a single map
        /// </summary>
        /// <param name="id">(Required) Map identifier</param>
        /// <param name="rating">(Required) Rating</param>
        /// <response code="200">Rating added/updated</response>
        /// <response code="400">Rating invalid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Map not found</response>
        /// <response code="500">Something broke internally</response>
        // POST/PUT: maps/<id>/ratings
        [Authorize]
        [HttpPost("{id}/ratings")]
        [HttpPut("{id}/ratings")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult AddUpdateRating(int id, [FromBody]Rating rating)
        {
            return this.HandleRating(id, rating);
        }

        /// <summary>
        /// Deletes a user's rating of a single map
        /// </summary>
        /// <param name="id">(Required) Map identifier</param>
        /// <response code="200">Rating deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Rating not found</response>
        /// <response code="500">Something broke internally</response>
        // DELETE: maps/<id>/ratings
        [Authorize]
        [HttpDelete("{id}/ratings")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult DeleteRating(int id)
        {
            return this.HandleRating(id);
        }

        private IActionResult HandleRating(int id, Rating rating = null)
        {
            int agentID = int.Parse(this.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentID").Value, CultureInfo.InvariantCulture);

            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>
            {
                { "@id", id },
                { "@agentID", agentID }
            };

            if (this.Request.Method != "DELETE")
            {
                if (rating == null)
                {
                    return this.BadRequest(new ErrorResponse()
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "Rating contains invalid properties"
                    });
                }
                else if (rating.Value < 1 || rating.Value > 5)
                {
                    return this.BadRequest(new ErrorResponse()
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "Invalid rating value"
                    });
                }

                parameters.Add("@rating", rating.Value);
            }

            if (this.Request.Method == "POST")
            {
                Map map = DB.Get(BaseQuery + " WHERE map.id = @id GROUP BY map.id", parameters, (x) => Create.Map(x));

                if (map == null)
                {
                    return this.NotFound(new ErrorResponse()
                    {
                        Code = (int)HttpStatusCode.NotFound,
                        Message = "Map not found"
                    });
                }
                else if (map.Author.ID == agentID)
                {
                    return this.NotFound(new ErrorResponse()
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "You cannot rate your own map"
                    });
                }
            }

            Rating currentRating = DB.Get("SELECT * FROM rating WHERE agent_id = @agentID AND type_id = @id AND type = 'Map'", parameters, Create.Rating);

            if (currentRating == null && this.Request.Method != "POST")
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Rating not found"
                });
            }
            else if (currentRating != null && this.Request.Method == "POST")
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Rating already exists"
                });
            }

            if (this.Request.Method == "POST")
            {
                DB.Insert("INSERT INTO rating(agent_id, type_id, type, rating) VALUES (@agentID, @id, 'Map', @rating)", parameters);
            }
            else if (this.Request.Method == "PUT")
            {
                DB.Update("UPDATE rating SET rating = @rating WHERE agent_id = @agentID AND type_id = @id AND type = 'Map'", parameters);
            }
            else
            {
                DB.Delete("DELETE FROM rating WHERE agent_id = @agentID AND type_id = @id AND type = 'Map'", parameters);
            }

            if (this.Request.Method != "DELETE")
            {
                return this.GetRating(id);
            }

            return this.Ok(new { code = (int)HttpStatusCode.OK, message = "Deleted" });
        }
    }
}

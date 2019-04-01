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
    using Microsoft.Extensions.Configuration;

    [Route("tunings")]
    public class TuningController : Controller
    {
        private const string BaseQuery = "SELECT tuning.id AS tuning_id, tuning.name AS tuning_name, description AS tuning_description,"
                + " tuning.settings AS tuning_settings, tuning.share AS tuning_share, tuning.last_update AS tuning_last_update,"
                + " author_id AS profile_id, steam_id AS profile_steam_id, agent.name AS profile_name, role AS profile_role, avatar_url AS profile_avatar_url,"
                + " COALESCE(((AVG(rating) DIV 0.5) * 0.5 + IF(AVG(rating) MOD 0.5 < 0.25, 0, 0.5)), 0) AS average_rating,"
                + " SUM(CASE WHEN rating.rating = 1 THEN 1 ELSE 0 END) AS 'rating_1', SUM(CASE WHEN rating.rating = 2 THEN 1 ELSE 0 END) AS 'rating_2',"
                + " SUM(CASE WHEN rating.rating = 3 THEN 1 ELSE 0 END) AS 'rating_3', SUM(CASE WHEN rating.rating = 4 THEN 1 ELSE 0 END) AS 'rating_4',"
                + " SUM(CASE WHEN rating.rating = 5 THEN 1 ELSE 0 END) AS 'rating_5'"
                + " FROM tuning INNER JOIN agent ON agent.id = tuning.author_id"
                + " LEFT JOIN rating ON tuning.id = rating.type_id AND rating.type = 'Tuning'";

        private static readonly Dictionary<string, string> ValidOrderingColumns = new Dictionary<string, string>()
        {
            { "default", "tuning_name" },
            { "id", "tuning_id" },
            { "name", "tuning_name" },
            { "description", "tuning_description" },
            { "author", "agent.name" },
            { "averageRating", "average_rating" },
            { "lastUpdate", "tuning_last_update" }
        };

        private readonly IConfiguration config;

        public TuningController(IConfiguration config)
        {
            this.config = config;
        }

        /// <summary>
        /// Retrieves a list of shared tuning settings
        /// </summary>
        /// <remarks>
        /// Ordering columns:
        ///
        ///         id,
        ///         name,
        ///         description,
        ///         author,
        ///         averageRating,
        ///         lastUpdate
        /// </remarks>
        /// <param name="name">(Optional) Filter results by tuning name or author name `(Default: empty)`</param>
        /// <param name="column">(Optional) Column to order the results by `(Default: name)`</param>
        /// <param name="order">(Optional) 0 = ASC, 1 = DESC `(Default: 0)`</param>
        /// <param name="page">(Optional) Instead of providing `limit` and `offset`, passing a page parameter will do all the calculations for you `(Default: 1)`</param>
        /// <response code="200">Tuning settings retrieved</response>
        /// <response code="400">Invalid ordering column</response>
        /// <response code="500">Something broke internally</response>
        // GET: tunings/shared
        [HttpGet("shared")]
        [ProducesResponseType(typeof(List<Tuning>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetShared([FromQuery]string name, [FromQuery]string column = "default", [FromQuery]int order = 0, [FromQuery]int page = 1)
        {
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            string queryString = BaseQuery;
            string countLimitingQuery = " WHERE share = 1";

            if (!string.IsNullOrEmpty(name))
            {
                countLimitingQuery += " AND (tuning.name LIKE @name OR agent.name LIKE @name)";
                parameters.Add("@name", name + '%');
            }

            queryString += countLimitingQuery + " GROUP BY tuning.id";
            countLimitingQuery = " INNER JOIN agent ON agent.id = tuning.author_id" + countLimitingQuery;

            if (!ValidOrderingColumns.ContainsKey(column))
            {
                return this.BadRequest(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = $"Ordering column '{column}' not found"
                });
            }

            DB.GetOrderingStatement(column, order, ValidOrderingColumns, ref queryString);

            return this.Ok(DB.GetPage(queryString, parameters, page, countLimitingQuery, "tuning", Create.Tuning));
        }

        /// <summary>
        /// Retrieves a user's tuning settings
        /// </summary>
        /// <response code="200">Tuning settings retrieved</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Something broke internally</response>
        // GET: tunings
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(List<Tuning>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetAgents()
        {
            int agentID = int.Parse(this.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentID").Value, CultureInfo.InvariantCulture);

            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>()
            {
                { "@authorID", agentID }
            };

            return this.Ok(DB.GetList(BaseQuery + " WHERE tuning.author_id = @authorID GROUP BY tuning.id", parameters, Create.Tuning));
        }

        /// <summary>
        /// Retrieves a single tuning
        /// </summary>
        /// <param name="id">(Required) Tuning identifier</param>
        /// <response code="200">Tuning retrieved</response>
        /// <response code="403">Forbidden</response>
        /// <response code="404">Tuning not found</response>
        /// <response code="500">Something broke internally</response>
        // GET: tunings/<id>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Tuning), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetSingle(int id)
        {
            int agentID = 0;

            if (this.HttpContext.User.Identity.IsAuthenticated)
            {
                agentID = int.Parse(this.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentID").Value, CultureInfo.InvariantCulture);
            }

            Tuning tuning = DB.Get(
                BaseQuery + " WHERE tuning.id = @id GROUP BY tuning.id",
                new Dictionary<string, dynamic>
                {
                    { "@id", id }
                },
                Create.Tuning);

            if (tuning == null)
            {
                return this.NotFound(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Tuning not found"
                });
            }
            else if (!tuning.Share && tuning.Author.ID != agentID)
            {
                return this.StatusCode(403, new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.Forbidden,
                    Message = "You do not have access to this tuning"
                });
            }

            return this.Ok(tuning);
        }

        /// <summary>
        /// Adds a user's tuning
        /// </summary>
        /// <param name="tuning">(Required) Tuning</param>
        /// <response code="200">Tuning added</response>
        /// <response code="400">Tuning invalid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Something broke internally</response>
        // POST: tunings
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(Tuning), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult Add([FromBody]Tuning tuning)
        {
            return this.AddUpdateDelete(0, tuning);
        }

        /// <summary>
        /// Updates a user's tuning
        /// </summary>
        /// <param name="id">(Required) Tuning identifier</param>
        /// <param name="tuning">(Required) Tuning</param>
        /// <response code="200">Tuning updated</response>
        /// <response code="400">Tuning invalid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Tuning not found</response>
        /// <response code="500">Something broke internally</response>
        // PUT: tunings/<id>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Tuning), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult Update(int id, [FromBody]Tuning tuning)
        {
            return this.AddUpdateDelete(id, tuning);
        }

        /// <summary>
        /// Deletes a user's tuning
        /// </summary>
        /// <param name="id">(Required) Tuning identifier</param>
        /// <response code="200">Tuning deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Tuning not found</response>
        /// <response code="500">Something broke internally</response>
        // DELETE: tunings/<id>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult Delete(int id)
        {
            return this.AddUpdateDelete(id);
        }

        /// <summary>
        /// Retrieves a user's rating of a single tuning
        /// </summary>
        /// <param name="id">(Required) Tuning identifier</param>
        /// <response code="200">Rating retrieved</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Rating not found</response>
        /// <response code="500">Something broke internally</response>
        // GET: tunings/<id>/ratings
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

            Rating rating = DB.Get("SELECT * FROM rating WHERE agent_id = @agentID AND type_id = @id AND type = 'Tuning'", parameters, Create.Rating);

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
        /// Adds (POST) or updates (PUT) a user's rating of a single tuning
        /// </summary>
        /// <param name="id">(Required) Tuning identifier</param>
        /// <param name="rating">(Required) Rating</param>
        /// <response code="200">Tuning added/updated</response>
        /// <response code="400">Rating invalid</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Tuning not found</response>
        /// <response code="500">Something broke internally</response>
        // POST/PUT: tunings/<id>/ratings
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
        /// Deletes a user's rating of a single tuning
        /// </summary>
        /// <param name="id">(Required) Tuning identifier</param>
        /// <response code="200">Rating deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Rating not found</response>
        /// <response code="500">Something broke internally</response>
        // DELETE: tunings/<id>/ratings
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

        private IActionResult AddUpdateDelete(int id, Tuning tuning = null)
        {
            int agentID = int.Parse(this.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AgentID").Value, CultureInfo.InvariantCulture);

            if (this.Request.Method != "DELETE")
            {
                if (tuning == null)
                {
                    return this.BadRequest(new ErrorResponse()
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "Tuning contains invalid properties"
                    });
                }

                if (string.IsNullOrEmpty(tuning.Name))
                {
                    return this.BadRequest(new ErrorResponse()
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "Tuning name is missing"
                    });
                }
                else if (tuning.Settings.ToString(CultureInfo.InvariantCulture) == null
                    || tuning.Settings.ToString(CultureInfo.InvariantCulture) == "{}")
                {
                    return this.BadRequest(new ErrorResponse()
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "Tuning settings are invalid"
                    });
                }
            }

            if (this.Request.Method != "POST")
            {
                Tuning currentTuning = DB.Get(
                    BaseQuery + " WHERE tuning.id = @id GROUP BY tuning.id",
                    new Dictionary<string, dynamic>
                    {
                        { "@id", id }
                    },
                    Create.Tuning);

                if (currentTuning == null)
                {
                    return this.NotFound(new ErrorResponse()
                    {
                        Code = (int)HttpStatusCode.NotFound,
                        Message = "Tuning not found"
                    });
                }
                else if (currentTuning.Author.ID != agentID)
                {
                    return this.StatusCode(403, new ErrorResponse()
                    {
                        Code = (int)HttpStatusCode.Forbidden,
                        Message = "You do not have access to this tuning"
                    });
                }

                if (this.Request.Method == "DELETE")
                {
                    Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>()
                    {
                        { "@id", id }
                    };

                    DB.Delete("DELETE FROM tuning WHERE id = @id", parameters);

                    DB.Delete("DELETE FROM rating WHERE type_id = @id AND type = 'Tuning'", parameters);

                    return this.Ok(new { code = (int)HttpStatusCode.OK, message = "Deleted" });
                }

                DB.Update(
                    "UPDATE tuning SET author_id = @authorID, name = @name, description = @description, settings = @settings, share = @share, last_update = NOW() WHERE id = @id",
                    new Dictionary<string, dynamic>()
                    {
                        { "@id", id },
                        { "@authorID", agentID },
                        { "@name", tuning.Name },
                        { "@description", tuning.Description },
                        { "@settings", tuning.Settings },
                        { "@share", tuning.Share }
                    });

                tuning.ID = id;
            }
            else
            {
                int count = DB.Get(
                    "SELECT COUNT(*) AS id FROM tuning WHERE author_id = @authorID",
                    new Dictionary<string, dynamic>()
                    {
                        { "@authorID", agentID }
                    },
                    Create.IDOnly);

                int maxTuningSettings = int.Parse(this.config["Settings:MaxTuningSettings"], CultureInfo.InvariantCulture);

                if (count >= maxTuningSettings)
                {
                    return this.BadRequest(new ErrorResponse()
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = $"You can only save {maxTuningSettings} tuning settings"
                    });
                }

                tuning.ID = DB.Insert(
                    "INSERT INTO tuning(author_id, name, description, settings, share, last_update) VALUES (@authorID, @name, @description, @settings, @share, NOW())",
                    new Dictionary<string, dynamic>()
                    {
                        { "@authorID", agentID },
                        { "@name", tuning.Name },
                        { "@description", tuning.Description },
                        { "@settings", tuning.Settings },
                        { "@share", tuning.Share }
                    },
                    true);
            }

            return this.GetSingle(tuning.ID);
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
                Tuning tuning = DB.Get(BaseQuery + " WHERE tuning.id = @id GROUP BY tuning.id", parameters, Create.Tuning);

                if (tuning == null)
                {
                    return this.NotFound(new ErrorResponse()
                    {
                        Code = (int)HttpStatusCode.NotFound,
                        Message = "Tuning not found"
                    });
                }
                else if (tuning.Author.ID == agentID)
                {
                    return this.NotFound(new ErrorResponse()
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = "You cannot rate your own tuning"
                    });
                }
            }

            Rating currentRating = DB.Get("SELECT * FROM rating WHERE agent_id = @agentID AND type_id = @id AND type = 'Tuning'", parameters, Create.Rating);

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
                DB.Insert("INSERT INTO rating(agent_id, type_id, type, rating) VALUES (@agentID, @id, 'Tuning', @rating)", parameters);
            }
            else if (this.Request.Method == "PUT")
            {
                DB.Update("UPDATE rating SET rating = @rating WHERE agent_id = @agentID AND type_id = @id AND type = 'Tuning'", parameters);
            }
            else
            {
                DB.Delete("DELETE FROM rating WHERE agent_id = @agentID AND type_id = @id AND type = 'Tuning'", parameters);
            }

            if (this.Request.Method != "DELETE")
            {
                return this.GetRating(id);
            }

            return this.Ok(new { code = (int)HttpStatusCode.OK, message = "Deleted" });
        }
    }
}

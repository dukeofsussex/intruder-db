// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using API.Models;
    using API.Services;
    using Microsoft.AspNetCore.Mvc;

    [Route("badges")]
    public class BadgesController : Controller
    {
        private const string BaseQuery = "SELECT id, title, description FROM badge";

        /// <summary>
        /// Retrieves a list of badges
        /// </summary>
        /// <response code="200">Badges retrieved</response>
        /// <response code="500">Something broke internally</response>
        // GET: badges
        [HttpGet]
        [ProducesResponseType(typeof(List<Badge>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetList()
        {
            return this.Ok(DB.GetList(BaseQuery, null, Create.Badge));
        }

        /// <summary>
        /// Retrieves a single badge
        /// </summary>
        /// <param name="id">(Required) Badge identifier</param>
        /// <response code="200">Badge retrieved</response>
        /// <response code="404">Badge not found</response>
        /// <response code="500">Something went wrong</response>
        // GET: badges/<id>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Badge), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetSingle(int id)
        {
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>
            {
                { "@id", id }
            };

            Badge badge = DB.Get(BaseQuery + " WHERE id = @id", parameters, Create.Badge);

            if (badge == null)
            {
                return this.NotFound(new ErrorResponse()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Badge not found"
                });
            }

            return this.Ok(badge);
        }
    }
}

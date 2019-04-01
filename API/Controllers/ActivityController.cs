// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using API.Models;
    using API.Services;
    using Microsoft.AspNetCore.Mvc;

    [Route("activity")]
    public class ActivityController : Controller
    {
        public static List<Activity> GetActivity(int serverID, string period)
        {
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();

            if (serverID != 0)
            {
                parameters.Add("@id", serverID);
            }

            string divider = string.Empty;

            switch (period)
            {
                case "m":
                case "month":
                    divider = "10800";
                    break;
                case "w":
                case "week":
                    divider = "3600";
                    break;
                case "d":
                case "day":
                default:
                    divider = "600";
                    break;
            }

            string queryString = $"SELECT {(serverID != 0 ? "server_id" : "0 AS server_id")}, MAX(agents) AS agents, FROM_UNIXTIME((UNIX_TIMESTAMP(`timestamp`) DIV {divider}) * {divider}) as timestamp"
                + " FROM ("
                    + " SELECT server_id, SUM(agents) as agents, timestamp "
                    + " FROM activity"
                    + " WHERE timestamp <= NOW()";

            if (serverID != 0)
            {
                queryString += " AND server_id = @id";
            }

            queryString += $" AND timestamp >= @then GROUP BY timestamp) sub GROUP BY UNIX_TIMESTAMP(timestamp) DIV {divider}";

            GetActivityPeriodPastDate(period, ref parameters);

            return DB.GetList(queryString, parameters, Create.Activity);
        }

        public static List<MapActivity> GetMapActivity(int serverID, string period)
        {
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();

            if (serverID != 0)
            {
                parameters.Add("@id", serverID);
            }

            string queryString = "SELECT id, name, COUNT(*) as count FROM map INNER JOIN activity ON activity.map_id = map.id"
                + " WHERE agents > 0 AND timestamp <= NOW() AND timestamp >= @then";

            if (serverID != 0)
            {
                queryString += " AND server_id = @id";
            }

            GetActivityPeriodPastDate(period, ref parameters);

            queryString += " GROUP BY id";

            return DB.GetList(queryString, parameters, Create.MapActivity);
        }

        /// <summary>
        /// Retrieves the global activity profile
        /// </summary>
        /// <remarks>
        /// Period values:
        ///
        ///     day (or d)
        ///     week (or w)
        ///     month (or m)
        /// </remarks>
        /// <param name="period">(Optional) The period to fetch the activity for `(Default: day)`</param>
        /// <response code="200">Activity profile retrieved</response>
        /// <response code="404">Activity profile not found</response>
        /// <response code="500">Something broke internally</response>
        // GET: activity
        [HttpGet]
        [ProducesResponseType(typeof(List<Activity>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetActivity([FromQuery]string period)
        {
            List<Activity> activityEntries = GetActivity(0, period);

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
        /// Retrieves the global map popularity profile
        /// </summary>
        /// <remarks>
        /// Period values:
        ///
        ///     day (or d)
        ///     week (or w)
        ///     month (or m)
        /// </remarks>
        /// <param name="period">(Optional) The period to fetch the activity for `(Default: day)`</param>
        /// <response code="200">Map popularity profile retrieved</response>
        /// <response code="404">Map popularity profile not found</response>
        /// <response code="500">Something broke internally</response>
        // GET: activity/maps
        [HttpGet("maps")]
        [ProducesResponseType(typeof(List<MapActivity>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetMapActivity([FromQuery]string period)
        {
            List<MapActivity> mapActivityEntries = GetMapActivity(0, period);

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

        private static void GetActivityPeriodPastDate(string period, ref Dictionary<string, dynamic> parameters)
        {
            switch (period)
            {
                case "m":
                case "month":
                    parameters.Add("@then", DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                    break;
                case "w":
                case "week":
                    parameters.Add("@then", DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                    break;
                case "d":
                case "day":
                default:
                    parameters.Add("@then", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
                    break;
            }
        }
    }
}

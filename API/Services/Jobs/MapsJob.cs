// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Services.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using API.Models;
    using API.Utils;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Primitives;
    using Quartz;

    [DisallowConcurrentExecution]
    public class MapsJob : IJob
    {
        private static readonly Dictionary<int, MapStage> ConvertedStages = new Dictionary<int, MapStage>()
        {
            { 90, MapStage.Official_Release },
            { 85, MapStage.Featured_Release },
            { 80, MapStage.Featured_Update },
            { 75, MapStage.Polished_Release },
            { 70, MapStage.Polished_Update },
            { 55, MapStage.Playable_Release },
            { 50, MapStage.Playable_Update },
            { 30, MapStage.Training },
            { 20, MapStage.Silly_Other },
            { 15, MapStage.AUG_Only },
            { 10, MapStage.In_Dev }
        };

        private readonly IConfiguration config;

        public MapsJob(IConfiguration config)
        {
            this.config = config;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Dictionary<string, dynamic> insertParameters = new Dictionary<string, dynamic>();
            List<string> insertValues = new List<string>();

            // Fetch maps
            string mapsResponse = await Request.GetAsync(new Uri(this.config["Settings:SBGURL"] + "/intruder/dlc5/download/?platform=0")).ConfigureAwait(false);

            // Split map urls
            string[] mapsArray = mapsResponse.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < mapsArray.Length; i++)
            {
                // Split map url into url and parameters
                string mapUrl = mapsArray[i].Split('?')[0];

                // Get the map uid
                int uidStartIndex = mapUrl.IndexOf("/dlc5/", StringComparison.Ordinal) + 6;
                string uid = mapUrl.Substring(uidStartIndex, mapUrl.IndexOf(".ilf", StringComparison.Ordinal) - uidStartIndex);

                // Get author and map name from uid
                MapUID separatedUID = Util.GetSeparatedUID(uid);

                Uri uri = new Uri(mapsArray[i]);
                Dictionary<string, StringValues> queryParams = QueryHelpers.ParseQuery(uri.Query);

                int authorID = Util.GetMapAuthorID(separatedUID);

                // User doesn't exist
                if (authorID == 0)
                {
                    authorID = DB.Insert(
                        "INSERT INTO agent (name, role, avatar_url, status, current_location, last_update, last_seen, registered, flagged)"
                        + $" VALUES (@name, 'Agent', '{this.config["Settings:SBGURL"]}/intruder/img/intruder_n_120.png',"
                        + " 'Offline', 'Offline', NOW(), NOW(), NOW(), false)",
                        new Dictionary<string, dynamic>()
                        {
                                { "@name", separatedUID.Author }
                        },
                        true);
                }

                Map map = new Map()
                {
                    UID = uid,
                    Name = separatedUID.Name,
                    Author = new AgentProfile()
                    {
                        ID = authorID
                    },
                    URL = new Uri(mapUrl),
                    Version = int.Parse(queryParams["version_number"], CultureInfo.InvariantCulture),
                    Stage = ConvertMapStage(int.Parse(queryParams["stage"], CultureInfo.InvariantCulture)),
                    Played = int.Parse(queryParams["play_count"], CultureInfo.InvariantCulture),
                    HasFloorplan = false,
                    Images = 0,

                    // Great timestamp format right there, gg SBG
                    LastUpdate = DateTime.ParseExact(queryParams["timestamp"], "yyyy-MM-dd-HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None)
                };

                // Check whether the map is already in the database
                map.ID = DB.Get(
                    "SELECT id FROM map WHERE uid = @uid",
                    new Dictionary<string, dynamic>()
                    {
                        { "@uid", map.UID }
                    },
                    Create.IDOnly);

                // The map is already in the database
                if (map.ID != 0)
                {
                    DB.Update(
                        "UPDATE map SET uid = @uid, name = @name, author_id = @author,"
                            + " url = @url, version = @version, stage = @stage, played = @played,"
                            + " last_update = @lastUpdate WHERE id = @id",
                        new Dictionary<string, dynamic>()
                        {
                            { "@id", map.ID },
                            { "@uid", map.UID },
                            { "@name", map.Name },
                            { "@author", map.Author.ID },
                            { "@url", map.URL },
                            { "@version", map.Version },
                            { "@stage", map.Stage.ToString() },
                            { "@played", map.Played },
                            { "@lastUpdate", map.LastUpdate }
                        });
                }
                else
                {
                    // Add all query parameters
                    insertParameters.Add("@uid" + i, map.UID);
                    insertParameters.Add("@name" + i, map.Name);
                    insertParameters.Add("@author" + i, map.Author.ID);
                    insertParameters.Add("@url" + i, map.URL);
                    insertParameters.Add("@version" + i, map.Version);
                    insertParameters.Add("@stage" + i, map.Stage.ToString());
                    insertParameters.Add("@played" + i, map.Played);
                    insertParameters.Add("@lastUpdate" + i, map.LastUpdate);

                    IEnumerable<string> keys = insertParameters.Keys.Where(k => k.Contains(i.ToString(CultureInfo.InvariantCulture)));
                    insertValues.Add($"({string.Join(",", keys)})");
                }
            }

            // Bulk insert new maps
            if (insertParameters.Count > 0)
            {
                DB.Insert(
                    $"INSERT INTO map(uid, name, author_id, url, version, stage, played, last_update) VALUES {string.Join(",", insertValues)}",
                    insertParameters);
            }
        }

        private static MapStage ConvertMapStage(int stage)
        {
            if (ConvertedStages.ContainsKey(stage))
            {
                return ConvertedStages[stage];
            }

            return MapStage.Unknown;
        }
    }
}

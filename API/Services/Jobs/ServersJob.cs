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
    using API.Utils.Resolvers;
    using Microsoft.Extensions.Configuration;
    using MySql.Data.MySqlClient;
    using Newtonsoft.Json;
    using NLog;
    using Quartz;

    [DisallowConcurrentExecution]
    public class ServersJob : IJob
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string[] regions = { "US", "EU", "Asia", "AU", "Japan" };

        private readonly IConfiguration config;

        public ServersJob(IConfiguration config)
        {
            this.config = config;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            int currentlyPlaying = 0;
            DateTime syncActivityTime = DateTime.Now;
            List<Activity> activities = new List<Activity>();
            List<Server> servers = new List<Server>();
            Dictionary<string, dynamic> insertParameters = new Dictionary<string, dynamic>();
            List<string> insertValues = new List<string>();
            Dictionary<string, dynamic> serverIDParameters = new Dictionary<string, dynamic>();
            JsonSerializerSettings deserializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new InputResolver(typeof(Server)),
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            // Grab page to work on from DB
            int gameServerVersion = DB.Get("SELECT * FROM storage WHERE name = 'settings.game_server_version'", null, Create.StorageObject).Value;

            // Fetch the servers for each region
            await Task.WhenAll(this.regions.Select(async (region) =>
            {
                // Retrieve the server list
                string regionServersString = await Request.PostAsync(
                    new Uri(this.config["Settings:SBGURL"] + "/intruder/rooms/index.php"),
                    new Dictionary<string, string>()
                    {
                        { "list", "rooms" },
                        { "serverversion", gameServerVersion.ToString(CultureInfo.InvariantCulture) },
                        { "region", region }
                    }).ConfigureAwait(false);

                // Deserialize the server list
                ServerList regionServerlist = JsonConvert.DeserializeObject<ServerList>(regionServersString, deserializationSettings);

                // Add the deserialized servers to the total server list
                if (regionServerlist != null && regionServerlist.Servers.Count > 0)
                {
                    servers.AddRange(regionServerlist.Servers);
                }
            })).ConfigureAwait(false);

            // No servers, skip
            if (servers.Count == 0)
            {
                return;
            }

            // Grab all maps from the DB once
            Dictionary<string, int> mapIDMappings = DB.GetList("SELECT uid, id FROM map", null, CreateMap)
                .SelectMany(m => m)
                .ToDictionary(d => d.Key.ToUpperInvariant(), d => d.Value);

            for (int i = 0; i < servers.Count; i++)
            {
                // Skip servers that are running on an older version of intruder
                if (servers[i].Version < gameServerVersion)
                {
                    logger.Warn("Server list contains a server running on an old version");
                    continue;
                }

                servers[i].LastUpdate = syncActivityTime;

                // Keep track of the currently playing count
                currentlyPlaying += servers[i].Agents;

                // Used to keep track of still active servers for server tidying up
                serverIDParameters.Add("@id" + i, servers[i].ID);

                // Add query parameters
                insertParameters.Add("@id" + i, servers[i].ID);
                insertParameters.Add("@uuid" + i, servers[i].UUID);
                insertParameters.Add("@name" + i, servers[i].Name);
                insertParameters.Add("@fancyname" + i, servers[i].FancyName);
                insertParameters.Add("@description" + i, servers[i].Description);
                insertParameters.Add("@region" + i, servers[i].Region);
                insertParameters.Add("@type" + i, servers[i].Type);
                insertParameters.Add("@style" + i, servers[i].Style);
                insertParameters.Add("@version" + i, servers[i].Version);
                insertParameters.Add("@mapType" + i, servers[i].MapType);
                insertParameters.Add("@maxAgents" + i, servers[i].MaxAgents);
                insertParameters.Add("@maxSpectators" + i, servers[i].MaxSpectators);
                insertParameters.Add("@gamemode" + i, servers[i].Gamemode);
                insertParameters.Add("@timemode" + i, servers[i].Timemode);
                insertParameters.Add("@time" + i, servers[i].Time);
                insertParameters.Add("@inProgress" + i, servers[i].InProgress);
                insertParameters.Add("@ranked" + i, servers[i].Ranked);
                insertParameters.Add("@joinableBy" + i, servers[i].JoinableBy);
                insertParameters.Add("@visiblyBy" + i, servers[i].VisibleBy);
                insertParameters.Add("@passworded" + i, servers[i].Passworded);
                insertParameters.Add("@masterAgent" + i, servers[i].MasterAgent);
                insertParameters.Add("@agentNames" + i, servers[i].AgentNames);
                insertParameters.Add("@masterIP" + i, servers[i].MasterIP);
                insertParameters.Add("@serverIP" + i, servers[i].ServerIP);
                insertParameters.Add("@updateIP" + i, servers[i].UpdateIP);
                insertParameters.Add("@lastUpdate" + i, servers[i].LastUpdate);
                insertParameters.Add("@rules" + i, servers[i].Rules);
                insertParameters.Add("@tuning" + i, servers[i].Tuning);

                // Add query keys
                IEnumerable<string> keys = insertParameters.Keys.Where(k => k.Contains(i.ToString(CultureInfo.InvariantCulture)));
                insertValues.Add($"({string.Join(",", keys)})");

                // Create a new activity entry for the server
                int mapID = 0;
                if (mapIDMappings.ContainsKey(servers[i].Map.UID.ToUpperInvariant()))
                {
                    mapID = mapIDMappings[servers[i].Map.UID.ToUpperInvariant()];
                }
                else
                {
                    MapUID separatedUID = Util.GetSeparatedUID(servers[i].Map.UID);

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

                    mapID = DB.Insert(
                        "INSERT INTO map(uid, name, author_id, url, version, stage, played, last_update)"
                            + $" VALUES ('{servers[i].Map.UID}', '{separatedUID.Name}', '{authorID}', 'https://intruder-db.info/maps/not-found', 1, '{MapStage.Unknown.ToString()}', 0, NOW())",
                        null,
                        true);
                }

                activities.Add(CreateActivityEntry(servers[i], mapID, syncActivityTime));
            }

            // Bulk insert all servers
            DB.Insert(
                    "INSERT INTO server(id, uuid, name, fancy_name, description, region, type, style, version, map_type, max_agents, max_spectators, gamemode, timemode, time,"
                    + " in_progress, ranked, joinable_by, visible_by, passworded, master_agent, agent_names, master_ip, server_ip, update_ip, last_update, rules, tuning)"
                    + $" VALUES {string.Join(",", insertValues)}"
                    + "ON DUPLICATE KEY UPDATE name = VALUES(name), fancy_name = VALUES(fancy_name), description = VALUES(description), region = VALUES(region), type = VALUES(type),"
                    + " style = VALUES(style), version = VALUES(version), map_type = VALUES(map_type), max_agents = VALUES(max_agents), max_spectators = VALUES(max_spectators),"
                    + " gamemode = VALUES(gamemode), timemode = VALUES(timemode), time = VALUES(time), in_progress = VALUES(in_progress), ranked = VALUES(ranked),"
                    + " joinable_by = VALUES(joinable_by), visible_by = VALUES(visible_by), passworded = VALUES(passworded), master_agent = VALUES(master_agent),"
                    + " agent_names = VALUES(agent_names), master_ip = VALUES(master_ip), server_ip = VALUES(server_ip), update_ip = VALUES(update_ip),"
                    + " last_update = VALUES(last_update), rules = VALUES(rules), tuning = VALUES(tuning)",
                    insertParameters);

            // Insert the new activity entries
            InsertActivityEntries(activities);

            // Update currently playing
            UpdateCurrentlyPlayingStats(currentlyPlaying);

            // Check whether we have a new alltime peak
            // If yes, update, else skip
            if (currentlyPlaying > 0)
            {
                UpdateAlltimePeakStats(currentlyPlaying);
            }

            // Remove old servers (agent-created temporary rooms) from the DB
            TidyUpOldServers(serverIDParameters);
        }

        private static Activity CreateActivityEntry(Server server, int mapID, DateTime timestamp)
        {
            return new Activity()
            {
                ServerID = server.ID,
                MapID = mapID,
                Agents = server.Agents,
                Timestamp = timestamp
            };
        }

        private static void InsertActivityEntries(List<Activity> entries)
        {
            Dictionary<string, dynamic> queryParameters = new Dictionary<string, dynamic>();
            List<string> queryValueGroups = new List<string>();

            for (int i = 0; i < entries.Count; i++)
            {
                queryParameters.Add("@sid" + i, entries[i].ServerID);
                queryParameters.Add("@mid" + i, entries[i].MapID);
                queryParameters.Add("@agents" + i, entries[i].Agents);
                queryParameters.Add("@timestamp" + i, entries[i].Timestamp);

                IEnumerable<string> keys = queryParameters.Keys.Where(k => k.Contains(i.ToString(CultureInfo.InvariantCulture)));
                queryValueGroups.Add($"({"@sid" + i},{"@mid" + i},{"@agents" + i},{"@timestamp" + i})");
            }

            DB.Insert($"INSERT INTO activity(server_id, map_id, agents, timestamp) VALUES {string.Join(",", queryValueGroups.ToArray())}", queryParameters);
        }

        private static void UpdateCurrentlyPlayingStats(int currentlyPlaying)
        {
            Dictionary<string, dynamic> queryParameters = new Dictionary<string, dynamic>
            {
                { "@name", "currently_playing" },
                { "@value", currentlyPlaying }
            };

            DB.Update("INSERT INTO stats(name, value) VALUES (@name, @value) ON DUPLICATE KEY UPDATE value = VALUES(value)", queryParameters);
        }

        private static void UpdateAlltimePeakStats(int currentlyPlaying)
        {
            int alltimePeak = DB.Get("SELECT * FROM stats WHERE name = 'alltime_peak'", null, CreateAlltimePeakStats);

            if (alltimePeak >= currentlyPlaying)
            {
                return;
            }

            Dictionary<string, dynamic> queryParameters = new Dictionary<string, dynamic>
            {
                { "@name", "alltime_peak" },
                { "@value", currentlyPlaying.ToString(CultureInfo.InvariantCulture) }
            };

            DB.Insert("INSERT INTO stats(name, value) VALUES (@name, @value) ON DUPLICATE KEY UPDATE value = VALUES(value)", queryParameters);
        }

        private static void TidyUpOldServers(Dictionary<string, dynamic> serverIDs)
        {
            DB.Delete($"DELETE FROM server WHERE id NOT IN ({string.Join(", ", serverIDs.Keys)})", serverIDs);
        }

        private static Dictionary<string, int> CreateMap(MySqlDataReader reader)
        {
            return new Dictionary<string, int>()
            {
                { reader.GetString("uid"), reader.GetInt32("id") }
            };
        }

        private static int CreateAlltimePeakStats(MySqlDataReader reader)
        {
            string value = reader.GetString("value");
            return value.Length == 0 ? 0 : int.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}

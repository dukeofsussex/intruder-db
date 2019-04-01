// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Services.Jobs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using API.Models;
    using API.Utils.Resolvers;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using NLog;
    using Quartz;

    [DisallowConcurrentExecution]
    public class OnlineAgentsJob : IJob
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly IConfiguration config;

        public OnlineAgentsJob(IConfiguration config)
        {
            this.config = config;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            int counter = 0;
            Dictionary<string, dynamic> updateParameters = new Dictionary<string, dynamic>();

            string onlineAgents = await Request.GetAsync(new Uri(this.config["Settings:SBGURL"] + "/intruder/rooms/?get=online")).ConfigureAwait(false);

            OnlineAgentList onlineAgentList = JsonConvert.DeserializeObject<OnlineAgentList>(onlineAgents, new JsonSerializerSettings
            {
                ContractResolver = new InputResolver(typeof(OnlineAgent)),
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            if (onlineAgentList == null)
            {
                logger.Info("No agents online, aborting...");
                return;
            }

            foreach (OnlineAgent agent in onlineAgentList.Agents)
            {
                Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>()
                {
                    { "@name", agent.Name },
                    { "@currentLocation", agent.CurrentLocation },
                    { "@status", Status.Online }
                };

                int storedAgentID = DB.Get(
                    "SELECT id FROM agent WHERE name = @name",
                    parameters,
                    Create.IDOnly);

                if (storedAgentID == 0)
                {
                    DB.Insert(
                        "INSERT INTO agent (name, role, avatar_url, status, current_location, last_update, last_seen, registered, flagged)"
                        + $" VALUES (@name, 'Agent', '{this.config["Settings:SBGURL"]}/intruder/img/intruder_n_120.png',"
                        + " 'Online', @currentLocation, NOW(), NOW(), NOW(), false)",
                        parameters);
                }
                else
                {
                    parameters.Add("@id", storedAgentID);

                    DB.Update("UPDATE agent SET current_location = @currentLocation, status = @status, last_seen = NOW() WHERE id = @id", parameters);
                }

                updateParameters.Add("@name" + counter, agent.Name);
                counter += 1;
            }

            DB.Update(
                $"UPDATE agent SET current_location = 'offline', status = 'Offline' WHERE name NOT IN ({string.Join(",", updateParameters.Select(m => m.Key).ToList())})",
                updateParameters);
        }
    }
}

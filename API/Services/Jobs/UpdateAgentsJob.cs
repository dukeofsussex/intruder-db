// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Services.Jobs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using API.Models;
    using Microsoft.Extensions.Configuration;
    using NLog;
    using Quartz;

    [DisallowConcurrentExecution]
    public class UpdateAgentsJob : IJob
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly AgentJobs agentJobs;

        private readonly IConfiguration config;

        public UpdateAgentsJob(AgentJobs agentJobs, IConfiguration config)
        {
            this.agentJobs = agentJobs;
            this.config = config;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            List<AgentJobDetails> agents = DB.GetList(
                "SELECT id, name, flagged FROM agent WHERE last_seen >= DATE_SUB(NOW(), INTERVAL 15 MINUTE)"
                    + " AND last_update < DATE_SUB(NOW(), INTERVAL 15 MINUTE) AND flagged = false",
                null,
                Create.AgentJobDetails);

            if (agents == null)
            {
                logger.Info("No agents to update, aborting...");
                return;
            }

            logger.Debug("Processing " + agents.Count + " agents...");

            using (SemaphoreSlim concurrencySemaphore = new SemaphoreSlim(5))
            {
                await Task.WhenAll(agents.Select(async (agent, index) =>
                {
                    await concurrencySemaphore.WaitAsync().ConfigureAwait(false);

                    await this.agentJobs.GetAgentAsync(agent.ID, agent.Name, agent.Flagged).ConfigureAwait(false);

                    concurrencySemaphore.Release();
                })).ConfigureAwait(false);
            }
        }
    }
}

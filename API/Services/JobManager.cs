// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Services
{
    using API.Services.Jobs;
    using NLog;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Impl.Matchers;

    public class JobManager : IJobManager
    {
        private static IScheduler scheduler;

        private static Logger logger = LogManager.GetLogger("JobManager");

        private readonly JobFactory jobFactory;

        public JobManager(JobFactory jobFactory)
        {
            this.jobFactory = jobFactory;
        }

        public async void StartAsync()
        {
            StdSchedulerFactory schedulerFactory = new StdSchedulerFactory();
            scheduler = await schedulerFactory.GetScheduler().ConfigureAwait(false);
            scheduler.JobFactory = this.jobFactory;

            IJobDetail forumMembersJob = JobBuilder.Create<ForumMembersJob>()
                .WithIdentity("forumMembersJob", "MainGroup")
                .Build();

            IJobDetail forumPMJob = JobBuilder.Create<ForumPMJob>()
                .WithIdentity("forumPMJob", "MainGroup")
                .Build();

            IJobDetail mapsJob = JobBuilder.Create<MapsJob>()
                .WithIdentity("mapsJob", "MainGroup")
                .Build();

            IJobDetail onlineAgentsJob = JobBuilder.Create<OnlineAgentsJob>()
                .WithIdentity("onlineAgentsJob", "MainGroup")
                .Build();

            IJobDetail serversJob = JobBuilder.Create<ServersJob>()
                .WithIdentity("serversJob", "MainGroup")
                .Build();

            IJobDetail updateAgentsJob = JobBuilder.Create<UpdateAgentsJob>()
                .WithIdentity("updateAgentsJob", "MainGroup")
                .Build();

            ITrigger forumMembersTrigger = TriggerBuilder.Create()
                .WithIdentity("forumMembersTrigger", "MainGroup")
                .WithCronSchedule("0 0/30 * 1/1 * ?")
                .ForJob("forumMembersJob", "MainGroup")
                .Build();

            ITrigger forumPMTrigger = TriggerBuilder.Create()
                .WithIdentity("forumPMTrigger", "MainGroup")
                .WithCronSchedule("0/30 * * * * ?")
                .ForJob("forumPMJob", "MainGroup")
                .Build();

            ITrigger mapsTrigger = TriggerBuilder.Create()
                .WithIdentity("mapsTrigger", "MainGroup")
                .WithCronSchedule("0 0 0/1 1/1 * ?")
                .ForJob("mapsJob", "MainGroup")
                .Build();

            ITrigger onlineAgentsTrigger = TriggerBuilder.Create()
                .WithIdentity("onlineAgentsTrigger", "MainGroup")
                .WithCronSchedule("0 0/1 * 1/1 * ?")
                .ForJob("onlineAgentsJob", "MainGroup")
                .Build();

            ITrigger serversTrigger = TriggerBuilder.Create()
                .WithIdentity("serversTrigger", "MainGroup")
                .WithCronSchedule("0 0/1 * 1/1 * ?")
                .ForJob("serversJob", "MainGroup")
                .Build();

            ITrigger updateAgentsTrigger = TriggerBuilder.Create()
                .WithIdentity("updateAgentsTrigger", "MainGroup")
                .WithCronSchedule("0 0/15 * 1/1 * ?")
                .ForJob("updateAgentsJob", "MainGroup")
                .Build();

            await scheduler.ScheduleJob(forumMembersJob, forumMembersTrigger).ConfigureAwait(false);
            await scheduler.ScheduleJob(forumPMJob, forumPMTrigger).ConfigureAwait(false);
            await scheduler.ScheduleJob(mapsJob, mapsTrigger).ConfigureAwait(false);
            await scheduler.ScheduleJob(onlineAgentsJob, onlineAgentsTrigger).ConfigureAwait(false);
            await scheduler.ScheduleJob(updateAgentsJob, updateAgentsTrigger).ConfigureAwait(false);
            await scheduler.ScheduleJob(serversJob, serversTrigger).ConfigureAwait(false);

            scheduler.ListenerManager.AddJobListener(new JobMonitor(), GroupMatcher<JobKey>.GroupEquals("MainGroup"));

            logger.Info("Starting job scheduler...");
            await scheduler.Start().ConfigureAwait(false);
        }

        public async void StopAsync()
        {
            logger.Info("Stopping job scheduler...");
            await scheduler.Shutdown(true).ConfigureAwait(false);
        }
    }
}

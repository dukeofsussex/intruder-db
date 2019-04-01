// Copyright (c) Chris Satchell. All rights reserved.

namespace API.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using NLog;
    using Quartz;

    public class JobMonitor : IJobListener
    {
        private const string JobMonName = "MainGroupMonitor";

        private static Logger logger = LogManager.GetLogger(JobMonName);

        public string Name
        {
            get { return JobMonName; }
        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.Warn($"Job {context.JobDetail.Key} execution vetoed");
            return Task.FromResult(0);
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            logger.Info($"Launching job {context.JobDetail.Key}...");
            return Task.FromResult(0);
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (jobException != null)
            {
                logger.Error($"Job {context.JobDetail.Key} threw a new exception: {jobException}");
            }
            else
            {
                logger.Info($"Job {context.JobDetail.Key} completed in {context.JobRunTime.TotalSeconds}s");
            }

            return Task.FromResult(0);
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.Services
{
    public class BackgroundJobService : BackgroundService
    {
        private readonly ILogger<BackgroundJobService> logger;
        private readonly IServiceProvider serviceProvider;

        public BackgroundJobService(ILogger<BackgroundJobService> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var repoService = (IRepoService)scope.ServiceProvider.GetRequiredService(typeof(IRepoService));
                    List<SortJob> pendingJobs = (await repoService.GetAllJobsAsync("Jobs")).FindAll(x => x.Status == SortJobStatus.Pending);

                    this.logger.LogInformation($"Background Worker ran at {DateTime.Today.TimeOfDay}. There are {pendingJobs.Count} pending sort jobs");

                    foreach (SortJob job in pendingJobs)
                    {
                        logger.LogInformation($"Processing job with ID {job.Id}");
                        var stopwatch = Stopwatch.StartNew();
                        var output = job.Input.OrderBy(n => n).ToArray();
                        Thread.Sleep(60000);
                        var duration = stopwatch.Elapsed;

                        SortJob sortJob = new SortJob(
                        id: job.Id,
                        status: SortJobStatus.Completed,
                        duration: duration,
                        input: job.Input,
                        output: output);

                        logger.LogInformation("Completed processing job with ID '{JobId}'. Duration: '{Duration}'.", job.Id, duration);

                        var result = await repoService.UpdateJobAsync("Jobs", sortJob);
                        if (result)
                        {
                            logger.LogInformation($"Database succesfully updated for JobId {sortJob.Id}");
                        }
                        else
                        {
                            logger.LogInformation($"There was some problem while updating the database for JobId: {sortJob.Id}");
                        }

                    }
                }
            }
        }
    }
}

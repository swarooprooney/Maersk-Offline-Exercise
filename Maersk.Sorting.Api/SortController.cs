using Maersk.Sorting.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.Controllers
{
    [ApiController]
    [Route("sort")]
    public class SortController : ControllerBase
    {
        private readonly ISortJobProcessor _sortJobProcessor;
        private readonly IRepoService repoService;

        public SortController(ISortJobProcessor sortJobProcessor, IRepoService repoService)
        {
            _sortJobProcessor = sortJobProcessor;
            this.repoService = repoService;
        }

        [HttpPost("run")]
        [Obsolete("This executes the sort job asynchronously. Use the asynchronous 'EnqueueJob' instead.")]
        public async Task<ActionResult<SortJob>> EnqueueAndRunJob(int[] values)
        {
            var pendingJob = new SortJob(
                id: Guid.NewGuid(),
                status: SortJobStatus.Pending,
                duration: null,
                input: values,
                output: null);

            var completedJob = await _sortJobProcessor.Process(pendingJob);

            return Ok(completedJob);
        }

        [HttpPost]
        public async Task<ActionResult<SortJob>> EnqueueJob([FromBody]int[] values)
        {
            if (values == null) return BadRequest();

            var pendingJob = new SortJob(
                id: Guid.NewGuid(),
                status: SortJobStatus.Pending,
                duration: null,
                input: values,
                output: null);
            var result = await repoService.EnqueueJobAsync("Jobs", pendingJob);
            if (result)
            {
                return Ok();
            }
            return StatusCode(500, "Unable to schedule the job, please try again after sometime");
        }

        [HttpGet]
        public async Task<ActionResult<SortJob[]>> GetJobs()
        {
            // TODO: Should return all jobs that have been enqueued (both pending and completed).
            var result = await repoService.GetAllJobsAsync("Jobs");
            return Ok(result);
        }

        [HttpGet("{jobId}")]
        public async Task<ActionResult<SortJob>> GetJob(Guid jobId)
        {
            if (jobId==null)
            {
                return BadRequest();
            }
            var result = await repoService.GetJobByIdAsync("Jobs", jobId);
            if (result!=null)
            {
                return Ok(result);
            }
            return NotFound($"The Job with Job Id: {jobId} is not found");
        }
    }
}

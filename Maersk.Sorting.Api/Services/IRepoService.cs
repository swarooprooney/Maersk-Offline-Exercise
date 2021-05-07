using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.Services
{
    public interface IRepoService
    {
        Task<bool> EnqueueJobAsync(string table, SortJob sortJob);
        Task<List<SortJob>> GetAllJobsAsync(string table);
        Task<SortJob> GetJobByIdAsync(string table,Guid id);
        Task<bool> UpdateJobAsync(string table,SortJob sortJob);
    }
}
using Maersk.Sorting.Api.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.Services
{
    public class RepoService : IRepoService
    {
        private readonly IDatabaseWrapper databaseWrapper;

        public RepoService(IDatabaseWrapper databaseWrapper)
        {
            this.databaseWrapper = databaseWrapper;
        }

        public async Task<bool> EnqueueJobAsync(string tableName, SortJob sortJob)
        {
            return await databaseWrapper.TryInsertNewRecordAsync(tableName, sortJob);
        }

        public async Task<List<SortJob>> GetAllJobsAsync(string table)
        {
            return await databaseWrapper.GetAllRecordsAsync<SortJob>(table);
        }

        public async Task<SortJob> GetJobByIdAsync(string table, Guid id)
        {
            return await databaseWrapper.GetRecordByIdAsync<SortJob>(table,id);
        }

        public async Task<bool> UpdateJobAsync(string table, SortJob sortJob)
        {
            return await databaseWrapper.UpdateRecord(table, sortJob.Id, sortJob);
        }
    }
}

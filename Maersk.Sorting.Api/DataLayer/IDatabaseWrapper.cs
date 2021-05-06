using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.DataLayer
{
    public interface IDatabaseWrapper
    {
        Task<List<T>> GetAllRecordsAsync<T>(string table);
        Task<T> GetRecordByIdAsync<T>(string table, Guid guid);
        Task<bool> TryInsertNewRecordAsync<T>(string table, T record);
    }
}
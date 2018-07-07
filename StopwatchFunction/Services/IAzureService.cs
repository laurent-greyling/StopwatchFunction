using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using StopwatchFunction.Entities;

namespace StopwatchFunction.Services
{
    public interface IAzureService
    {
        IEnumerable<UserDetailsEntity> RetrieveEntities(UserDetailsEntity userDetailsEntity);
        Task<TableResult> RetrieveEntityAsync(UserDetailsEntity userdetailsEntity);

        Task UpdateTableAsync(StopwatchEntity entity);

        Task AddMessageAsync(StopwatchEntity entity);
    }
}

using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using StopwatchFunction.Entities;
using StopwatchFunction.Helpers;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Collections.Generic;
using StopwatchFunction.Models;
using System.Linq;

namespace StopwatchFunction.Services
{
    public class StopwatchDetails : IStopwatchDetails
    {
        private CloudTable _table;
        private CloudQueue _queue;

        public StopwatchDetails()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureWebJobsStorage"));

            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("stopwatchdetails");

            var queueClient = storageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference("stopwatchqueue");

            _queue.CreateIfNotExists();
            _table.CreateIfNotExists();
        }

        public async Task Save(StopwatchEntity stopwatchEntity)
        {
            try
            {
                await UpdateTableAsync(stopwatchEntity);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }            
        }

        public async Task<List<UserDetailsModel>> Retrieve(UserDetailsEntity userDetailsEntity)
        {
            try
            {
                var entityList = new List<UserDetailsModel>();
                if (!string.IsNullOrEmpty(userDetailsEntity.UserName) && !string.IsNullOrEmpty(userDetailsEntity.StopWatchName))
                {                    
                    var userEntity = await RetrieveEntityAsync(userDetailsEntity);
                    entityList.Add(new UserDetailsModel
                    {
                        UserName = ((UserDetailsEntity)userEntity.Result).UserName,
                        StopWatchName = ((UserDetailsEntity)userEntity.Result).StopWatchName,
                        Status = ((UserDetailsEntity)userEntity.Result).Status,
                        ElapsedTime = ((UserDetailsEntity)userEntity.Result).ElapsedTime
                    });
                }

                if (string.IsNullOrEmpty(userDetailsEntity.StopWatchName))
                {
                    var entities = RetrieveEntities(userDetailsEntity);
                    entityList.AddRange(entities.Select(x => new UserDetailsModel
                    {
                        UserName = x.UserName,
                        StopWatchName = x.StopWatchName,
                        Status = x.Status,
                        ElapsedTime = x.ElapsedTime
                    }).ToList());
                }

                return entityList;
            } 
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }
        }

        private IEnumerable<UserDetailsEntity> RetrieveEntities(UserDetailsEntity userDetailsEntity)
        {
            var query = new TableQuery<UserDetailsEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userDetailsEntity.UserName.ToLower().Replace(" ", "-")));

            return _table.ExecuteQuery(query);
        }

        private async Task<TableResult> RetrieveEntityAsync(UserDetailsEntity userdetailsEntity)
        {
            var retrieveOperation = TableOperation.Retrieve<UserDetailsEntity>(
                userdetailsEntity.UserName.ToLower().Replace(" ", "-"),
                userdetailsEntity.StopWatchName.ToLower().Replace(" ", "-"));
            return await _table.ExecuteAsync(retrieveOperation);
        }

        private async Task UpdateTableAsync(StopwatchEntity entity)
        {
            entity.PartitionKey = entity.UserName.ToLower().Replace(" ", "-");
            entity.RowKey = entity.StopWatchName.ToLower().Replace(" ", "-");
            entity.StartTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);

            if (entity.Start)
            {
                entity.Status = StopwatchStatus.Created.ToString();
            }
            if (entity.Restart)
            {
                entity.Status = StopwatchStatus.Restart.ToString();
            }
            if (entity.Stop)
            {
                entity.Status = StopwatchStatus.Stop.ToString();
            }
            if (entity.Reset)
            {
                entity.Status = StopwatchStatus.Reset.ToString();
            }

            var operation = TableOperation.InsertOrMerge(entity);
            await _table.ExecuteAsync(operation);

            await AddMessageAsync(entity);
        }

        private async Task AddMessageAsync(StopwatchEntity entity)
        {
            var messageString = JsonConvert.SerializeObject(entity);
            var message = new CloudQueueMessage(messageString);
            await _queue.AddMessageAsync(message);
        }

    }
}

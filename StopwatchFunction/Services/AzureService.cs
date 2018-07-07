using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using StopwatchFunction.Entities;
using StopwatchFunction.Helpers;

namespace StopwatchFunction.Services
{
    public class AzureService : IAzureService
    {
        private readonly CloudTable _table;
        private readonly CloudQueue _queue;
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss.fff";

        private readonly ElapsedTime _elapsedTime;

        public AzureService()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureWebJobsStorage"));

            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference("stopwatchdetails");

            var queueClient = storageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference("stopwatchqueue");

            _queue.CreateIfNotExists();
            _table.CreateIfNotExists();
            _elapsedTime = new ElapsedTime();
        }

        public IEnumerable<UserDetailsEntity> RetrieveEntities(UserDetailsEntity userDetailsEntity)
        {
            var query = new TableQuery<UserDetailsEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userDetailsEntity.UserName.ToLower().Replace(" ", "-")));

            return _table.ExecuteQuery(query);
        }

        public async Task<TableResult> RetrieveEntityAsync(UserDetailsEntity userdetailsEntity)
        {
            var retrieveOperation = TableOperation.Retrieve<UserDetailsEntity>(
                userdetailsEntity.UserName.ToLower().Replace(" ", "-"),
                userdetailsEntity.StopWatchName.ToLower().Replace(" ", "-"));
            return await _table.ExecuteAsync(retrieveOperation);
        }

        public async Task UpdateTableAsync(StopwatchEntity entity)
        {
            entity.PartitionKey = entity.UserName.ToLower().Replace(" ", "-");
            entity.RowKey = entity.StopWatchName.ToLower().Replace(" ", "-");
            var entityStatus = await RetrieveEntityAsync(MapEntity(entity));
            var dateTime = DateTime.UtcNow.ToString(DateFormat, CultureInfo.InvariantCulture);

            if (entity.Start)
            {
                if (entityStatus.Result != null 
                    && ((UserDetailsEntity)entityStatus.Result).Status == StopwatchStatus.Stop.ToString())
                {
                    entity.Status = StopwatchStatus.Start.ToString();
                    //Need to set startdate to now and add the time that was on clock when stopped. This will then handle a stop start situation, where
                    //clock was not restarted or reset.
                    var elapsedTime =
                        _elapsedTime.CalculateElapsedTime(((UserDetailsEntity) entityStatus.Result).EndTime,
                            ((UserDetailsEntity) entityStatus.Result).StartTime);
                    var timeSpan = TimeSpan.Parse(elapsedTime);
                    entity.StartTime = DateTime.UtcNow.Add(timeSpan).ToString(DateFormat);
                }
                else
                {
                    entity.Status = StopwatchStatus.Created.ToString();
                    entity.StartTime = dateTime;
                }
            }
            if (entity.Restart)
            {
                entity.Status = StopwatchStatus.Restart.ToString();
                entity.StartTime = dateTime;
            }
            if (entity.Stop)
            {
                entity.Status = StopwatchStatus.Stop.ToString();
                entity.EndTime = dateTime;
            }
            else
            {
                entity.EndTime = string.Empty;
            }
            if (entity.Reset)
            {
                entity.Status = StopwatchStatus.Reset.ToString();
                entity.StartTime = dateTime;
            }

            var operation = TableOperation.InsertOrMerge(entity);
            await _table.ExecuteAsync(operation);

            //This is only for when I run agains the webjob. The qebjob has a queue trigger
            //await AddMessageAsync(entity);
        }

        public async Task AddMessageAsync(StopwatchEntity entity)
        {
            var messageString = JsonConvert.SerializeObject(entity);
            var message = new CloudQueueMessage(messageString);
            await _queue.AddMessageAsync(message);
        }

        private UserDetailsEntity MapEntity(StopwatchEntity entity)
        {
            return new UserDetailsEntity
            {
                UserName = entity.UserName,
                StopWatchName = entity.StopWatchName
            };
        }
    }
}

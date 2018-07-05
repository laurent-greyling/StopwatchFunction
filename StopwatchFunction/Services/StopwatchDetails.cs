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

namespace StopwatchFunction.Services
{
    public class StopwatchDetails : IStopwatchDetails
    {
        public async Task Save(StopwatchEntity stopwatchEntity)
        {
            try
            {
                var operationsConnectionString = CloudConfigurationManager.GetSetting("AzureWebJobsStorage");
                var operationsStorageAccount =
                    CloudStorageAccount.Parse(operationsConnectionString);

                await UpdateTableAsync(operationsStorageAccount, stopwatchEntity);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }            
        }

        private bool IsEntityAvailable(CloudTable table, StopwatchEntity stopwatchEntity)
        {
            var retrieveOperation = TableOperation.Retrieve<StopwatchEntity>(stopwatchEntity.PartitionKey, stopwatchEntity.RowKey);
            var retrievedResult = table.Execute(retrieveOperation);
            return retrievedResult.Result != null;
        }

        private async Task UpdateTableAsync(CloudStorageAccount storageAccount, StopwatchEntity entity)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("stopwatchdetails");
            table.CreateIfNotExists();

            entity.PartitionKey = entity.UserName.ToLower().Replace(" ", "-");
            entity.RowKey = entity.StopWatchName.ToLower().Replace(" ", "-");
            entity.StartTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            entity.Status = IsEntityAvailable(table, entity) ? StopwatchStatus.Reset.ToString() : StopwatchStatus.Created.ToString();

            var operation = TableOperation.InsertOrMerge(entity);
            await table.ExecuteAsync(operation);

            await AddMessageAsync(storageAccount, entity);
        }

        private async Task AddMessageAsync(CloudStorageAccount storageAccount, StopwatchEntity entity)
        {
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("stopwatchqueue");
            queue.CreateIfNotExists();

            var messageString = JsonConvert.SerializeObject(entity);
            var message = new CloudQueueMessage(messageString);
            await queue.AddMessageAsync(message);
        }
    }
}

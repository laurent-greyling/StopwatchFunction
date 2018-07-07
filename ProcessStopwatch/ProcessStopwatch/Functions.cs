using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using ProcessStopwatch.Entities;
using ProcessStopwatch.Helpers;

namespace ProcessStopwatch
{
    public class Functions
    {
        private static Stopwatch _sw;
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static async Task ProcessQueueMessage([QueueTrigger("stopwatchqueue")] string message, TextWriter log)
        {
            var userData = JsonConvert.DeserializeObject<StopwatchEntity>(message);

            if (userData.Status == StopwatchStatus.Created.ToString())
            {
                _sw = new Stopwatch();
                _sw.Start();
            }

            if (userData.Status == StopwatchStatus.Restart.ToString())
            {
                _sw.Restart();
            }

            if (userData.Status == StopwatchStatus.Stop.ToString())
            {
                _sw.Stop();
            }

            if (userData.Status == StopwatchStatus.Reset.ToString())
            {
                _sw.Reset();
            }

            var operationsConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            var operationsStorageAccount =
                CloudStorageAccount.Parse(operationsConnectionString);

            if (!_sw.IsRunning) return;
            userData.Status = StopwatchStatus.Running.ToString();

            while (_sw.IsRunning)
            {
                userData.ElapsedTime = _sw.Elapsed.ToString();
                await UpdateTable(operationsStorageAccount, userData);
            }
        }

        private static async Task UpdateTable(CloudStorageAccount storageAccount, StopwatchEntity entity)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("stopwatchdetails");

            var retrieveEntity = TableOperation.Retrieve<StopwatchEntity>(
                entity.UserName.ToLower().Replace(" ", "-"),
                entity.StopWatchName.ToLower().Replace(" ", "-")
            );
            var updateEntity = await table.ExecuteAsync(retrieveEntity);

            var newEntity = new StopwatchEntity
            {
                PartitionKey = ((StopwatchEntity)updateEntity.Result).PartitionKey,
                RowKey = ((StopwatchEntity)updateEntity.Result).RowKey,
                UserName = ((StopwatchEntity)updateEntity.Result).UserName,
                StopWatchName = ((StopwatchEntity)updateEntity.Result).StopWatchName,
                Status = entity.Status,
                ElapsedTime = entity.ElapsedTime
            };

            var operation = TableOperation.InsertOrMerge(newEntity);
            table.Execute(operation);
        }
    }
}

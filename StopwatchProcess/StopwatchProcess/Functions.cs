using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using StopwatchProcess.Entities;
using StopwatchProcess.Helpers;

namespace StopwatchProcess
{
    public class Functions
    {
        private static Stopwatch _sw;
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("stopwatchqueue")] string message, TextWriter log)
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
            var queueClient = operationsStorageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("elapsedtimequeue");
            queue.CreateIfNotExists();

            userData.Status = StopwatchStatus.Running.ToString();            

            while (_sw.IsRunning)
            {
                userData.ElapsedTime = _sw.Elapsed.ToString();
                UpdateTable(operationsStorageAccount, userData);
            }
        }

        private static void UpdateTable(CloudStorageAccount storageAccount, StopwatchEntity entity)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("stopwatchdetails");

            var operation = TableOperation.InsertOrMerge(entity);
            table.Execute(operation);
        }
    }
}

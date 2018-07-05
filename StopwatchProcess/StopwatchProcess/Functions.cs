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
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("stopwatchqueue")] string message, TextWriter log)
        {
            var userData = JsonConvert.DeserializeObject<StopwatchEntity>(message);

            var sw = new Stopwatch();

            if (userData.Status == StopwatchStatus.Created.ToString())
            {
                sw.Start();
            }

            if (userData.Status == StopwatchStatus.Reset.ToString())
            {
                sw.Reset();
            }

            if (userData.Status == StopwatchStatus.Stop.ToString())
            {
                sw.Stop();
            }

            var operationsConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
            var operationsStorageAccount =
                CloudStorageAccount.Parse(operationsConnectionString);
            var queueClient = operationsStorageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("elapsedtimequeue");
            queue.CreateIfNotExists();

            userData.Status = StopwatchStatus.Running.ToString();
            userData.ElapsedTime = sw.Elapsed.ToString();

            //Needs some work.
            var messageString = JsonConvert.SerializeObject(userData);
            var queueMessage = new CloudQueueMessage(messageString);
            queue.AddMessage(queueMessage);

            var messageToUpdate = queue.GetMessage();

            UpdateTable(operationsStorageAccount, userData);

            while (sw.IsRunning)
            {
                userData.ElapsedTime = sw.Elapsed.ToString();

                messageString = JsonConvert.SerializeObject(userData);

                messageToUpdate.SetMessageContent(messageString);
                queue.UpdateMessage(messageToUpdate, TimeSpan.FromSeconds(1.0),
                    MessageUpdateFields.Content | MessageUpdateFields.Visibility);
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

using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using StopwatchFunction.Entities;
using StopwatchFunction.Helpers;
using System;

namespace StopwatchFunction.Services
{
    public class StopwatchDetails : IStopwatchDetails
    {
        public void Save(StopwatchEntity stopwatchEntity)
        {
            try
            {
                var operationsConnectionString = CloudConfigurationManager.GetSetting("AzureWebJobsStorage");
                var operationsStorageAccount =
                    CloudStorageAccount.Parse(operationsConnectionString);

                var tableClient = operationsStorageAccount.CreateCloudTableClient();
                var table = tableClient.GetTableReference("stopwatchdetails");

                table.CreateIfNotExists();

                stopwatchEntity.PartitionKey = stopwatchEntity.UserName.ToLower().Replace(" ", "-");
                stopwatchEntity.RowKey = stopwatchEntity.StopWatchName.ToLower().Replace(" ", "-");

                stopwatchEntity.Status = IsEntityAvailable(table, stopwatchEntity) ? StopwatchStatus.Reset.ToString() : StopwatchStatus.Created.ToString();

                var operation = TableOperation.InsertOrMerge(stopwatchEntity);

                table.Execute(operation);
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
            return retrievedResult.Result != null ? true : false;
        }
    }
}

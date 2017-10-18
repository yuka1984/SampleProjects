using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace FunctionSingletonTest
{
    public static class TriggerTest1
    {
        private static Lazy<Guid> lazyDateTime = new Lazy<Guid>(() => Guid.NewGuid());

        [Singleton]
        [FunctionName("TriggerTest1")]
        public static async Task Run1(
            [QueueTrigger("Test1", Connection = "AzureWebJobsStorage")]string myQueueItem
            , ExecutionContext context
            , [Table("Test1", Connection = "AzureWebJobsStorage")] CloudTable table
            , TraceWriter log)
        {
            await Run(myQueueItem, context, table, log);
        }

        [Singleton(Mode = SingletonMode.Listener)]
        [FunctionName("TriggerTest2")]
        public static async Task Run2(
            [QueueTrigger("Test2", Connection = "AzureWebJobsStorage")]string myQueueItem
            , ExecutionContext context
            , [Table("Test2", Connection = "AzureWebJobsStorage")] CloudTable table
            , TraceWriter log)
        {
            await Run(myQueueItem, context, table, log);
        }

        [FunctionName("TriggerTest3")]
        public static async Task Run3(
            [QueueTrigger("Test3", Connection = "AzureWebJobsStorage")]string myQueueItem
            , ExecutionContext context
            , [Table("Test3", Connection = "AzureWebJobsStorage")] CloudTable table
            , TraceWriter log)
        {
            
            await Run(myQueueItem, context, table, log);

        }

        private static async Task Run(
            string myQueueItem
            , ExecutionContext context
            , CloudTable table
            , TraceWriter log)
        {
            await table.CreateIfNotExistsAsync();
            var insertOperation = TableOperation.Insert(new Result()
            {
                PartitionKey = lazyDateTime.Value.ToString("N"),
                RowKey = string.Format("{0:D3}", int.Parse(myQueueItem)),
                InvocationId = context.InvocationId.ToString("N"),
                Time = DateTime.UtcNow.AddHours(9).ToLongTimeString()

            });
            await table.ExecuteAsync(insertOperation);
            Thread.Sleep(1000);
        }



        public class Result : TableEntity
        {
            public string Time { get; set; }
            public string InvocationId { get; set; }
            
        }
    }
}

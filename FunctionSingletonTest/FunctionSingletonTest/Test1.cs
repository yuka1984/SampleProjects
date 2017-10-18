using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Queue;

namespace FunctionSingletonTest
{
    public static class Test1
    { 
        [Singleton]
        [FunctionName("Test")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req
            ,[Queue(queueName: "Test1", Connection = "AzureWebJobsStorage")]IAsyncCollector<CloudQueueMessage> queue1
            , [Queue(queueName: "Test2", Connection = "AzureWebJobsStorage")]IAsyncCollector<CloudQueueMessage> queue2
            , [Queue(queueName: "Test3", Connection = "AzureWebJobsStorage")]IAsyncCollector<CloudQueueMessage> queue3
            , TraceWriter log)
        {
            for (int i = 0; i < 500; i++)
            {
                await queue1.AddAsync(new CloudQueueMessage($"{i + 1}"));
                await queue2.AddAsync(new CloudQueueMessage($"{i + 1}"));
                await queue3.AddAsync(new CloudQueueMessage($"{i + 1}"));
            }

            return req.CreateResponse(HttpStatusCode.OK, "Start");
        }
    }
}

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Host.Triggers;
using TwitterBinding;

namespace FunctionApp1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req
            , [Tweet]  ICollector<string> collector
            , TraceWriter log)
        {

            collector.Add("Azure Functionsの出力バインディングでツイートするテスト" + DateTime.UtcNow.AddHours(9));

            return req.CreateResponse("OK");


        }
    }


}

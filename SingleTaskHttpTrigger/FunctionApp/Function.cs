using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace FunctionApp
{
    public static class Function
    {
        [FunctionName("GetRequestTrigger")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestMessage req
            , ILogger log)
        {
            var request = new MessageModel();
            var namevalue = req.RequestUri.ParseQueryString();
            foreach (var key in namevalue.AllKeys) {
                if (key.Equals("a", StringComparison.CurrentCultureIgnoreCase))
                    if (int.TryParse(namevalue[key], out int a))
                        request.A = a;
                if (key.Equals("b", StringComparison.CurrentCultureIgnoreCase))
                    if (int.TryParse(namevalue[key], out int b))
                        request.B = b;
            }

            var namespaceManager =
                NamespaceManager.CreateFromConnectionString(
                    ConfigurationManager.AppSettings["servicebusInQueueConnections"]);
            var subscriptionName = Guid.NewGuid().ToString("N");

            await namespaceManager.CreateSubscriptionAsync(new SubscriptionDescription("out", subscriptionName));

            var sbclient =
                SubscriptionClient.CreateFromConnectionString(
                    ConfigurationManager.AppSettings["servicebusInQueueConnections"], "out", subscriptionName);

            var queueclient =
                QueueClient.CreateFromConnectionString(ConfigurationManager.AppSettings["servicebusInQueueConnections"],
                    "in");

            var resultJson = JsonConvert.SerializeObject(request);
            var binary = Encoding.UTF8.GetBytes(resultJson);


            var outMessage = new BrokeredMessage(new MemoryStream(binary));
            var id = Guid.NewGuid().ToString();
            outMessage.To = id;

            await queueclient.SendAsync(outMessage);

            while (true) {
                var message = await sbclient.ReceiveAsync();
                await message.CompleteAsync();
                if (message.To == id) {
                    var stream = message.GetBody<Stream>();
                    var json = new StreamReader(stream).ReadToEnd();

                    await namespaceManager.DeleteSubscriptionAsync("out", subscriptionName);
                    return req.CreateResponse(HttpStatusCode.OK, json);
                }
            }
        }


    }

    public class MessageModel
    {
        public int A { get; set; }

        public int B { get; set; }

        public int Result { get; set; }

        public DateTime ExecuteDateTime { get; set; }
    }
}
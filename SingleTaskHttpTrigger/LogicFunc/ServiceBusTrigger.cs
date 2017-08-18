using System;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.ServiceBus;
using Newtonsoft.Json;

namespace LogicFunc
{
    public static class ServiceBusTrigger
    {
        [FunctionName("SumSBTrigger")]
        public static void SumSBTrigger(
            [ServiceBusTrigger("in", AccessRights.Listen, Connection = "servicebusInQueueConnections")] BrokeredMessage
                myQueueItem
            ,
            [ServiceBus("out", Connection = "servicebusInQueueConnections", EntityType = EntityType.Topic)] out
                BrokeredMessage outMessage, ILogger log)
        {
            myQueueItem.RenewLock();
            var stream = myQueueItem.GetBody<Stream>();
            var json = new StreamReader(stream).ReadToEnd();
            var request = JsonConvert.DeserializeObject<MessageModel>(json);


            Thread.Sleep(5000);
            request.Result = request.A + request.B;
            request.ExecuteDateTime = DateTime.Now;

            var resultJson = JsonConvert.SerializeObject(request);
            var binary = Encoding.UTF8.GetBytes(resultJson);

            outMessage = new BrokeredMessage(new MemoryStream(binary));
            outMessage.To = myQueueItem.To;
            myQueueItem.Complete();
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

using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SlackConnector.Models;
using SlackMessageTriggerExtention;

namespace TestFunction
{
    public static class SlackTriggerFunction
    {
        [FunctionName("SlackTriggerFunction")]
        public static void Run([SlackMessageTrigger]SlackMessage message, TraceWriter log)
        {
            log.Info($"{message.Text} : {message.TimeStamp}");
        }
    }
}

using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;

namespace SlackMessageTriggerExtention
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public sealed class SlackMessageTriggerAttribute : Attribute
    {
        /// <summary>Gets the name of the connection string to use.</summary>
        public string AccessToken { get; set; } = "SlackBotUserAccessToken";
    }
}
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using SlackConnector;
using SlackConnector.Models;
using SlackMessageTriggerExtention;

namespace FunctionApp2
{
    public class SlackMessageListener : IListener
    {
        private ISlackConnection connection;
        private ISlackConnector connector;
        private SlackMessageTriggerAttribute attribute;

        public SlackMessageListener(ITriggeredFunctionExecutor executor, SlackMessageTriggerAttribute attribute)
        {
            Executor = executor;
            this.attribute = attribute;
        }

        public ITriggeredFunctionExecutor Executor { get; }

        public void Cancel() {}

        public void Dispose() {}

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            connector = new SlackConnector.SlackConnector();
            var accessToken =  AmbientConnectionStringProvider.Instance.GetConnectionString(attribute.AccessToken);
            connection = await connector.Connect(accessToken);
            connection.OnMessageReceived += ConnectionOnOnMessageReceived;
            connection.OnDisconnect += ConnectionOnOnDisconnect;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            connection.OnMessageReceived -= ConnectionOnOnMessageReceived;
            connection.OnDisconnect -= ConnectionOnOnDisconnect;
            connection.Disconnect();            
            connection = null;
            return Task.CompletedTask;
        }

        private void ConnectionOnOnDisconnect()
        {
            // TODO:要再接続実装
            throw new Exception("Slack Disconnect");
        }

        private async Task ConnectionOnOnMessageReceived(SlackMessage message)
        {
            var triggerData = new TriggeredFunctionData
            {
                TriggerValue = message
            };

            await Executor.TryExecuteAsync(triggerData, CancellationToken.None);
        }
    }
}
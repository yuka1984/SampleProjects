using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using FunctionApp2;
using Microsoft.Azure.WebJobs.Host.Triggers;
using SlackConnector.Models;

namespace SlackMessageTriggerExtention
{
    public class SlackMessageTriggerAttributeBindingProvider : ITriggerBindingProvider
    {
        private readonly SlackMessageExtentionConfig _extensionConfigProvider;

        public SlackMessageTriggerAttributeBindingProvider(SlackMessageExtentionConfig extensionConfigProvider)
        {
            _extensionConfigProvider = extensionConfigProvider;
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var parameter = context.Parameter;
            var attribute =
                parameter.GetCustomAttribute<SlackMessageTriggerAttribute>(false);
            if (attribute == null)
                return Task.FromResult<ITriggerBinding>(null);
            if (!IsSupportBindingType(parameter.ParameterType))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    "Can't bind SlackMessageTriggerAttribute to type '{0}'.", parameter.ParameterType));
            return
                Task.FromResult<ITriggerBinding>(new SlackMessageTriggerBinding(context.Parameter,
                    _extensionConfigProvider, context.Parameter.Member.Name));
        }

        public bool IsSupportBindingType(Type t)
        {
            return t == typeof(SlackMessage) || t == typeof(string);
        }
    }
}
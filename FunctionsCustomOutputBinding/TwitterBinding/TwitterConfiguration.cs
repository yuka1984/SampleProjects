using Microsoft.Azure.WebJobs.Host.Config;

namespace TwitterBinding
{
    public class TwitterConfiguration : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            context.AddConverter<string, TweetMessage>(input => new TweetMessage() {Message = input});

            context.AddBindingRule<TweetAttribute>().BindToCollector<TweetMessage>(attr => new TweetAsyncCollector(attr));
        }
    }
}
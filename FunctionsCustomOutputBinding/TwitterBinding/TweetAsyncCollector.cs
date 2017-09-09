using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreTweet;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json.Linq;

namespace TwitterBinding
{
    public class TweetAsyncCollector : IAsyncCollector<TweetMessage>
    {
        private readonly Tokens _token;

        public TweetAsyncCollector(TweetAttribute tweetAttribute)
        {
            _token = CoreTweet.Tokens.Create(tweetAttribute.ApiKey, tweetAttribute.ApiSeacret,
                tweetAttribute.AccessToken, tweetAttribute.AccessSeacret);
        }

        public async Task AddAsync(TweetMessage item, CancellationToken cancellationToken = new CancellationToken())
        {
            await _token.Statuses.UpdateAsync(new
            {
                status = item.Message,
            });
        }

        public Task FlushAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
        }        
    }
}

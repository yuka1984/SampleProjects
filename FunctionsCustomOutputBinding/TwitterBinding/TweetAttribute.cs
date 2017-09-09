using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;

namespace TwitterBinding
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public class TweetAttribute : Attribute
    {
        [AppSetting(Default = "TwitterApiKeyName")]
        public string ApiKey { get; set; }

        [AppSetting(Default = "TwitterApiSeacretName")]
        public string ApiSeacret { get; set; }

        [AppSetting(Default = "TwitterAccessTokenName")]
        public string AccessToken { get; set; }

        [AppSetting(Default = "TwitterAccessSeacretName")]
        public string AccessSeacret { get; set; }
    }
}

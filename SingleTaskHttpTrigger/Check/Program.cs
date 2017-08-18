using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Check
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(10);
            

            Task.WhenAll(
                Enumerable.Range(0, 10).Select(x =>
                {
                    return client.GetStringAsync("https://onikutabetai.azurewebsites.net/api/GetRequestTrigger")
                        .ContinueWith(
                            task => { Console.WriteLine(task.Result); });
                })).ContinueWith(t=> Console.WriteLine("Finish"));

            

            Console.ReadLine();
        }
    }
}

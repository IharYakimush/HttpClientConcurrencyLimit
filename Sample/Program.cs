using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using HttpClientConcurrencyLimit;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Sample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddLogging(l => l.AddConsole(c=>c.Format = Microsoft.Extensions.Logging.Console.ConsoleLoggerFormat.Systemd));
            services.AddHttpClient("sample").AddConcurrencyLimitationSemaphoreSlim();

            HttpClient client = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient("sample");
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(client.GetAsync($"http://www.foo.com?q={i}"));
            }

            await Task.WhenAll(tasks);
        }
    }
}

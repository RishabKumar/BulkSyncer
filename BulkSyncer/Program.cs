using BulkSyncer.Model;
using BulkSyncer.RateLimiter;
using BulkSyncer.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BulkSyncer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var calls = 1000;
            var maxTokens = 50;
            var dateList = new Queue<DateTime>();
            for (var i = 0; i < calls; i++)
            {
                dateList.Enqueue(DateTime.Now.AddDays(i * -1));
            }

            var apiService = new ApiService();
            var tokenBucket = new TokenBucket(maxTokens, dateList);

            while (tokenBucket.HasTokens)
            {
                var tasks = new List<Task<Weather>>();
                foreach (var token in tokenBucket.ConsumeToken())
                {
                    tasks.Add(apiService.GetWeatherData(token));
                }
                await tokenBucket.Refill();
                if (tasks.Count > 0)
                {
                    var weathers = await Task.WhenAll(tasks);
                    foreach (var weather in weathers)
                    {
                        Console.WriteLine(weather.Summary);
                        Console.WriteLine(weather.Date);
                        Console.WriteLine("---------------------------------");
                    }
                    tasks.Clear();
                }
            }
            Console.Read();
        }
    }
}
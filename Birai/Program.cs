using Birai.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Birai
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Bot bot = new();
            Console.WriteLine($"||> BOT已启动 ({DateTime.Now.ToLongTimeString()})\n");

            while (true)
            {
                await bot.TickAsync();

                Thread.Sleep(500);
            }
        }
    }
}

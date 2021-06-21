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
            Console.WriteLine($"[Birai] BOT已启动 ({DateTime.Now.ToLongTimeString()})\n");

            while (true)
            {
                try
                {
                    await bot.TickAsync();

                    Thread.Sleep(500);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}

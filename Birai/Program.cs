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
            Console.WriteLine("|||||||||> BOT已启动 <|||||||||");

            while (true)
            {
                await bot.TickAsync();

                Thread.Sleep(500);
            }
        }
    }
}

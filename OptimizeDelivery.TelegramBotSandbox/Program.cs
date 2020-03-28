using System;
using System.Threading.Tasks;
using OptimizeDelivery.TelegramBot;

namespace TelegramBotSandbox
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var bot = new OptimizeDeliveryTelegramBot();
            var me = await bot.Me();
            Console.WriteLine("Here's my Id: " + me.Id + " and my Name: " + me.FirstName);

            bot.Start();

            Console.ReadLine();
        }
    }
}
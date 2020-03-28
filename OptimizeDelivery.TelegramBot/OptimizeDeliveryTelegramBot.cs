using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace OptimizeDelivery.TelegramBot
{
    public class OptimizeDeliveryTelegramBot
    {
        private static readonly string TelegramBotAccessToken =
            ConfigurationManager.AppSettings["TelegramBotAccessToken"];

        private static ITelegramBotClient BotClient;

        private static readonly Dictionary<string, Func<Message, Task<Message>>> CommandToMethodMapping =
            new Dictionary<string, Func<Message, Task<Message>>>
            {
                ["/start"] = StartCommand,
                ["/register"] = RegisterCommand,
                ["/route"] = GetRouteCommand
            };

        private static readonly Dictionary<string, string> CommandToDescriptionMapping = new Dictionary<string, string>
        {
            ["/register"] = "register you in the optimize delivery service",
            ["/route"] = "returns a detailed route for today with link to Google Maps"
        };

        public OptimizeDeliveryTelegramBot()
        {
            BotClient = new TelegramBotClient(TelegramBotAccessToken);
        }

        public async Task<User> Me()
        {
            return await BotClient.GetMeAsync();
        }

        public void Start()
        {
            BotClient.OnMessage += OptimizeDeliveryBotOnMessage;
            BotClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        private async void OptimizeDeliveryBotOnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                if (CommandToMethodMapping.TryGetValue(e.Message.Text.ToLower(), out var command))
                    try
                    {
                        await command(e.Message);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                    }

                else
                    await DefaultCommand(e.Message);
            }
        }

        private static Task<Message> StartCommand(Message message)
        {
            return BotClient.SendTextMessageAsync(message.Chat,
                "Hi, " + message.From.FirstName + " " + message.From.LastName + " " + Emoji(0x1F44B) + "\n" +
                Emoji(0x1F69A) + " Here's an OptimizeDeliveryBot, I'm providing Routes for registered couriers.\n" +
                "Please, take a look at list of commands that I'm supporting:\n\n" +
                string.Join("\n", CommandToDescriptionMapping.Select(x => x.Key + " - " + x.Value)) + "\n\n" +
                "I recommend you to register first to get your daily routes.");
        }

        private static async Task<Message> RegisterCommand(Message message)
        {
            var apiClient = new OptimizeDeliveryApiClient();
            var user = message.From;
            Console.WriteLine(
                $"Trying register user, Id = {user.Id}, Name = {user.FirstName}, Surname = {user.LastName}");
            var response = await apiClient.CreateCourier(user.Id, user.FirstName, user.LastName);
            switch (response.Status)
            {
                case "Existing":
                    return await BotClient.SendTextMessageAsync(message.Chat,
                        "Looks like you've already registered. Try to get your /route for today.");
                case "Created":
                    return await BotClient.SendTextMessageAsync(message.Chat,
                        "Successfully registered! Now you can try to get your /route for today.");
                default:
                    return await BotClient.SendTextMessageAsync(message.Chat,
                        "Something went wrong.");
            }
        }

        private static async Task<Message> GetRouteCommand(Message message)
        {
            var apiClient = new OptimizeDeliveryApiClient();
            var response = await apiClient.GetRouteForToday(message.From.Id);
            switch (response.Status)
            {
                case "Unauthorized":
                    return await BotClient.SendTextMessageAsync(message.Chat,
                        "Looks like you're not registered user. Try use /register command first.");
                case "NoRoutes":
                    return await BotClient.SendTextMessageAsync(message.Chat,
                        "I can't find any route for you for today. Possibly all routes were assigned to other couriers.");
                case "OK":
                    return await BotClient.SendTextMessageAsync(message.Chat,
                        "Your route for today: \n\n" +
                        string.Join("\n", response.Steps.Select(x => Emoji(0x1F4CD) + x)),
                        ParseMode.Default, false, false, 0,
                        new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl("Open route in Maps",
                            response.RouteUrl)));
                default:
                    return await BotClient.SendTextMessageAsync(message.Chat,
                        "Something went wrong.");
            }
        }

        private static Task<Message> DefaultCommand(Message message)
        {
            return BotClient.SendTextMessageAsync(message.Chat,
                "Looks like I don't get what you're asking about. Sorry :(");
        }

        private static string Emoji(int code)
        {
            return char.ConvertFromUtf32(code);
        }
    }
}
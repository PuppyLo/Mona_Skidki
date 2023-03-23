using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using BarcodeLib;
using Telegram.Bot.Types.InputFiles;

var botClient = new TelegramBotClient("5477124482:AAGhkN5aZB9uYdacpIVdmL7uc1gO5oyNtKY");

        var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions {
            AllowedUpdates = { }
        };

botClient.StartReceiving(
    HandleUpdatesAsync,
    HandleErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token);

Console.WriteLine($"Начал прослушку @{botClient.GetMeAsync().Result.FirstName}");
Console.ReadLine();

async Task HandleUpdatesAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
    if (update.Type == UpdateType.Message && update?.Message?.Text != null) {
        await HandleMessage(botClient, update.Message);
        return;
    }

    if (update.Type == UpdateType.CallbackQuery) {
        await HandleCallbackQuery(botClient, update.CallbackQuery);
        return;
    }
}

async Task HandleMessage(ITelegramBotClient botClient, Message message) {
    string firstName = message.From.FirstName;

    ReplyKeyboardMarkup keyboard = new(
        new[]
        {
            new KeyboardButton[] {"Выпустить карту", "Помощь"},
        }) {
        ResizeKeyboard = true
    };

    InlineKeyboardMarkup inlineKeyboard_Kontakty = new(new[]
   {
         new[]
         {
                InlineKeyboardButton.WithUrl("Связаться c Владимиром", @"https://t.me/vova534"),
         }
    });

    switch (message.Text) {
        case "/start": {
                try {
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"{firstName}, Добро пожаловать в MonaPuf! Выберите, что вас интересует 🔽", replyMarkup: keyboard);
                    Console.WriteLine(message.Chat.Id + "@" + message.Chat.Username + " - " + message.From.FirstName + " - " + message.Chat.FirstName + " - " + message.From.LastName + " - " + message.Text);
                    await botClient.SendTextMessageAsync(5112277210, message.Chat.Id + "@" + message.Chat.Username + " - " + message.From.FirstName + " - " + message.Chat.FirstName + " - " + message.From.LastName + " - " + message.Text);
                }
                catch { }
                break;
            }
        case "Помощь": {
                try {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "По всем вопросам: ", replyMarkup: inlineKeyboard_Kontakty);
                }
                catch { }
                break;
            }

        case "Выпустить карту": {
               
                        Barcode _b = new();
                        Image img = _b.Encode(TYPE.CODE128, "200000", Color.Black, Color.White, 290, 120);
                        _b.SaveImage(@"C:\XboxGames\1233.png", SaveTypes.PNG);

                string mypath = @"C:\XboxGames\1233.png";
                using var fileStream = new FileStream(mypath, FileMode.Open, FileAccess.Read, FileShare.Read);
                await botClient.SendPhotoAsync(
                        message.Chat.Id,
                        photo: new InputOnlineFile(fileStream),
                        caption: "My Photo");

                return;
            }

        default: break;
    }
    return;
}

async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery) {
    Message msg = callbackQuery.Message;

}



Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken) {
    var ErrorMessage = exception switch {
        ApiRequestException apiRequestException
            => $"Ошибка телеграм АПИ:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    Console.WriteLine(ErrorMessage);

    return Task.CompletedTask;
}

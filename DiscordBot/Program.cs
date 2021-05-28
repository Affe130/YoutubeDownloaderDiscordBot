using DiscordBot.Modules;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentFTP;
using System.Net;

namespace DiscordBot
{
    class Program
    {
        public DiscordSocketClient client;
        public CommandService commands;
        public IServiceProvider services;

        public static Settings settings;
        public static Logger logger;
        public static FtpClient ftpClient;

        public static string rootPath;
        public static string configFilePath;
        public static string logFilePath;
        public static string downloadsPath;

        static void Main(string[] args)
        {
            Console.Title = "YoutubeDownloadDiscordBot";

            rootPath = Directory.GetCurrentDirectory();
            configFilePath = Path.Combine(rootPath, "config.json");
            logFilePath = Path.Combine(rootPath, "log.txt");
            downloadsPath = Path.Combine(rootPath, "downloads");

            logger = new(logFilePath);

            if (!Directory.Exists(downloadsPath))
            {
                Directory.CreateDirectory(downloadsPath);
            }

            if (!File.Exists(configFilePath))
            {
                logger.ConsoleLog(Logger.LogType.Error, $"config.json was not found");
                logger.ConsoleLog(Logger.LogType.Info, $"Creating config.json...");
                settings = new();
                settings.SetToStandard(configFilePath);
                logger.ConsoleLog(Logger.LogType.Info, $"config.json was created in {rootPath} modify config.json with your bot token");
                Console.ReadKey();

                return;
            }

            try
            {
                logger.ConsoleLog(Logger.LogType.Info, $"Loading settings...");
                settings = Settings.LoadFromJson(configFilePath);
            }
            catch
            {
                logger.ConsoleLog(Logger.LogType.Fatal, $"Could't load settings from config.json");
                Console.ReadKey();

                return;
            }

            try
            {
                logger.ConsoleLog(Logger.LogType.Info, $"Connecting to FTP server...");
                FtpClient ftpClient = new(settings.FtpHost);
                ftpClient.Credentials = new NetworkCredential(settings.FtpUsername, settings.FtpPassword);
                ftpClient.ConnectAsync();
            }
            catch
            {
                logger.ConsoleLog(Logger.LogType.Fatal, $"Could't connect to FTP server");
                Console.ReadKey();

                return;
            }

            try
            {
                logger.ConsoleLog(Logger.LogType.Info, $"Connecting to Discord...");
                new Program().RunBotAsync().GetAwaiter().GetResult();
            }
            catch
            {
                logger.ConsoleLog(Logger.LogType.Fatal, $"Could't connect to Discord");
                Console.ReadKey();

                return;
            }
        } 

        private async Task RunBotAsync()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();
            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .BuildServiceProvider();

            client.Log += logger.DiscordLog;

            await client.SetGameAsync(settings.BotStatus);
            await client.SetStatusAsync(UserStatus.Online);

            await RegisterCommandsAsync();

            await client.LoginAsync(TokenType.Bot, settings.BotToken);

            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task RegisterCommandsAsync()
        {
            client.MessageReceived += HandleCommandAsync;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            SocketCommandContext context = new(client, message);
            if (message.Author.IsBot)
            {
                return;
            }
            int argPos = 0;
            if (message.HasStringPrefix(settings.CommandPrefix, ref argPos))
            {
                logger.ConsoleLog(Logger.LogType.Info, $"Command: {message} executed by {message.Author}");
                IResult result = await commands.ExecuteAsync(context, argPos, services);
                if (!result.IsSuccess)
                {
                    logger.ConsoleLog(Logger.LogType.Error, $"{result.Error} Reason: {result.ErrorReason}");
                }
            }
        }
    }
}

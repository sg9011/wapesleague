using Base.Bot.Infrastructure;
using Base.Bot.Queue;
using Base.Bot.Queue.Dto;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Base.Bot
{
    public class Bot
    {
        private readonly ILogger<Bot> _logger;
        private CancellationTokenSource _cts { get; set; }
        private DiscordSettings _discordSettings { get; set; }

        public DiscordClient _discordClient { get; private set; }
        public CommandsNextExtension _commands { get; private set; }
        public IServiceProvider _serviceProvider { get; private set; }

        public Bot(DiscordSettings discordSettings, IServiceProvider serviceProvider, ILogger<Bot> logger)
        {
            _discordSettings = discordSettings;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            _cts = new CancellationTokenSource();

            var loggerFactory = new LoggerFactory().AddSerilog();
            var config = new DiscordConfiguration
            {
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Warning,
                LoggerFactory = loggerFactory,
                Token = _discordSettings.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All
            };
            _discordClient = new DiscordClient(config);

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = _discordSettings.CommandPrefixes,
                CaseSensitive = false,
                Services = _serviceProvider
            };
            _commands = _discordClient.UseCommandsNext(commandsConfig);
            _commands.RegisterCommands(Assembly.GetEntryAssembly());

            _discordClient.GuildMemberAdded += (s, e) =>
            {
                try
                {
                    _logger.LogWarning($"GuildMemberAdded Event!, Guild: {e?.Guild?.Id}");
                    GuildMemberAddedQueue.Queue.Enqueue(new GuildMemberAddedDto(e.Guild.Id, e.Guild.Name, e.Member.Id));
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "GuildMemberAdded event not added to the Queue!");
                }
                return Task.CompletedTask;
                
            };

            await _discordClient.ConnectAsync();

            while (!_cts.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
            _logger.LogCritical("The Bot.cs class reached the last line of the class");

        }

        public async Task<DiscordClient> GetConnectedDiscordClientAsync()
        {
            var loggerFactory = new LoggerFactory().AddSerilog();
            var config = new DiscordConfiguration
            {
                AutoReconnect = false,
                MinimumLogLevel = LogLevel.Warning,
                LoggerFactory = loggerFactory,
                Token = _discordSettings.Token,
                TokenType = TokenType.Bot
            };

            _discordClient = new DiscordClient(config);
            await _discordClient.ConnectAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(10));

            return _discordClient;
        }
    }
}

using Base.Bot.Commands;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Business.Dto.Mix;
using AutoMapper;
using WaPesLeague.Business.Dto;
using WaPesLeague.Bot.Commands.Base;
using Microsoft.Extensions.Logging;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Constants;
using System.Collections.Generic;
using WaPesLeague.Bot.Helpers;

namespace WaPesLeague.Bot.Commands.Mix
{
    public class MixCommandsModule : BaseMixBotModule<MixCommandsModule>
    {
        private readonly IMixSessionWorkflow _mixSessionWorkflow;
        private readonly IUserWorkflow _userWorkflow;
        private readonly IMixStatsWorkflow _mixStatsWorkflow;
        private readonly IMapper _mapper;

        public MixCommandsModule(IMixSessionWorkflow mixSessionWorkflow, IUserWorkflow userWorkflow, IMixStatsWorkflow mixStatsWorkflow, IServerWorkflow serverWorkflow,
            IMapper mapper, ILogger<MixCommandsModule> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(serverWorkflow, logger, errorMessages, generalMessages)
        {
            _mixSessionWorkflow = mixSessionWorkflow;
            _userWorkflow = userWorkflow;
            _mixStatsWorkflow = mixStatsWorkflow;
            _mapper = mapper;
        }

        [Command("CreateMix"), Aliases("Create", "Criar")]
        [Description("Create a Mix Room")]
        public async Task CreateMix(CommandContext ctx,
            [Description("Options to overwrite the defaults when creating a mix. For more help: .CreateHelp")] [RemainingText] string stringOptions)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasAdvancedRolesAsync(ctx))
                    return;

                var discordCommandProperties = new DiscordCommandProperties(ctx); 
                var createRoomDto = new CreateMixRoomGroupDto(_mapper.Map<DiscordCommandPropsDto>(discordCommandProperties), server);

                try { createRoomDto.MapOptionsToDto(stringOptions, ErrorMessages, GeneralMessages); }
                catch (Exception ex)
                {
                    await ReplyToFailedRequest(ctx.Message, $"Something went wrong while mapping your parameters, {ex.Message}");
                    return;
                }

                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.CreateMix, discordCommandProperties, server)
                {
                    CreateMixRoomGroupDto = createRoomDto
                });
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("DeleteMixGroup"), Aliases("DeleteGroup", "DeleteMix", "DeleteSession", "DeleteMixSession", "DeleteChannel", "DeleteMixChannel", "ExcluirGrupo")]
        [Description("Delete a mixroom and the related channels.")]
        public async Task DeactivateMixGroup(CommandContext ctx, [RemainingText] string textToIgnore = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasTopTierRolesAsync(ctx))
                    return;

                var discordCommandProperties = new DiscordCommandProperties(ctx);
                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.DeleteMix, discordCommandProperties, server));
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("SignIn"), Aliases("In", "On", "Em", "Ent", "Dentro", "Jugar", "Si")]
        [Description("Sign in into a Mix Room")]
        public async Task In(CommandContext ctx, [Description("1 of the Team codes")] string team = null, [Description("The position code")] string position = null, [Description("You can provide some extra info like **up on ck** for example")][RemainingText] string extraInfo = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                var discordCommandProps = new DiscordCommandProperties(ctx);
                var actorRoleIds = ctx.Member?.Roles?.Select(r => r.Id.ToString()).ToList() ?? new List<string>();
                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.SignIn, discordCommandProps, server)
                {
                    Team = team,
                    Position = position,
                    ExtraInfo = extraInfo,
                    RoleIdsPlayer1 = actorRoleIds,
                    ActorRoleIds = actorRoleIds
                });
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("SignUserIn"), Aliases("UserIn", "UserOn", "PlayerIn", "PlayerOn", "UsuárioEm", "UsuarioEm")]
        [Description("Sign Someone in by userMention, into the Mix Room")]
        public async Task UserIn(CommandContext ctx, [Description("The discord member you want to sign in")] DiscordMember member, [Description("1 of the Team codes")] string team = null, [Description("The position code")] string position = null, [Description("You can provide some extra info like **up on ck** for example")][RemainingText] string extraInfo = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasAdvancedRolesAsync(ctx))
                    return;
                var discordCommandProps = new DiscordCommandProperties(ctx, member);

                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.SignIn, discordCommandProps, server)
                {
                    Team = team,
                    Position = position,
                    ExtraInfo = extraInfo,
                    RoleIdsPlayer1 = member?.Roles?.Select(r => r.Id.ToString()).ToList() ?? new List<string>(),
                    ActorRoleIds = ctx.Member?.Roles?.Select(r => r.Id.ToString()).ToList() ?? new List<string>(),
                });
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("SignOut"), Aliases("Out", "Off", "Fora", "Sai", "Fuera", "Quitar", "No", "JEnAiAssez", "JEnAiMarre")]
        [Description("Sign out of a Mix Room")]
        public async Task Out(CommandContext ctx, [RemainingText] string textToIgnore = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                var discordCommandProperties = new DiscordCommandProperties(ctx);
                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.SignOut, discordCommandProperties, server));
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("SignUserOut"), Aliases("UserOut", "UserOff", "PlayerOut", "PlayerOff", "UsuárioFora", "Afdassen")]
        [Description("Sign Someone out of the Mix Room by userMention")]
        public async Task UserOut(CommandContext ctx, [Description("The discord member you want to sign out")] DiscordMember member, [RemainingText] string textToIgnore = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasAdvancedRolesAsync(ctx))
                    return;

                var discordCommandProperties = new DiscordCommandProperties(ctx, member);
                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.SignOut, discordCommandProperties, server));
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("Refresh"), Aliases("Current", "Show", "Atual", "Actualizar", "Tonen", "Equipos", "Teams", "Equipes", "Times")]
        [Description("Display the current mix again")]
        public async Task Refresh(CommandContext ctx, [RemainingText] string textToIgnore = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                var discordCommandProperties = new DiscordCommandProperties(ctx);
                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.ShowMix, discordCommandProperties, server));
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        //No Queue
        [Command("Sides"), Aliases("CurrentSides", "ShowSides", "Side", "Lados", "Veja", "Demonstrar", "Kant", "WhereDoWeGo")]
        [Description("Display the side selection")]
        public async Task Sides(CommandContext ctx, [RemainingText] string textToIgnore = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                var result = await _mixSessionWorkflow.ShowSidesAsync(server.ServerId, ctx.Channel.Id);

                await ctx.RespondAsync(result.Message);
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        //No Queue
        [Command("MyStats"), Aliases("MixStats", "Estatísticas", "Estatisticas", "MisEstadísticas")]
        [Description("Get advanced stats for mix")]
        public async Task AdvancedStats(CommandContext ctx, [RemainingText] string textToIgnore = null)
        {
            try
            {
                Data.Entities.Discord.Server server = null;
                if (ctx.Guild != null)
                {
                    server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);
                }

                var discordCommandProperties = new DiscordCommandProperties(ctx);
                await HandleAdvancedStats(ctx, discordCommandProperties, server);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error with the UserStats");
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        //No Queue
        [Command("UserStats"), Aliases("UserMixStats", "PlayerMixStats", "PlayerStats")]
        [Description("Get advanced user stats for mix")]
        public async Task UserAdvancedStats(CommandContext ctx, [Description("The discord member")] DiscordMember userMention, [RemainingText] string textToIgnore = null)
        {
            try
            {
                Data.Entities.Discord.Server server = null;
                if (ctx.Guild != null)
                {
                    server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);
                }

                if (!await ValidateHasAdvancedRolesAsync(ctx))
                    return;

                var discordCommandProperties = new DiscordCommandProperties(ctx, userMention);
                await HandleAdvancedStats(ctx, discordCommandProperties, server);
                
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error with the UserStats");
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        private async Task HandleAdvancedStats(CommandContext ctx, DiscordCommandProperties discordCommandProperties, Data.Entities.Discord.Server server)
        {
            var discordPropsDto = _mapper.Map<DiscordCommandPropsDto>(discordCommandProperties);
            var userId = await _userWorkflow.GetOrCreateUserIdByDiscordId(discordPropsDto);
            var result = await _mixStatsWorkflow.GetUserAdvancedStats(userId, server?.ServerId);
            var embed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Gold,
                Title = $"Stats of {discordCommandProperties.NickName ?? discordCommandProperties.UserName}",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail()
                {
                    Height = 16,
                    Width = 16,
                    Url = ctx.Member?.AvatarUrl
                }
            };
            foreach (var r in result.OrderByDescending(x => x.Order))
            {
                embed.AddField($"{r.PostionGroup}{(r.ReservationCount > 0 ? $" x {r.ReservationCount}" : string.Empty)}", r.MinutesPlayed != 0 ? r.ReadableTime : "NaN", true);
            }
            embed.Footer = new DiscordEmbedBuilder.EmbedFooter()
            {
                Text = "Ask visualcomplexity for more info!"

            };

            var message = new DiscordMessageBuilder();
            message.AddEmbed(embed);
            message.AddDiscordLinkButtonsToMessageIfNeeded(server, new Random());
            
            await ctx.RespondAsync(message);
        }

        [Command("Password"), Aliases("SetPassword", "PW", "P", "SefinirSenha", "Senha", "Sen", "Contraseña", "Contrasena")]
        [Description("Set password for mix")]
        public async Task SetPassword(CommandContext ctx, [RemainingText][Description("The password")] string password = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if ((password?.Length ?? 0) > 20)
                {
                    await ctx.RespondAsync(string.Format(ErrorMessages.ValueToLong.GetValueForLanguage(), 20));
                    return;
                }
                var discordCommandProps = new DiscordCommandProperties(ctx);

                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.SetPassword, discordCommandProps, server)
                {
                    Password = password?.Trim()
                });
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("Server"), Aliases("SetServer", "S", "ServidorDefinido", "Servidor")]
        [Description("Set Server for mix")]
        public async Task SetServer(CommandContext ctx, [RemainingText][Description("The server")] string server = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var dbDiscordServer = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if ((server?.Length ?? 0) > 30)
                {
                    await ctx.RespondAsync(string.Format(ErrorMessages.ValueToLong.GetValueForLanguage(), 30));
                    return;
                }
                var discordCommandProps = new DiscordCommandProperties(ctx);
                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.SetServer, discordCommandProps, dbDiscordServer)
                {
                    GameServer = server?.Trim()
                });
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("Room"), Aliases("SetRoom", "SetName", "Sala", "NomeDoSala", "Nome")]
        [Description("Set Room name for mix")]
        public async Task SetRoomName(CommandContext ctx, [RemainingText][Description("The RoomName")] string roomName = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if ((roomName?.Length ?? 0) > 50)
                {
                    await ctx.RespondAsync(string.Format(ErrorMessages.ValueToLong.GetValueForLanguage(), 50));
                    return;
                }
                var discordCommandProps = new DiscordCommandProperties(ctx);
                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.SetRoom, discordCommandProps, server)
                {
                    RoomName = roomName?.Trim()
                });
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("RoomOwner"), Aliases("Owner", "SetRoomOwner", "SetOwner", "Proprietário", "Proprietario", "Chefe", "dueno")]
        [Description("Set roomowner for mix")]
        public async Task SetRoomOwner(CommandContext ctx, [Description("Leave it blank if you want to put yourself as roomowner, if you want to assign it to someone else provice the discord mention")] DiscordMember roomOwnerMember = null, [RemainingText] string textToIgnore = null)
        {
            try
            {
                DiscordCommandProperties discordCommandProps = null;
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (roomOwnerMember != null)
                    discordCommandProps = new DiscordCommandProperties(ctx, roomOwnerMember);
                if (discordCommandProps == null)
                    discordCommandProps = new DiscordCommandProperties(ctx);

                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.SetRoomOwner, discordCommandProps, server));
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("Captain"), Aliases("SetCaptain", "Cap", "C", "Capitaine", "Kapitein", "Capitana", "Capitano", "Capitan", "Kapitän", "Kapitan", "Capitão", "Capitao")]
        [Description("Set captain for mix")]
        public async Task SetCaptain(CommandContext ctx, [Description("Leave it blank if you want to put yourself as Captain, if you want to assign it to someone else provice the discord mention")] DiscordMember captainMember = null, [RemainingText] string textToIgnore = null)
        {
            try
            {
                DiscordCommandProperties discordCommandProps = null;
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (captainMember != null)
                    discordCommandProps = new DiscordCommandProperties(ctx, captainMember);
                if (discordCommandProps == null)
                    discordCommandProps = new DiscordCommandProperties(ctx);

                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.SetCaptain, discordCommandProps, server));
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("PlayerCount"), Aliases("SetPlayerCount", "NumberOfPlayers", "PC", "NumeroDeJugadores", "NDJ")]
        [Description("Set player count for a locked team")]
        public async Task SetPlayerCount(CommandContext ctx,
            [Description("Number of players in the locked team, between 1 and 11")] int playerCount,
            [Description("The alias of the locked team, only needed when both teams are locked")] string teamCode = null,
            [RemainingText] string textToIgnore = null)
        {
            try
            {
                DiscordCommandProperties discordCommandProps = null;
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (playerCount < 0 || playerCount > 11)
                {
                    await ctx.RespondAsync(ErrorMessages.PlayerCountValueError.GetValueForLanguage());
                    return;
                }

                discordCommandProps = new DiscordCommandProperties(ctx);

                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.SetPlayerCount, discordCommandProps, server)
                {
                    PlayerCount = playerCount,
                    Team = teamCode
                });
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("Clean"), Aliases("CleanRoom", "Clear", "Claro")]
        [Description("Clean the current mixSession")]
        public async Task Clean(CommandContext ctx, [RemainingText] string textToIgnore = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasAdvancedRolesAsync(ctx))
                    return;
                var discordCommandProps = new DiscordCommandProperties(ctx);

                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.CleanMix, discordCommandProps, server));
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("UpdatePosition"), Aliases("UpdateMixSessionPosition", "UpdateSessionPosition")]
        [Description("Update a position in the current mixSession")]
        public async Task UpdatePosition(CommandContext ctx, 
            [Description("1 of the Team Codes")] string team = null, 
            [Description("The position you want to change")] string oldPosition = null, 
            [Description("the new position")] string newPosition = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);
                if (!await ValidateHasAdvancedRolesAsync(ctx))
                    return;

                var discordCommandProps = new DiscordCommandProperties(ctx);
                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.UpdatePosition, discordCommandProps, server)
                {
                    OldPosition = oldPosition,
                    NewPosition = newPosition,
                    Team = team
                });
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }


        [Command("Swap"), Aliases("Switch", "SwapPlayers", "SwitchPlayers", "SwitchUsers", "SwapUsers")]
        [Description("Swap 2 players within a mixSession.")]
        public async Task Swap(CommandContext ctx,
            [Description("Player 1")] DiscordMember player1,
            [Description("Player 2 if not provided this will be the person that sends the message.")] DiscordMember player2 = null,
            [RemainingText] string textToIgnore = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);
                if (!server.AllowActiveSwapCommand && !server.AllowInactiveSwapCommand)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.CommandDisabledError.GetValueForLanguage());
                }

                if (player1.Id == (player2?.Id ?? ctx.User.Id))
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.SwapWithYourselfIsNotAllowedError.GetValueForLanguage());
                }

                var discordCommandProps = new DiscordCommandProperties(ctx);
                var discordCommandPropsPlayer1 = new DiscordCommandProperties(ctx, player1);
                var discordCommandPropsPlayer2 = player2 != null
                    ? new DiscordCommandProperties(ctx, player2)
                    : discordCommandProps;

                var actorRoleIds = ctx.Member.Roles?.Select(r => r.Id.ToString()).ToList() ?? new List<string>();
                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.Swap, discordCommandProps, server)
                {
                    Player1 = discordCommandPropsPlayer1,
                    RoleIdsPlayer1 = player1.Roles?.Select(r => r.Id.ToString()).ToList() ?? new List<string>(),
                    Player2 = discordCommandPropsPlayer2,
                    RoleIdsPlayer2 = player2 != null
                        ? player2.Roles?.Select(r => r.Id.ToString()).ToList() ?? new List<string>()
                        : actorRoleIds,
                    ActorRoleIds = actorRoleIds
                });
            }
            catch(Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("OpenTeam"), Aliases("Open", "OpenUpTeam", "Abrir", "OpenSesame", "Sesame")]
        [Description("Open the closed mixteam in the session!")]
        public async Task OpenTeam(CommandContext ctx,
            [Description("only this role can sign players into the opened team for a limited time.")] DiscordRole discordRole = null,
            [Description("The timeframe that this role has sole access to the opened team")] int minutes = 1,
            [RemainingText] string textToIgnore = null)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }
                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasAdvancedRolesAsync(ctx))
                    return;

                var discordCommandProps = new DiscordCommandProperties(ctx);
                var requestDto = new MixRequestDto(MixRequestType.OpenTeam, discordCommandProps, server);
                if (discordRole != null && minutes > 0)
                {
                    requestDto.RoleId = discordRole.Id;
                    requestDto.RoleName = discordRole.Name;
                    requestDto.Minutes = minutes;
                }

                MixRequestQueue.Queue.Enqueue(requestDto);
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }

        [Command("EarlyRoleRegistration"), Aliases("EarlyRole", "EarlyRegistration")]
        [Description("Set a registration in minutes that you want to open earlier relative to the normal registration time for a role in a certain MixGroup")]
        public async Task RoleRegistration(CommandContext ctx,
            [Description("The role that will get a special treatment")] DiscordRole role,
            [Description("The amount of minutes, positive means early and negative number means late")] int minutes)
        {
            try
            {
                if (ctx.Guild == null)
                {
                    await ReplyToFailedRequest(ctx.Message, ErrorMessages.PrivateServerNotAllowedError.GetValueForLanguage());
                    return;
                }

                var server = await SetServerCulture(ctx.Guild.Id, ctx.Guild.Name);

                if (!await ValidateHasTopTierRolesAsync(ctx))
                    return;


                var discordCommandProps = new DiscordCommandProperties(ctx);
                MixRequestQueue.Queue.Enqueue(new MixRequestDto(MixRequestType.RoleRegistration, discordCommandProps, server)
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Minutes = minutes * -1
                });
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync(string.Format(ErrorMessages.BaseError.GetValueForLanguage(), ex.Message));
            }
        }
    }
}

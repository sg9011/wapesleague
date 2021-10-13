using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Business.Dto.Mix
{
    public class CreateMixRoomDtoValidator : AbstractValidator<CreateMixRoomGroupDto>
    {
        private readonly IMixChannelManager _mixChannelManager;
        private readonly IServerFormationManager _serverFormationManager;
        private readonly ErrorMessages ErrorMessages;
        private readonly GeneralMessages GeneralMessages;

        public CreateMixRoomDtoValidator(IMixChannelManager mixChannelManager, IServerFormationManager serverFormationManager, ErrorMessages errorMessages, GeneralMessages generalMessages)
        {
            _mixChannelManager = mixChannelManager;
            _serverFormationManager = serverFormationManager;
            ErrorMessages = errorMessages;
            GeneralMessages = generalMessages;

            RuleFor(obj => obj)
                .MustAsync(NoOtherRoomsActiveInChannel)
                .WithMessage(ErrorMessages.BotAlreadyActiveInChannel.GetValueForLanguage())
                .Must(ValidTimes)
                .WithMessage(ErrorMessages.InvalidTimesForMixRoom.GetValueForLanguage())
                .Must(x => !string.Equals(x.TeamAName, x.TeamBName, StringComparison.InvariantCultureIgnoreCase)
                    || string.IsNullOrWhiteSpace(x.TeamAName))
                .WithMessage(ErrorMessages.TeamsWithSameName.GetValueForLanguage());

            RuleFor(obj => obj)
                .MustAsync(ValidFormationA)
                .WithMessage(x => string.Format(ErrorMessages.FormationUnknown.GetValueForLanguage(), x));

            RuleFor(obj => obj)
                .MustAsync(ValidFormationB)
                .WithMessage(x => string.Format(ErrorMessages.FormationUnknown.GetValueForLanguage(), x));

            RuleFor(obj => obj.DiscordServerId)
                .Must(x => x > 0)
                .WithMessage(ErrorMessages.DiscordServerIdRequired.GetValueForLanguage());

            RuleFor(obj => obj.DiscordChannelId)
                .Must(x => x > 0)
                .WithMessage(ErrorMessages.DiscordChannelIdRequired.GetValueForLanguage());

            RuleFor(obj => obj.Name)
                .MaximumLength(90)
                .WithMessage(string.Format(ErrorMessages.ValidationValueToLong.GetValueForLanguage(), GeneralMessages.ChannelName.GetValueForLanguage(), 90));

            RuleFor(obj => obj.TeamAName)
                .MaximumLength(50)
                .WithMessage(string.Format(ErrorMessages.ValidationValueToLong.GetValueForLanguage(), GeneralMessages.TeamAName.GetValueForLanguage(), 50))
                .Must(x => x != null)
                .WithMessage(string.Format(ErrorMessages.PropForCreateRequired.GetValueForLanguage(), GeneralMessages.TeamAName.GetValueForLanguage()));

            RuleFor(obj => obj.TeamBName)
                .MaximumLength(50)
                .WithMessage(string.Format(ErrorMessages.ValidationValueToLong.GetValueForLanguage(), GeneralMessages.TeamBName.GetValueForLanguage(), 50))
                .Must(x => x != null)
                .WithMessage(string.Format(ErrorMessages.PropForCreateRequired.GetValueForLanguage(), GeneralMessages.TeamBName.GetValueForLanguage()));

            RuleFor(obj => obj.ExtraInfo)
                .MaximumLength(500)
                .WithMessage(string.Format(ErrorMessages.ValidationValueToLong.GetValueForLanguage(), GeneralMessages.ExtraInfo.GetValueForLanguage(), 500));
                
        }

        public async Task<bool> NoOtherRoomsActiveInChannel(CreateMixRoomGroupDto dto, CancellationToken cancellationToken)
        {
            var NoActiveChannelInDiscordChannel = !await _mixChannelManager.HasActiveMixChannelByDiscordIdsAsync(dto.DiscordServerId.ToString(), dto.DiscordChannelId.ToString());
            if (NoActiveChannelInDiscordChannel)
            {
                var activeMixChannel = await _mixChannelManager.GetActiveMixChannelByDiscordIds(dto.DiscordServerId.ToString(), dto.DiscordChannelId.ToString());
                return activeMixChannel == null;
            }
            return NoActiveChannelInDiscordChannel;
        }

        public async Task<bool> ValidFormationA(CreateMixRoomGroupDto dto, CancellationToken cancellationToken)
        {
            return await ValidFormation(dto.TeamAFormation, dto.DiscordServerId);
        }
        public async Task<bool> ValidFormationB(CreateMixRoomGroupDto dto, CancellationToken cancellationToken)
        {
            return await ValidFormation(dto.TeamBFormation, dto.DiscordServerId);
        }

        private async Task<bool> ValidFormation(string formationName, ulong discordServerId)
        {
            return string.IsNullOrEmpty(formationName)
                ? true
                : await _serverFormationManager.HasFormationWithTagOrNameAsync(formationName, discordServerId.ToString());
        }

        public bool ValidTimes(CreateMixRoomGroupDto dto)
        {
            return dto.StartTime < 24m && dto.StartTime >= 0m &&
                dto.HoursToOpenRegistrationBeforeStart < 24m && dto.HoursToOpenRegistrationBeforeStart >= 0m &&
                dto.MaxSessionDurationInHours > 0m && dto.MaxSessionDurationInHours <= (24m - dto.HoursToOpenRegistrationBeforeStart);
        }
    }
}

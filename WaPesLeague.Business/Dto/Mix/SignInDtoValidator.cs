using FluentValidation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Helpers;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Business.Dto.Mix
{
    public class SignInDtoValidator : AbstractValidator<SignInDto>
    {
        private readonly IMixChannelManager _mixChannelManager;
        private readonly IMixSessionWorkflow _mixSessionWorkflow;
        private readonly IMixSessionManager _mixSessionManager;
        private readonly ErrorMessages ErrorMessages;
        private readonly GeneralMessages GeneralMessages;

        private bool hasChannel { get; set; }
        private bool userNotAlreadyActiveInOtherMixChannel { get; set; }
        private bool withinValidHours { get; set; }
        private MixGroupIdAndRegistrationTime mixGroupIdAndRegistrationTime { get; set; }

        public SignInDtoValidator(IMixChannelManager mixChannelManager, IMixSessionWorkflow mixSessionWorkflow, IMixSessionManager mixSessionManager,ErrorMessages errorMessages, GeneralMessages generalMessages)
        {
            CascadeMode = CascadeMode.Stop;
            _mixChannelManager = mixChannelManager;
            _mixSessionWorkflow = mixSessionWorkflow;
            _mixSessionManager = mixSessionManager;
            ErrorMessages = errorMessages;
            GeneralMessages = generalMessages;
            
            RuleFor(obj => obj.Team)
                .Must(x => x != null)
                .WithMessage(string.Format(ErrorMessages.PropRequired.GetValueForLanguage(), GeneralMessages.TeamCode.GetValueForLanguage()));
            RuleFor(obj => obj.Position)
                .MaximumLength(20)
                .WithMessage(x => string.Format(ErrorMessages.ValidationValueToLong.GetValueForLanguage(), GeneralMessages.PositionCode.GetValueForLanguage(), 20))
                .Must(x => x != null)
                .WithMessage(string.Format(ErrorMessages.PropRequired.GetValueForLanguage(), GeneralMessages.TeamCode.GetValueForLanguage()));
            RuleFor(obj => obj.ExtraInfo)
                .MaximumLength(20)
                .WithMessage(x => string.Format(ErrorMessages.ValidationValueToLong.GetValueForLanguage(),GeneralMessages.ExtraInfo.GetValueForLanguage(), 20));

            RuleFor(obj => obj)
                .MustAsync(HaveActiveMixInChannel)
                .WithMessage(ErrorMessages.NoActiveSessionFoundInChannel.GetValueForLanguage())
                .MustAsync(UserNotActiveInOtherChannelInSameMixGroup)
                .WithMessage(ErrorMessages.UserActiveInOtherMix.GetValueForLanguage())
                .MustAsync(WithinValidHours)
                .WithMessage(ErrorMessages.RegistrationWithinValidHours.GetValueForLanguage())
                .MustAsync(NotSnipingAgain)
                .WithMessage(z => 
                    string.Format(ErrorMessages.NotSnipingAgain.GetValueForLanguage()
                        , DateTimeHelper.ConvertDateTimeToApplicationTimeZone(z.UserMember.Snipers.First().DateEnd, z.Server.TimeZoneName).ToString()
                        , z.Server.ServerSnipings.First().IntervalAfterRegistrationOpeningInMinutes
                        , z.Server.ServerSnipings.First().SignUpDelayInMinutes)
                    );
        }

        private async Task<bool> HaveActiveMixInChannel(SignInDto dto, CancellationToken cancellationToken)
        {
            return await _mixChannelManager.HasActiveMixChannelByDiscordIdsAsync(dto.DiscordServerId.ToString(), dto.DiscordChannelId.ToString());
        }

        private async Task<bool> UserNotActiveInOtherChannelInSameMixGroup(SignInDto dto, CancellationToken cancellationToken)
        {
            //This is so wrong --> is always true now
            userNotAlreadyActiveInOtherMixChannel = !hasChannel || await _mixChannelManager.UserCanSignIntoMixChannelAsync(dto.DiscordServerId.ToString(), dto.DiscordChannelId.ToString(), dto.UserId);
            return userNotAlreadyActiveInOtherMixChannel;
        }

        private async Task<bool> WithinValidHours(SignInDto dto, CancellationToken cancellationToken)
        {
            mixGroupIdAndRegistrationTime ??= await _mixSessionManager.HasOpenMixSessionByDiscordIds(dto.DiscordServerId.ToString(), dto.DiscordChannelId.ToString(), dto.DbSignInTime);

            withinValidHours = !userNotAlreadyActiveInOtherMixChannel || await _mixSessionWorkflow.ValidateWithinValidHours(mixGroupIdAndRegistrationTime, dto.RoleIds, dto.DbSignInTime);
            return withinValidHours;
        }

        private async Task<bool> NotSnipingAgain(SignInDto dto, CancellationToken cancellationToken)
        {
            mixGroupIdAndRegistrationTime ??= await _mixSessionManager.HasOpenMixSessionByDiscordIds(dto.DiscordServerId.ToString(), dto.DiscordChannelId.ToString(), dto.DbSignInTime);
            return !withinValidHours || _mixSessionWorkflow.ValidateIsNotSnipingAgain(mixGroupIdAndRegistrationTime, dto.UserMember, dto.Server, dto.DbSignInTime);
        }
    }
}

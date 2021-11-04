using FluentValidation;
using System.Threading;
using System.Threading.Tasks;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Business.Dto.Mix
{
    public class SignInDtoValidator : AbstractValidator<SignInDto>
    {
        private readonly IMixChannelManager _mixChannelManager;
        private readonly IMixSessionManager _mixSessionManager;
        private readonly IMixGroupRoleOpeningWorkflow _mixGroupRoleOpeningWorkflow;
        private readonly IMixSessionWorkflow _mixSessionWorkflow;
        private readonly ErrorMessages ErrorMessages;
        private readonly GeneralMessages GeneralMessages;

        private bool hasChannel { get; set; }
        private bool userNotAlreadyActiveInOtherMixChannel { get; set; }

        public SignInDtoValidator(IMixChannelManager mixChannelManager, IMixSessionManager mixSessionManager, IMixGroupRoleOpeningWorkflow mixGroupRoleOpeningWorkflow, IMixSessionWorkflow mixSessionWorkflow, ErrorMessages errorMessages, GeneralMessages generalMessages)
        {
            CascadeMode = CascadeMode.Stop;
            _mixChannelManager = mixChannelManager;
            _mixSessionManager = mixSessionManager;
            _mixGroupRoleOpeningWorkflow = mixGroupRoleOpeningWorkflow;
            _mixSessionWorkflow = mixSessionWorkflow;
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
                .WithMessage(ErrorMessages.RegistrationWithinValidHours.GetValueForLanguage());
        }

        private async Task<bool> HaveActiveMixInChannel(SignInDto dto, CancellationToken cancellationToken)
        {
            return await _mixChannelManager.HasActiveMixChannelByDiscordIdsAsync(dto.DiscordServerId.ToString(), dto.DiscordChannelId.ToString());
        }

        private async Task<bool> UserNotActiveInOtherChannelInSameMixGroup(SignInDto dto, CancellationToken cancellationToken)
        {
            userNotAlreadyActiveInOtherMixChannel = !hasChannel || await _mixChannelManager.UserCanSignIntoMixChannelAsync(dto.DiscordServerId.ToString(), dto.DiscordChannelId.ToString(), dto.UserId);
            return userNotAlreadyActiveInOtherMixChannel;
        }

        private async Task<bool> WithinValidHours(SignInDto dto, CancellationToken cancellationToken)
        {
            if (!userNotAlreadyActiveInOtherMixChannel)
                return true;

            return await _mixSessionWorkflow.ValidateWithinValidHours(dto.DiscordServerId.ToString(), dto.DiscordChannelId.ToString(), dto.RoleIds);
        }
    }
}

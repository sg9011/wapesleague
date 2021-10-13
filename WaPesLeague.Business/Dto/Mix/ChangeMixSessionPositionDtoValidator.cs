using FluentValidation;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Entities.Position.Constants;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Business.Dto.Mix
{
    public class ChangeMixSessionPositionDtoValidator : AbstractValidator<ChangeMixSessionPositionDto>
    {
        private readonly IMixChannelManager _mixChannelManager;
        private readonly IPositionManager _positionManager;
        private readonly ErrorMessages ErrorMessages;

        public ChangeMixSessionPositionDtoValidator(IMixChannelManager mixChannelManager, IPositionManager positionManager, ErrorMessages errorMessages)
        {
            _mixChannelManager = mixChannelManager;
            _positionManager = positionManager;
            ErrorMessages = errorMessages;

            CascadeMode = CascadeMode.Stop;
            RuleFor(obj => obj.OldPosition)
                .NotEmpty()
                .WithMessage(ErrorMessages.OldPositionEmpty.GetValueForLanguage());

            RuleFor(obj => obj.NewPosition)
                .NotEmpty()
                .WithMessage(ErrorMessages.NewPositionEmpty.GetValueForLanguage());

             RuleFor(obj => obj)
                .MustAsync(PositionExistsAsync)
                .WithMessage(x => $" {ErrorMessages.ProvidedPositionUnknown.GetValueForLanguage()} {x}");

            RuleFor(obj => obj.Team)
                .NotEmpty()
                .WithMessage(ErrorMessages.ProvideTeam.GetValueForLanguage());

            RuleFor(obj => obj)
                .MustAsync(PositionsAreNotGKAsync)
                .WithMessage(ErrorMessages.UpdatePositionGK.GetValueForLanguage())
                .MustAsync(HaveActiveMixInChannelForTeamAndPosition)
                .WithMessage(x => string.Format(ErrorMessages.OldPositionNotFoundInTeam.GetValueForLanguage(), x.OldPosition, x.Team));
        }

        private async Task<bool> HaveActiveMixInChannelForTeamAndPosition(ChangeMixSessionPositionDto dto, CancellationToken cancellationToken)
        {
            return await _mixChannelManager.HasActiveMixChannelByDiscordIdsAndTeamAndPositionAsync(dto.DiscordServerId.ToString(), dto.DiscordChannelId.ToString(), dto.Team, dto.OldPosition);
        }

        private async Task<bool> PositionExistsAsync(ChangeMixSessionPositionDto dto, CancellationToken cancellationToken)
        {
            return (await _positionManager.GetPostionByTagOrCodeAsync(dto.NewPosition, dto.ServerId)) != null;
        }

        private async Task<bool> PositionsAreNotGKAsync(ChangeMixSessionPositionDto dto, CancellationToken cancellationToken)
        {
            var goalKeeper = await _positionManager.GetPostionByTagOrCodeAsync(PositionConstants.Group.Goalkeeper, dto.ServerId);
            var tagsAndCodes = (new List<string> { goalKeeper.Code }).Union(goalKeeper.Tags.Select(t => t.Tag));
            return tagsAndCodes.All(tc => !string.Equals(dto.NewPosition, tc, StringComparison.InvariantCultureIgnoreCase)
                && !string.Equals(dto.OldPosition, tc, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}

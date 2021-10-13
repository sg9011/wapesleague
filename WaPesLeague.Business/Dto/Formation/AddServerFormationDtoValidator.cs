using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Entities.Position.Constants;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Business.Dto.Formation
{
    public class AddServerFormationDtoValidator : AbstractValidator<AddServerFormationDto>
    {
        private readonly IServerFormationManager _serverFormationManager;
        private readonly IPositionManager _positionManager;
        private readonly ErrorMessages ErrorMessages;

        public AddServerFormationDtoValidator(IServerFormationManager serverFormationManager, IPositionManager positionManager, ErrorMessages errorMessages)
        {
            _serverFormationManager = serverFormationManager;
            _positionManager = positionManager;
            ErrorMessages = errorMessages;

            RuleFor(obj => obj.FormationName)
                .NotEmpty()
                .WithMessage(ErrorMessages.FormationNameRequired.GetValueForLanguage());

            RuleFor(obj => obj)
                .MustAsync(UniqueFormationNameAsync)
                .WithMessage(ErrorMessages.UniqueFormationName.GetValueForLanguage());

            RuleFor(obj => obj.Positions)
                .NotNull()
                .Must(x => x.Count == 11)
                .WithMessage(ErrorMessages.FormationPositionCount.GetValueForLanguage());

            RuleFor(obj => obj)
                .MustAsync(PositionsMustExistAsync)
                .WithMessage(ErrorMessages.FormationPositionUnknown.GetValueForLanguage());

            RuleFor(obj => obj)
                .MustAsync(ValidAmountOfGkAsync)
                .WithMessage(ErrorMessages.FormationPosition1GK.GetValueForLanguage());
        }

        public async Task<bool> UniqueFormationNameAsync(AddServerFormationDto dto, CancellationToken cancellationToken)
        {
            return !(await _serverFormationManager.HasFormationWithTagOrNameAsync(dto.FormationName, dto.DiscordServerId.ToString()));
        }

        public async Task<bool> PositionsMustExistAsync(AddServerFormationDto dto, CancellationToken cancellationToken)
        {
            var allPostions = await _positionManager.GetAllPositionsWithTagsAsync(dto.ServerId);
            var allCodesAndTags = allPostions.Select(p => p.Code).Union(allPostions.SelectMany(p => p.Tags.Select(t => t.Tag))).ToList();
            return dto.Positions.All(providedPosition => allCodesAndTags.Any(x => string.Equals(x, providedPosition, StringComparison.InvariantCultureIgnoreCase)));
        }

        public async Task<bool> ValidAmountOfGkAsync(AddServerFormationDto dto, CancellationToken cancellationToken)
        {
            var goalKeeper = await _positionManager.GetPostionByTagOrCodeAsync(PositionConstants.Group.Goalkeeper, dto.ServerId);
            var tagsAndCodes = new List<string> { goalKeeper.Code };
            tagsAndCodes.AddRange(goalKeeper.Tags.Select(t => t.Tag));
            var gkPositions = dto.Positions.Where(providedPosition => tagsAndCodes.Any(t => string.Equals(t, providedPosition, StringComparison.InvariantCultureIgnoreCase))).ToList();
            return gkPositions.Count == 1;
        }
    }
}

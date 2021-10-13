using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto.Position;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;
using WaPesLeague.Data.Entities.Position;
using WaPesLeague.Data.Managers.Interfaces;

namespace WaPesLeague.Business.Workflows
{
    public class PositionWorkflow : BaseWorkflow<PositionWorkflow>, IPositionWorkflow
    {
        private readonly IPositionManager _positionManager;
        private readonly IPositionTagManager _positionTagManager;
        public PositionWorkflow(IPositionManager positionManager, IPositionTagManager positionTagManager, IMemoryCache memoryCache, IMapper mapper, ILogger<PositionWorkflow> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(memoryCache, mapper, logger, errorMessages, generalMessages)
        {
            _positionManager = positionManager;
            _positionTagManager = positionTagManager;
        }

        public async Task<IReadOnlyCollection<PositionDto>> GetAllPositionsOrderedAsync(int? serverId = null)
        {
            var positions = await _positionManager.GetAllPositionsAsync(serverId);
            var mappedPostions = Mapper.Map<IReadOnlyCollection<PositionDto>>(positions);

            return mappedPostions.OrderBy(mp => mp.PositionGroupOrder).ThenBy(mp => mp.PositionOrder).ToList();
        }

        public async Task<DiscordWorkflowResult> AddPositionTagAsync(string positionCode, string tag, int serverId)
        {
            var allPostions = await _positionManager.GetAllPositionsAsync(serverId);
            var tagPosition = allPostions.SingleOrDefault(x => 
                string.Equals(x.Code, tag, StringComparison.InvariantCultureIgnoreCase)
                || x.Tags.Any(t => string.Equals(t.Tag, tag, StringComparison.InvariantCultureIgnoreCase)));

            var codePosition = allPostions.SingleOrDefault(x =>
                string.Equals(x.Code, positionCode, StringComparison.InvariantCultureIgnoreCase)
                || x.Tags.Any(t => string.Equals(t.Tag, positionCode, StringComparison.InvariantCultureIgnoreCase)));

            if (codePosition == null)
                return new DiscordWorkflowResult($"{ErrorMessages.ProvidedPositionUnknown.GetValueForLanguage()} {positionCode}", false);

            if (tagPosition != null)
            {
                if (tagPosition.PositionId == codePosition.PositionId)
                    return new DiscordWorkflowResult(GeneralMessages.NothingChanged.GetValueForLanguage(), true);
                else
                    return new DiscordWorkflowResult(string.Format(ErrorMessages.TagExists.GetValueForLanguage(), tag, codePosition.Display()), false);
            }

            var positionTag = new PositionTag()
            {
                PositionId = codePosition.PositionId,
                ServerId = serverId,
                IsDisplayValue = false,
                Tag = tag.ToUpper()
            };

            _ = await _positionTagManager.AddAsync(positionTag);

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
        }

        public async Task<DiscordWorkflowResult> RemovePositionTagAsync(string tag, int serverId)
        {
            var tagPosition = await _positionManager.GetPostionByTagOrCodeAsync(tag, serverId);

            if (tagPosition == null)
                return new DiscordWorkflowResult($"{ErrorMessages.ProvidedPositionUnknown.GetValueForLanguage()} {tag}", false);

            if (string.Equals(tagPosition.Code, tag, StringComparison.InvariantCultureIgnoreCase))
                return new DiscordWorkflowResult(ErrorMessages.NotAllowedToDeletePositionCode.GetValueForLanguage(), false);

            var positionTag = tagPosition.Tags.Single(t => string.Equals(t.Tag, tag, StringComparison.InvariantCultureIgnoreCase));
            await _positionTagManager.DeleteAsync(positionTag.PositionTagId);

            return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
        }

        public async Task<DiscordWorkflowResult> SetPositionDisplayAsync(string positionCode, string displayValue, int serverId)
        {
            //Should Add the tag if not exists
            //Should put this new or existing tag as server pos display tag
            //If exists for other pos then fail.
            //if tag equals code remove isDisplay from other tags
            var allPostions = await _positionManager.GetAllPositionsAsync(serverId);

            var displayPosition = allPostions.SingleOrDefault(x =>
                string.Equals(x.Code, displayValue, StringComparison.InvariantCultureIgnoreCase)
                || x.Tags.Any(t => string.Equals(t.Tag, displayValue, StringComparison.InvariantCultureIgnoreCase)));

            var codePosition = allPostions.SingleOrDefault(x =>
                string.Equals(x.Code, positionCode, StringComparison.InvariantCultureIgnoreCase)
                || x.Tags.Any(t => string.Equals(t.Tag, positionCode, StringComparison.InvariantCultureIgnoreCase)));

            if (codePosition == null)
                return new DiscordWorkflowResult($"{ErrorMessages.ProvidedPositionUnknown.GetValueForLanguage()} {positionCode}", false);

            if (displayPosition == null)
            {
                var positionTag = new PositionTag()
                {
                    PositionId = codePosition.PositionId,
                    ServerId = serverId,
                    IsDisplayValue = true,
                    Tag = displayValue.ToUpper()
                };

                await SetAllPostionTagsToIsNotDisplayValue(codePosition.Tags?.Where(x => x.IsDisplayValue == true));

                _ = await _positionTagManager.AddAsync(positionTag);
                return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
            }

            if (codePosition.PositionId == displayPosition.PositionId)
            {
                if (string.Equals(codePosition.Code, displayValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    await SetAllPostionTagsToIsNotDisplayValue(displayPosition.Tags?.Where(x => x.IsDisplayValue == true));
                    return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
                }

                var displayTag = displayPosition.Tags?.Single(t => string.Equals(t.Tag, displayValue, StringComparison.InvariantCultureIgnoreCase));
                if (displayTag.IsDisplayValue == false)
                {
                    displayTag.IsDisplayValue = true;
                    await _positionTagManager.UpdateAsync(displayTag);
                }
                await SetAllPostionTagsToIsNotDisplayValue(displayPosition.Tags?.Where(x => x.IsDisplayValue == true && x.PositionTagId != displayTag.PositionTagId));

                return new DiscordWorkflowResult(GeneralMessages.AppliedChanges.GetValueForLanguage(), true);
            }

            return new DiscordWorkflowResult(string.Format(ErrorMessages.CodeAndDisplayValueAreLinkedToDifferentPositions.GetValueForLanguage(),
                positionCode, codePosition.Display(), displayValue, displayPosition.Display()), false);
        }

        private async Task SetAllPostionTagsToIsNotDisplayValue(IEnumerable<PositionTag> positionTags)
        {
            foreach (var posTag in positionTags)
            {
                posTag.IsDisplayValue = false;
                _ = await _positionTagManager.UpdateAsync(posTag);
            }
        }
    }
}

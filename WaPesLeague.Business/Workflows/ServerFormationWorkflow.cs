using System;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaPesLeague.Business.Dto.Formation;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Business.Workflows.Interfaces;
using WaPesLeague.Data.Entities.Formation;
using WaPesLeague.Data.Managers.Interfaces;
using Microsoft.Extensions.Logging;
using WaPesLeague.Business.Dto;
using WaPesLeague.Constants;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Business.Workflows
{
    public class ServerFormationWorkflow : BaseWorkflow<ServerFormationWorkflow>, IServerFormationWorkflow
    {
        private readonly IServerFormationManager _serverformationManager;
        private readonly IPositionManager _positionManager;
        private readonly IServerWorkflow _serverWorkflow;
        public ServerFormationWorkflow(IServerFormationManager serverformationManager, IPositionManager positionManager, IServerWorkflow serverWorkflow,
            IMemoryCache memoryCache, IMapper mapper, ILogger<ServerFormationWorkflow> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
            : base(memoryCache, mapper, logger, errorMessages, generalMessages)
        {
            _serverformationManager = serverformationManager;
            _positionManager = positionManager;
            _serverWorkflow = serverWorkflow;
        }

        public async Task<IReadOnlyCollection<FormationDto>> GetAllServerFormationsAsync(DiscordCommandPropsDto propsDto)
        {
            var formations = await _serverformationManager.GetAllFormationsByDiscordServerIdAsync(propsDto.ServerId.ToString());
            var mappedFormations = Mapper.Map<IReadOnlyCollection<FormationDto>>(formations);

            return mappedFormations;
        }

        public async Task<DiscordWorkflowResult> AddServerFormationAsync(AddServerFormationDto addServerFormationDto)
        {
            var server = await _serverWorkflow.GetOrCreateServerAsync(addServerFormationDto.DiscordServerId, addServerFormationDto.DiscordServerName);

            var validator = new AddServerFormationDtoValidator(_serverformationManager, _positionManager, ErrorMessages);
            var validationResults = await validator.ValidateAsync(addServerFormationDto);
            if (validationResults.Errors.Any())
                return HandleValidationResults(validationResults);
            var allPostions = await _positionManager.GetAllPositionsWithTagsAsync(addServerFormationDto.ServerId);
            var serverFormationPostions = addServerFormationDto.Positions.Select(p => 
                new ServerFormationPosition()
                {
                    PositionId = allPostions.Single(ap => 
                        string.Equals(ap.Code, p, StringComparison.InvariantCultureIgnoreCase)
                        || ap.Tags.Any(t => string.Equals(t.Tag, p, StringComparison.InvariantCultureIgnoreCase))).PositionId
                }).ToList();
            var serverformation = new ServerFormation()
            {
                Name = addServerFormationDto.FormationName,
                IsDefault = false,
                Positions = serverFormationPostions,
                ServerId = server.ServerId
            };

            serverformation = await _serverformationManager.AddAsync(serverformation);
            MemoryCache.Remove(Cache.GetServerId(addServerFormationDto.DiscordServerId).ToUpperInvariant());

            return new DiscordWorkflowResult($"{GeneralMessages.NewFormationAddedResponse.GetValueForLanguage()}: {addServerFormationDto.FormationName}");
        }

        public async Task<DiscordWorkflowResult> SetDefaultServerFormationAsync(string formationName, DiscordCommandPropsDto propsDto)
        {
            var formations = await _serverformationManager.GetAllFormationsByDiscordServerIdAsync(propsDto.ServerId.ToString());
            var formationToUpdate = formations.SingleOrDefault(f => string.Equals(f.Name, formationName, StringComparison.InvariantCultureIgnoreCase));
            if (formationToUpdate == null)
                return new DiscordWorkflowResult(string.Format(ErrorMessages.FormationUnknown.GetValueForLanguage(), formationName), false);

            if (formationToUpdate.IsDefault)
                return new DiscordWorkflowResult(GeneralMessages.NothingChanged.GetValueForLanguage(), true);

            var defaultFormation = formations.Single(f => f.IsDefault);
            defaultFormation.IsDefault = false;
            formationToUpdate.IsDefault = true;
            await _serverformationManager.UpdateAsync(Mapper.Map<ServerFormation>(defaultFormation));
            await _serverformationManager.UpdateAsync(Mapper.Map<ServerFormation>(formationToUpdate));

            MemoryCache.Remove(Cache.GetServerId(propsDto.ServerId).ToUpperInvariant());

            return new DiscordWorkflowResult(string.Format(GeneralMessages.UpdatedDefaultServerFormation.GetValueForLanguage(), defaultFormation.Name, formationToUpdate.Name));
        }
    }
}

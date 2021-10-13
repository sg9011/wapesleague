using AutoMapper;
using FluentValidation.Results;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Text;
using WaPesLeague.Business.Helpers;
using WaPesLeague.Constants.Resources;

namespace WaPesLeague.Business.Workflows
{
    public abstract class BaseWorkflow<T> where T : BaseWorkflow<T>
    {
        protected IMemoryCache MemoryCache;
        protected readonly IMapper Mapper;
        protected readonly ILogger<T> Logger;
        protected readonly ErrorMessages ErrorMessages;
        protected readonly GeneralMessages GeneralMessages;

        public BaseWorkflow(IMemoryCache memoryCache, IMapper mapper, ILogger<T> logger, ErrorMessages errorMessages, GeneralMessages generalMessages)
        {
            MemoryCache = memoryCache;
            Mapper = mapper;
            Logger = logger;
            ErrorMessages = errorMessages;
            GeneralMessages = generalMessages;
        }

        protected DiscordWorkflowResult HandleValidationResults(ValidationResult validationResult)
        {
            var errorMessageBuilder = new StringBuilder();
            foreach (var error in validationResult.Errors)
            {
                errorMessageBuilder.AppendLine(error.ErrorMessage);
            }
            return new DiscordWorkflowResult(errorMessageBuilder.ToString(), false);
        }
    }
}

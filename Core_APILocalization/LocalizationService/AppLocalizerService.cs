using System.Reflection;
using Core_APILocalization.Resources;
using Microsoft.Extensions.Localization;

namespace Core_APILocalization.LocalizationService
{
    public class AppLocalizerService : IAppLocalizerService
    {
        private readonly IStringLocalizer stringLocalizer;
        private readonly ILogger<AppLocalizerService> logger;

        public AppLocalizerService(IStringLocalizerFactory factory, ILogger<AppLocalizerService> logger)
        {
            this.logger = logger;
            logger.LogInformation($"Accept-Language Header ");
            var type = typeof(SharedResource);
            var assembly = type.GetTypeInfo().Assembly;
            if (assembly != null && !string.IsNullOrEmpty(assembly.FullName))
            {
                var assemblyName = new AssemblyName(assembly.FullName);
                stringLocalizer = factory.Create("SharedResource", assemblyName?.Name ?? throw new ArgumentNullException(nameof(assemblyName.Name)));
                logger.LogInformation($"Using resource assembly: {assemblyName.Name}");
            }
            else
            {
                throw new ArgumentNullException(nameof(assembly));
            }
        }

        public string GetLocalizationString(string key)
        {
            var localizedString = stringLocalizer[key];

            foreach (var culture in stringLocalizer.GetAllStrings())
            {
                logger.LogInformation($"Culture: {culture}");
            }


            logger.LogInformation($"Localized string for key '{key}': {localizedString}");
            return localizedString;
        }
    }
}


using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace LonerApp.Helpers.Extensions
{
    [AcceptEmptyServiceProvider]
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        public string Text { get; set; } = "";

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return I18nHelper.Get(Text);
        }
    }

    public static class I18nHelper 
    {
        const string ResourceId = "LonerApp.Resources.Languages.Resources";

        static ResourceManager resourceManager = new ResourceManager(ResourceId,
                                Assembly.GetExecutingAssembly());
        
        public static string Get(string key)
        {
            // TODO: Get current setting language, default is vi
            // var currentLanguage = UserSettingsGroup.Get(StorageKeys.Language) ?? "vi";
            var currentLanguage = "vi";
            var currentInfor = new CultureInfo(currentLanguage);

            var translation = resourceManager.GetString(key, currentInfor);

            if (translation == null)
            {
#if DEBUG
                throw new ArgumentException(
                    $"Key '{key}' was not found in resources '{ResourceId}' for culture '{currentInfor.Name}'");
#else
                translation = key; 
#endif
            }

            return translation;
        }
    }
}
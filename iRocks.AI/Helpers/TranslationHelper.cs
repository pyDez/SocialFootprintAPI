using iRocks.AI.Helpers.Localisation;
using iRocks.DataLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace iRocks.AI
{
    public static class TranslationHelper
    {
        public static Category GetCategoryTranslation(Category category, string locale)
        {

            var label = category.Labels.Where(l => l.Locale == locale);
            if (label.Any())
                category.Label = label.First().Label;
            return category;
        }
        public static Badge GetBadgeTranslation(Badge badge, string locale)
        {

            var translation = badge.Labels.Where(l => l.Locale == locale);
            if (translation.Any())
            {
                badge.Label = translation.First().Label;
                badge.Explanation = translation.First().Explanation;
            }
            return badge;
        }
        

        public static string GetTranslation(string locale, string key)
        {
            string language = "en";
            if (!string.IsNullOrWhiteSpace(locale)) {
                language = locale.Split('_').First();
            }
            switch(language.ToLowerInvariant())
            {
                case "en":
                    return translation_en.ResourceManager.GetString(key);
                case "fr":
                    return translation_fr.ResourceManager.GetString(key);
                default:
                    return translation_en.ResourceManager.GetString(key);

            }
        }
    }
}

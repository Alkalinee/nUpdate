// Author: Dominic Beger (Trade/ProgTrade)

using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace nUpdate.Localization
{
    internal class LocalizationHelper
    {
        /// <summary>
        ///     Returns the localized values for the given enumeration objects.
        /// </summary>
        /// <param name="properties">The <see cref="LocalizationProperties" />-instance to use for the localization.</param>
        /// <param name="objects">The objects for the localization.</param>
        /// <returns>Returns the found localizations.</returns>
        public static IEnumerable<string> GetLocalizedEnumerationValues(LocalizationProperties properties,
            object[] objects)
        {
            foreach (var o in objects)
            {
                var fieldInfo = o.GetType().GetField(o.ToString());
                var descriptionAttributes =
                    (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (descriptionAttributes.Length > 0)
                {
                    var resourceId = descriptionAttributes[0].Description;
                    yield return
                        (string)
                            properties.GetType()
                                .GetProperties()
                                .First(x => x.Name == resourceId)
                                .GetValue(properties, null);
                }
                else
                {
                    yield return o.ToString();
                }
            }
        }

        public static CultureInfo[] IntegratedCultures => new[] { new CultureInfo("de-AT"), new CultureInfo("de-CH"), new CultureInfo("de-DE"), new CultureInfo("en"), new CultureInfo("fr-FR") };

        public static bool IsIntegratedCulture(CultureInfo cultureInfo)
        {
            return IntegratedCultures.Contains(cultureInfo);
        }

        public LocalizationProperties GetLocaizationProperties(CultureInfo cultureInfo)
        {
            string resourceName = $"nUpdate.Localization.{cultureInfo.Name}.json";
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                return Serializer.Deserialize<LocalizationProperties>(stream);
            }
        }
    }
}
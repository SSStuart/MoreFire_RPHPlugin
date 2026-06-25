using Rage;
using System.Collections.Generic;
using System.Globalization;

namespace DeleteThatEntity
{
    public class Localization
    {
        private readonly string locale;
        private readonly Dictionary<string, Dictionary<string, string>> strings = new Dictionary<string, Dictionary<string, string>>
        {
            {"en",
                new Dictionary<string, string>{
                    { "updateAvailable", "~y~Update available!" },
                }
            },
            {"fr",
                new Dictionary<string, string>{
                    { "updateAvailable", "~y~Mise à jour disponible !" },
                }
            }
        };

        public Localization(string locale = "auto")
        {
            if (locale == "auto")
                locale = CultureInfo.InstalledUICulture.Name.Split('-')[0];
            else
                locale = locale.Split('-')[0];

            if (!strings.ContainsKey(locale))
                locale = "en";
            Game.LogTrivial($"Localization: Using locale '{locale.ToUpper()}'");

            this.locale = locale;
        }

        public string GetString(string key, params (string key, object value)[] replace)
        {
            string localizedString;
            if (strings[this.locale].ContainsKey(key))
                localizedString = strings[this.locale][key];
            else
            {
                localizedString = key;
                Game.LogTrivial($"Localization: Missing translation for key '{key}'");
            }

            foreach (var replacement in replace)
            {
                localizedString = localizedString.Replace($":{replacement.key}", replacement.value?.ToString() ?? "");
            }

            return localizedString;
        }
    }
}

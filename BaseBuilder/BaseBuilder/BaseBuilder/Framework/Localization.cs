using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BaseBuilder
{
    /// <summary>
    /// Languages available in the game
    /// </summary>
    public enum Languages
    {
        English
    }

    /// <summary>
    /// Usage example: Localization.Get("text_key");
    /// </summary>
    public static class Localization
    {
        private const string MISSING_STRING = "**MISSING STRING**";

        private static Languages _language = Languages.English;
        private static Dictionary<string, string> _textValues;

        static Localization()
        {
            _textValues = new Dictionary<string, string>();
        }

        /// <summary>
        /// Load the localization file of the currently selected language
        /// </summary>
        public static void LoadLocalization()
        {
            if (File.Exists("Localization.csv"))
            {
                string[] lines = File.ReadAllLines("Localization.csv");

                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line) == false)
                    {
                        string[] split = line.Split(',');
                        string key = split[0];
                        string textValue = split[1];
                        _textValues.Add(key, textValue);
                    }
                }
            }
            else
            {
                // TODO: Add an error message for the localization file not being found
            }
        }

        /// <summary>
        /// Get a localized text value from a key
        /// </summary>
        /// <param name="key">The key of the text value</param>
        /// <returns>A localized string</returns>
        public static string Get(string key)
        {
            if (_textValues.ContainsKey(key))
            {
                return _textValues[key];
            }
            else
            {
                return MISSING_STRING;
            }
        }

        /// <summary>
        /// The currently loaded language the game is using
        /// </summary>
        public static Languages Language
        {
            get
            {
                return _language;
            }
            set
            {
                if (value != _language)
                {
                    // Only load the loc if it's a different language being requested compared to the current
                    _language = value;
                    LoadLocalization();
                }
            }
        }
    }
}

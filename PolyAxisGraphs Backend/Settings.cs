using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PolyAxisGraphs_Backend
{
    /// <summary>
    /// currently opened settings file.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// error thrown if settings file not found.
        /// </summary>
        public class SettingsFileNotFoundError : Exception 
        {
            /// <summary>
            /// throw error if settings file not found.
            /// </summary>
            public SettingsFileNotFoundError() : base("settings file not found.") { }
        }

        /// <summary>
        /// key value pairs of settings
        /// </summary>
        private Dictionary<string, string> SettingPairs { get; set; }

        /// <summary>
        /// currently opened language
        /// </summary>
        public LanguagePack? currentlang { get; set; }

        /// <summary>
        /// path to opened file.
        /// </summary>
        private string file { get; set; }

        /// <summary>
        /// read settings from settings file.
        /// </summary>
        /// <param name="_file">path to settings file.</param>
        public Settings(string _file) 
        {
            file = _file;
            SettingPairs = new Dictionary<string, string>();
            ReadSettings();
            var def = FindValueFromKey("defaultlanguagepath");
            if (def != null) currentlang = new LanguagePack(def);
        }

        /// <summary>
        /// create default settings file
        /// </summary>
        /// <param name="_file"></param>
        public static void CreateDefault()
        {
            if (File.Exists("Settings.ini"))
            {
                File.Delete("Settings.ini");
            }

            using (StreamWriter writer = new StreamWriter("Settings.ini", append: true))
            {
                writer.WriteLine("#Settings for Multiple Y Axis Data");
                writer.WriteLine("#");
                writer.WriteLine("#initial directory, directory of input data files and saved files");
                writer.WriteLine("initialdirectory=" + "DataFiles");
                writer.WriteLine("#");
                writer.WriteLine("#default language, language file of default language");
                writer.WriteLine("defaultlanguagepath=" + @"LanguageFiles\EN.lng");
                writer.WriteLine("#");
                writer.WriteLine("controlfontsize=" + 15);
                writer.WriteLine("chartfontsize=" + 10);
                writer.WriteLine("charttitlefontsize=" + 20);
                writer.WriteLine("fontfamily=" + "Consolas");
                writer.WriteLine("chartgridinterval=" + 20);
                writer.WriteLine("yaxiswidth=" + 30);
            }
        }

        /// <summary>
        /// read settings from settingsfile.
        /// </summary>
        /// <exception cref="SettingsFileNotFoundError"></exception>
        private void ReadSettings()
        {
            if (File.Exists(file))
            {
                foreach(string line in File.ReadAllLines(file))
                {
                    if(line != "")
                    {
                        if (line[0] != '#' && line.Contains("="))
                        {
                            string[] strings = line.Split('=');
                            SettingPairs[strings[0]] = strings[1];
                        }
                    }
                }
            }
            else
            {
                throw new SettingsFileNotFoundError();
            }
        }

        /// <summary>
        /// find value from key in settings
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string? FindValueFromKey(string key)
        {
            try
            {
                return SettingPairs[key];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// save settings with current values.
        /// </summary>
        public void SaveSettings() 
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }

            using(StreamWriter writer = new StreamWriter(file, append: true))
            {
                writer.WriteLine("#Settings for Multiple Y Axis Data");
                writer.WriteLine("#");
                writer.WriteLine("#initial directory, directory of input data files and saved files");
                writer.WriteLine("initialdirectory=" + FindValueFromKey("initialdirectory"));
                writer.WriteLine("#");
                writer.WriteLine("#default language, language file of default language");
                writer.WriteLine("defaultlanguagepath=" + FindValueFromKey("defaultlanguagepath"));
                writer.WriteLine("#");
                writer.WriteLine("controlfontsize=" + FindValueFromKey("controlfontsize"));
                writer.WriteLine("chartfontsize=" + FindValueFromKey("chartfontsize"));
                writer.WriteLine("charttitlefontsize=" + FindValueFromKey("charttitlefontsize"));
                writer.WriteLine("fontfamily=" + FindValueFromKey("fontfamily"));
                writer.WriteLine("chartgridinterval=" + FindValueFromKey("chartgridinterval"));
                writer.WriteLine("yaxiswidth=" + FindValueFromKey("yaxiswidth"));
            }

            ReadSettings();
        }
    }
}

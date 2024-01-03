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
        /// initial directory of data files.
        /// </summary>
        public string? initialdirectory { get; set; }
        /// <summary>
        /// default language pack file.
        /// </summary>
        public LanguagePack? defaultlang { get; set; }
        /// <summary>
        /// current language pack file.
        /// </summary>
        public LanguagePack? currentlang { get; set; }
        /// <summary>
        /// fontsize of text on controls.
        /// </summary>
        public int? controlfontsize { get; set; }
        /// <summary>
        /// fontsize of text on chart.
        /// </summary>
        public int? chartfontsize { get; set; }
        /// <summary>
        /// fontsize of title on chart.
        /// </summary>
        public int? charttitlefontsize { get; set; }
        /// <summary>
        /// font family.
        /// </summary>
        public string? fontfamily { get; set; }
        /// <summary>
        /// number of divisions of chart grid.
        /// </summary>
        public int? chartgridinterval { get; set; }
        /// <summary>
        /// width of one y axis of chart in pixel.
        /// </summary>
        public int? yaxiswidth { get; set; }

        /// <summary>
        /// path to opened file.
        /// </summary>
        public string file { get; set; }

        /// <summary>
        /// read settings from settings file.
        /// </summary>
        /// <param name="_file">path to settings file.</param>
        public Settings(string _file) 
        {
            file = _file;
            ReadSettings();
        }

        /// <summary>
        /// read settings from settingsfile.
        /// </summary>
        /// <exception cref="SettingsFileNotFoundError"></exception>
        public void ReadSettings()
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
                            switch(strings[0])
                            {
                                case "initialdirectory": initialdirectory = strings[1]; break;
                                case "defaultlanguage": defaultlang = new LanguagePack(strings[1]); currentlang = defaultlang; break;
                                case "controlfontsize": controlfontsize = int.Parse(strings[1]); break;
                                case "chartfontsize": chartfontsize = int.Parse(strings[1]); break;
                                case "charttitlefontsize": charttitlefontsize = int.Parse(strings[1]); break;
                                case "fontfamily": fontfamily = strings[1]; break;
                                case "chartgridinterval": chartgridinterval = int.Parse(strings[1]); break;
                                case "yaxiswidth": yaxiswidth = int.Parse(strings[1]); break;
                            }
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
        /// write settings with specified values.
        /// </summary>
        /// <param name="_initialdirectory">initial directory of data files.</param>
        /// <param name="_languagefile">default language file path.</param>
        /// <param name="_yaxiswidth">width of one y axis in pixel.</param>
        /// <param name="_controlfontsize">fontsize of text on controls.</param>
        /// <param name="_chartfontsize">fontsize of text on chart.</param>
        /// <param name="_charttitlefontsize">fontsize of title on chart.</param>
        /// <param name="_fontfamily">font family of displayed text.</param>
        /// <param name="_chartgridinterval">count of divisions of chart grid.</param>
        public void WriteSettings(string _initialdirectory, string _languagefile, string _yaxiswidth, string _controlfontsize, string _chartfontsize, string _charttitlefontsize, string _fontfamily, string _chartgridinterval) 
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
                writer.WriteLine("initialdirectory=" + _initialdirectory);
                writer.WriteLine("#");
                writer.WriteLine("#default language, language file of default language");
                writer.WriteLine("defaultlanguage=" + _languagefile);
                writer.WriteLine("#");
                writer.WriteLine("controlfontsize=" + _controlfontsize);
                writer.WriteLine("chartfontsize=" + _chartfontsize);
                writer.WriteLine("charttitlefontsize=" + _charttitlefontsize);
                writer.WriteLine("fontfamily=" + _fontfamily);
                writer.WriteLine("chartgridinterval=" + _chartgridinterval);
                writer.WriteLine("yaxiswidth=" + _yaxiswidth);
            }

            ReadSettings();
        }
    }
}

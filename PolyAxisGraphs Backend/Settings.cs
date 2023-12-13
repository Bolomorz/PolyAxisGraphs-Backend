using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PolyAxisGraphs_Backend
{
    public class Settings
    {
        public string? initialdirectory { get; set; }
        public LanguagePack? defaultlang { get; set; }
        public LanguagePack? currentlang { get; set; }
        public int? controlfontsize { get; set; }
        public int? chartfontsize { get; set; }
        public int? charttitlefontsize { get; set; }
        public string? fontfamily { get; set; }
        public int? chartgridinterval { get; set; }
        public int? yaxiswidth { get; set; }

        public string file { get; set; }

        public Settings(string _file) 
        {
            file = _file;
            ReadSettings();
        }

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
                throw new Exception("settings file not found");
            }
        }

        public void WriteSettings(string _initialdirectory, string _languagefile,string _yaxiswidth, string _controlfontsize, string _chartfontsize, string _charttitlefontsize, string _fontfamily, string _chartgridinterval) 
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

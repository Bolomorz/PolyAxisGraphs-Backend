using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyAxisGraphs_Backend
{
    public class LanguagePack
    {
        public struct LPPair
        {
            public string name { get; set; }
            public string? value { get; set; }
        }
        public string file { get; set; }
        public List<LPPair> strings { get; set; }
        public bool IsEN { get; set; }
        public static LanguagePack EN = new LanguagePack(@"LanguageFile\EN.lng");

        public LanguagePack(string _file)
        {
            file = _file;
            if (Path.GetFileName(file) == "EN.lng") IsEN = true; else IsEN = false;
            strings = new List<LPPair>();
            ReadFile();
            if (!IsEN) Update();
        }

        public string? FindElement(string name)
        {
            foreach(var pair in strings)
            {
                if(pair.name == name && pair.value is not null)
                {
                    return pair.value;
                }
            }
            return null;
        }

        private void ReadFile()
        {
            if (File.Exists(file))
            {
                foreach(var line in File.ReadAllLines(file))
                {
                    if(line != "")
                    {
                        if (line[0] != '#' && line.Contains("="))
                        {
                            string[] pair = line.Split('=');
                            if(pair.Length != 2)
                            {
                                throw new Exception("error in line " + line + ". no or too many separator(=) or comment with no (#)");
                            }
                            else
                            {
                                strings.Add(new LPPair() { name = pair[0], value = pair[1] });
                            }
                        }
                    }
                }
            }
        }

        private void Update()
        {
            List<string> str = new List<string>();
            foreach (var element in LanguagePack.EN.strings)
            {
                bool found = false;
                foreach (var item in strings)
                {
                    if (element.name == item.name)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    str.Add(element.name);
                }
            }
            if (str.Count > 0)
            {
                using (StreamWriter writer = new StreamWriter(file, append: true))
                {
                    writer.WriteLine(Environment.NewLine);
                    writer.WriteLine("#");
                    writer.WriteLine("#Update " + DateTime.Now.ToString("d"));
                    foreach (var element in str)
                    {
                        writer.WriteLine(element + "=");
                    }
                }
            }
        }
    }
}

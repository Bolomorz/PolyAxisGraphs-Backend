using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyAxisGraphs_Backend
{
    /// <summary>
    /// language pack for controls on views/windows.
    /// </summary>
    public class LanguagePack
    {
        /// <summary>
        /// error in specified line while reading .lng file.
        /// </summary>
        public class LanguagePackFileReadError : Exception 
        {
            /// <summary>
            /// display line where error is thrown.
            /// </summary>
            /// <param name="line">faulty line.</param>
            public LanguagePackFileReadError(string line) : base(string.Format("error in line {0}. no or too many separator(=) or comment with no (#)", line)) { }
        }
        
        /// <summary>
        /// language pack tuple of name and value.
        /// </summary>
        public struct LPPair
        {
            /// <summary>
            /// name of variable.
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// value of variable.
            /// </summary>
            public string? value { get; set; }
        }
        /// <summary>
        /// path to currently opened language file.
        /// </summary>
        public string file { get; set; }
        /// <summary>
        /// data of currently opened language file.
        /// </summary>
        public List<LPPair> strings { get; set; }
        /// <summary>
        /// boolean wether opened language is default english language.
        /// </summary>
        public bool IsEN { get; set; }
        /// <summary>
        /// default english language.
        /// </summary>
        public static LanguagePack EN = new LanguagePack(@"LanguageFile\EN.lng");

        /// <summary>
        /// open and read language file.
        /// </summary>
        /// <param name="_file">path to opened file.</param>
        /// <exception cref="LanguagePackFileReadError">thrown if error found in file.</exception>
        public LanguagePack(string _file)
        {
            file = _file;
            if (Path.GetFileName(file) == "EN.lng") IsEN = true; else IsEN = false;
            strings = new List<LPPair>();
            ReadFile();
            if (!IsEN) Update();
        }

        /// <summary>
        /// find value of variable with specified name.
        /// </summary>
        /// <param name="name">specified name of variable.</param>
        /// <returns>value of variable if found. returns null if not found.</returns>
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

        /// <summary>
        /// read opened file from filepath.
        /// </summary>
        /// <exception cref="LanguagePackFileReadError">thrown if error found in file.</exception>
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
                                throw new LanguagePackFileReadError(line);
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

        /// <summary>
        /// update currently opened language file if necessary.
        /// </summary>
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

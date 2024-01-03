using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyAxisGraphs_Backend
{
    /// <summary>
    /// generate data file with random generated values.
    /// </summary>
    public class FileGenerator
    {
        /// <summary>
        /// filetype of data file.
        /// </summary>
        public enum FileType { txt, csv, xlsx }
        Random rd;
        /// <summary>
        /// x-axis data.
        /// </summary>
        public FileGeneratorAxis XAxis { get; set; }
        /// <summary>
        /// list of y-axis data.
        /// </summary>
        public List<FileGeneratorAxis> YAxes { get; set; }
        string file { get; set; }
        Settings settings { get; set; }

        /// <summary>
        /// create file generator with specified y-axis-count.
        /// </summary>
        /// <param name="YAxisCount">y-axis-count.</param>
        /// <param name="_settings">settings of pag.</param>
        public FileGenerator(int YAxisCount, Settings _settings)
        {
            rd = new Random();
            XAxis = new FileGeneratorAxis();
            YAxes = new List<FileGeneratorAxis>();
            for (int i = 0; i < YAxisCount; i++)
            {
                YAxes.Add(new FileGeneratorAxis());
            }
            file = "";
            settings = _settings;
        }

        /// <summary>
        /// create file generator with predefined axis data.
        /// </summary>
        /// <param name="_settings">settings of pag.</param>
        public FileGenerator(Settings _settings)
        {
            rd = new Random();
            XAxis = FileGeneratorAxis.NM;
            YAxes = new List<FileGeneratorAxis>
            {
                FileGeneratorAxis.AMP,
                FileGeneratorAxis.VOL,
                FileGeneratorAxis.RPM,
                FileGeneratorAxis.EFF,
                FileGeneratorAxis.POW
            };
            file = "";
            settings = _settings;
        }

        /// <summary>
        /// generate data and file of specified file type.
        /// </summary>
        /// <param name="type">file type of data file.</param>
        /// <returns>path to file. returns null if no file generated.</returns>
        public string? GenerateFile(FileType type)
        {
            double startx = (double)(rd.Next(0, 100)) / 1000.0;
            XAxis.last = startx;
            XAxis.values.Add(startx);

            foreach (var fa in YAxes)
            {
                double starty = rd.Next(fa.min, (fa.max - (fa.max - fa.min) / 2));
                fa.values.Add(starty);
                fa.last = starty;
            }

            bool cont = true;
            while (cont)
            {
                double nextdouble = (double)rd.Next(int.MaxValue / 100, int.MaxValue) / (double)int.MaxValue;
                double nextx = nextdouble / 10;
                if ((nextx + XAxis.last) < XAxis.max)
                {
                    AddValue(XAxis, nextx);
                    foreach (var fa in YAxes)
                    {
                        double interval = fa.max - fa.min;
                        nextdouble = (double)rd.Next(int.MaxValue / 100, int.MaxValue) / (double)int.MaxValue;
                        double nexty = nextdouble * interval / 100;
                        AddValue(fa, nexty);
                    }
                }
                else
                {
                    cont = false;
                }
            }
            string? f = null;
            switch (type)
            {
                case FileType.txt:
                    f = FindFileName(type);
                    if (f is not null)
                    {
                        file = f;
                        SaveFileTxt();
                    }
                    break;
                case FileType.xlsx:
                    f = ExcelReaderWriter.FindNextFileName(settings);
                    if (f is not null)
                    {
                        file = f;
                        SaveFileXlsx();
                    }
                    break;
                case FileType.csv:
                    f = FindFileName(type);
                    if (f is not null)
                    {
                        file = f;
                        SaveFileCsv();
                    }
                    break;
            }

            return file;
        }

        /// <summary>
        /// add value to specified axis.
        /// </summary>
        /// <param name="axis">specified axis.</param>
        /// <param name="value">added value.</param>
        private void AddValue(FileGeneratorAxis axis, double value)
        {
            double next;
            if (axis.direction)
            {
                next = axis.last + value;
                if(next > axis.max)
                {
                    next = next - value;
                    axis.direction = false;
                }
            }
            else
            {
                next = axis.last - value;
                if (next < axis.min)
                {
                    next = next + value;
                    axis.direction = true;
                }
            }
            next = Math.Round(next, 5);
            axis.values.Add(next);
            axis.last = next;
        }

        /// <summary>
        /// find unused filename of type in settings.initialdirectory.
        /// </summary>
        /// <param name="type">file type of file.</param>
        /// <returns>path to file. returns null if settings.initialdirectory is null.</returns>
        private string? FindFileName(FileType type)
        {
            int i = 0;
            while (settings.initialdirectory is not null)
            {
                string? path = null;
                if (type == FileType.txt) path = settings.initialdirectory + "TestFile" + i + ".txt";
                else if(type == FileType.csv) path = settings.initialdirectory + "TestFile" + i + ".csv";
                if(path is not null)
                {
                    if (!File.Exists(path))
                    {
                        return path;
                    }
                }
                i++;
            }
            return null;
        }

        /// <summary>
        /// save file as .txt.
        /// </summary>
        private void SaveFileTxt()
        {
            string firstline = "";
            firstline += XAxis.name;
            foreach(var fx in YAxes)
            {
                firstline += " " + fx.name;
            }
            File.WriteAllText(file, firstline);

            int count = 0;
            foreach (var x in XAxis.values)
            {
                string line = Environment.NewLine;
                line += x.ToString();
                foreach(var fx in YAxes)
                {
                    line += " " + fx.values[count].ToString();
                }
                File.AppendAllText(file, line);
                count++;
            }
        }

        /// <summary>
        /// save file as .csv.
        /// </summary>
        private void SaveFileCsv()
        {
            string firstline = "";
            firstline += XAxis.name;
            foreach (var fx in YAxes)
            {
                firstline += ";" + fx.name;
            }
            File.WriteAllText(file, firstline);

            int count = 0;
            foreach (var x in XAxis.values)
            {
                string line = Environment.NewLine;
                line += x.ToString();
                foreach (var fx in YAxes)
                {
                    line += ";" + fx.values[count].ToString();
                }
                File.AppendAllText(file, line);
                count++;
            }
        }

        /// <summary>
        /// save file as .xlsx.
        /// </summary>
        private void SaveFileXlsx()
        {
            ExcelReaderWriter erw = new ExcelReaderWriter(file, settings);
            erw.EstablishConnection();
            Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Turquoise, Color.Purple, Color.Yellow, Color.Black };
            erw.WriteCell(1, 2, "XValues");
            erw.WriteCell(2, 2, XAxis.name);
            int col = 3;
            foreach (var fx in YAxes)
            {
                if ((col - 3) < colors.Length)
                {
                    erw.WriteCell(1, col, "YValues");
                    erw.WriteCell(2, col, fx.name);
                    erw.SetColor(2, col, colors[col - 3]);
                }
                else
                {
                    erw.WriteCell(1, col, "YValues");
                    erw.WriteCell(2, col, fx.name);
                    erw.SetColor(2, col, Color.Gray);
                }
                col++;
            }

            int row = 3;
            int count = 0;
            foreach (double x in XAxis.values)
            {
                erw.WriteCell(row, 1, count);
                erw.WriteCell(row, 2, x);

                int column = 3;
                foreach (var fx in YAxes)
                {
                    erw.WriteCell(row, column, fx.values[count]);
                    column++;
                }

                row++;
                count++;
            }
            erw.Disconnect();
        }
    }

    /// <summary>
    /// axis with data for file generator.
    /// </summary>
    public class FileGeneratorAxis
    {
        /// <summary>
        /// data of axis.
        /// </summary>
        public List<double> values { get; set; }
        /// <summary>
        /// min value.
        /// </summary>
        public int min { get; set; }
        /// <summary>
        /// max value.
        /// </summary>
        public int max { get; set; }
        /// <summary>
        /// value of last added data.
        /// </summary>
        public double last { get; set; }
        /// <summary>
        /// name of axis.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// direction of next data. true -> add, false -> subtract.
        /// </summary>
        public bool direction { get; set; }

        /// <summary>
        /// predefined axis with name volt, min = 10, max = 15.
        /// </summary>
        public static FileGeneratorAxis VOL = new FileGeneratorAxis(10, 15, "Volt");
        /// <summary>
        /// predefined axis with name ampere, min = 0, max = 10.
        /// </summary>
        public static FileGeneratorAxis AMP = new FileGeneratorAxis(0, 10, "Ampere");
        /// <summary>
        /// predefined axis with name efficiency, min = 0, max = 1.
        /// </summary>
        public static FileGeneratorAxis EFF = new FileGeneratorAxis(0, 1, "Efficiency");
        /// <summary>
        /// predefined axis with name rotations per minute, min = 1000, max = 10000.
        /// </summary>
        public static FileGeneratorAxis RPM = new FileGeneratorAxis(1000, 10000, "RotationsPerMinute");
        /// <summary>
        /// predefined axis with name newton meter, min = 0, max = 10.
        /// </summary>
        public static FileGeneratorAxis NM = new FileGeneratorAxis(0, 10, "NewtonMeter");
        /// <summary>
        /// predefined axis with name power, min = 0, max = 150.
        /// </summary>
        public static FileGeneratorAxis POW = new FileGeneratorAxis(0, 150, "Power");

        /// <summary>
        /// create axis without names and without min/max values.
        /// </summary>
        public FileGeneratorAxis()
        {
            values = new List<double>();
            name = string.Empty;
            min = 0;
            max = 0;
            last = 0;
            direction = true;
        }

        /// <summary>
        /// create axis with name and min/max values.
        /// </summary>
        /// <param name="_min">min value.</param>
        /// <param name="_max">max value.</param>
        /// <param name="_name">name of axis</param>
        public FileGeneratorAxis(int _min, int _max, string _name)
        {
            values = new List<double>();
            name = _name;
            min = _min;
            max = _max;
            last = 0;
            direction = true;
        }
    }
}

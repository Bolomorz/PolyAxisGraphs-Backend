using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Diagnostics;

namespace PolyAxisGraphs_Backend
{
    public class PolyAxisGraph
    {
        public List<Series> series { get; set; }
        public string xaxisname { get; set; }
        public string charttitle { get; set; }

        public Settings settings { get; set; }

        public string filepath { get; set; }

        public double lastx { get; set; }
        public int x1 { get; set; }
        public int x2 { get; set; }
        public int defx1 { get; set; }
        public int defx2 { get; set; }

        public PolyAxisGraph(Settings _settings) 
        {
            series = new List<Series>();
            settings = _settings;
            filepath = "";
            lastx = 0;
            x1 = 0;
            x2 = 0;
            defx1 = 0;
            defx2 = 0;
            xaxisname = "";
            charttitle = "";
        }

        public static int ReadStringToInt(string val)
        {
            string newval = "";

            foreach(var c in val)
            {
                if(PolyAxisGraph.IsNumeric(c)) newval += c;
            }

            if (newval != "") return Convert.ToInt32(newval); else return 0;
        }

        public static double ReadStringToDouble(string val)
        {
            string newval = "";

            foreach(var c in val)
            {
                if (PolyAxisGraph.IsNumeric(c))
                {
                    newval += c;
                }
                else if(c == ',' || c == '.')
                {
                    newval += System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                }
            }

            if(newval != "")
            {
                return double.Parse(newval, System.Globalization.NumberStyles.AllowDecimalPoint);
            }
            else
            {
                return 0;
            }
        }

        public static bool IsNumeric(char val)
        {
            if (val == '0' || val == '1' || val == '2' || val == '3' || val == '4' || val == '5' || val == '6' || val == '7' || val == '8' || val == '9')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CalculateRegression(Series series, Regression.FunctionType type, int order)
        {
            Regression regression = new Regression(series.XValues, series.YValues, settings);
            series.rft = type;

            switch (type)
            {
                case Regression.FunctionType.Line:
                    series.regressionfunction = regression.LinearRegression();
                    break;
                case Regression.FunctionType.Exponential:
                    series.regressionfunction = regression.ExponentialRegression();
                    break;
                case Regression.FunctionType.Logarithm:
                    series.regressionfunction = regression.LogarithmicRegression();
                    break;
                case Regression.FunctionType.Polynomial:
                    series.regressionfunction = regression.PolynomialRegression(order);
                    if (double.IsNaN(series.regressionfunction[0]))
                    {
                        series.rft = Regression.FunctionType.NaF;
                    }
                    break;
                case Regression.FunctionType.Power:
                    series.regressionfunction = regression.PowerRegression();
                    break;
                case Regression.FunctionType.NaF:
                    series.regressionfunction = new double[1];
                    series.regressionfunction[0] = double.NaN;
                    break;
            }

        }

        public double CalculateValue(double xvalue, double[] function, Regression.FunctionType type)
        {
            switch (type)
            {
                case Regression.FunctionType.Line:
                    return function[0] + function[1] * xvalue;
                case Regression.FunctionType.Exponential:
                    return function[0] * Math.Exp(function[1] * xvalue);
                case Regression.FunctionType.Logarithm:
                    return function[0] + function[1] * Math.Log(xvalue);
                case Regression.FunctionType.Polynomial:
                    double exponent = 0;
                    double y = 0;
                    foreach(var coeff in function)
                    {
                        y += coeff * Math.Pow(xvalue, exponent);
                        exponent++;
                    }
                    return y;
                case Regression.FunctionType.Power:
                    return function[0] * Math.Pow(xvalue, function[1]);
                default:
                    return 0;
            }
        }

        public void SetChartTitle(string title)
        {
            charttitle = title;
        }

        public void SetLanguage(string file)
        {
            settings.currentlang = new LanguagePack(file);
        }

        public void ReadData()
        {
            bool exists = File.Exists(filepath);
            string extension = Path.GetExtension(filepath);
            if(exists && extension == ".txt")
            {
                series.Clear();
                Debug.WriteLine("exists + extension == .txt is true");
                lastx = double.MinValue;

                int count = 0;

                foreach(string line in File.ReadLines(filepath))
                {
                    Debug.WriteLine($"{line}");
                    if(count == 0)
                    {
                        ReadFirstLine(line, ' ');
                    }
                    else
                    {
                        ReadLine(line, ' ');
                    }
                    count++;
                }

                defx1 = (int)Math.Floor(series[0].XValues[0]);
                defx2 = (int)Math.Ceiling(series[0].XValues[series[0].XValues.Count - 1]);
                x1 = defx1;
                x2 = defx2;
            }
            else if(exists && extension == ".xlsx")
            {
                series.Clear();

                lastx = double.MinValue;

                ReadXlsx();

                defx1 = (int)Math.Floor(series[0].XValues[0]);
                defx2 = (int)Math.Ceiling(series[0].XValues[series[0].XValues.Count - 1]);
                x1 = defx1;
                x2 = defx2;
            }
            else if(exists && extension == ".csv")
            {
                series.Clear();

                lastx = double.MinValue;

                int count = 0;

                foreach (string line in File.ReadLines(filepath))
                {
                    if (count == 0)
                    {
                        ReadFirstLine(line, ';');
                    }
                    else
                    {
                        ReadLine(line, ';');
                    }
                    count++;
                }

                defx1 = (int)Math.Floor(series[0].XValues[0]);
                defx2 = (int)Math.Ceiling(series[0].XValues[series[0].XValues.Count - 1]);
                x1 = defx1;
                x2 = defx2;
            }
        }

        public void SetFilePath(string file)
        {
            filepath = file;
        }

        private void ReadFirstLine(string line, char separator)
        {
            char[] separators = { separator };
            string[] axisnames = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            Color[] colors = { Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Turquoise, Color.Purple, Color.Yellow, Color.Black };
            int count = 0;
            foreach(string axisname in axisnames)
            {
                if(count == 0)
                {
                    xaxisname = axisname;
                }
                else
                {
                    int seriesCount = count - 1;
                    if(seriesCount < colors.Length - 1)
                    {
                        series.Add(new Series(axisname, colors[seriesCount], settings));
                    }
                    else
                    {
                        series.Add(new Series(axisname, Color.Gray, settings));
                    }
                }
                count++;
            }
        }

        private void ReadLine(string line, char separator)
        {
            char[] separators = { separator };
            string[] values = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            double xval = PolyAxisGraph.ReadStringToDouble(values[0]);
            if(xval > lastx)
            {
                for(int i = 1; i < values.Length; i++)
                {
                    series[i - 1].Add(xval, ReadStringToDouble(values[i]));
                }
                lastx = xval;
            }
        }

        private void ReadXlsx()
        {
            ExcelReaderWriter reader = new ExcelReaderWriter(filepath, settings);
            reader.EstablishConnection();
            var cell = reader.ReadCell(2, 2);
            if (cell.value is not null)
            {
                xaxisname = cell.value.ToString();
            }
            bool cont = true;
            int col = 3;
            while (cont)
            {
                cell = reader.ReadCell(2, col);
                if (cell.value is not null && cell.color is not null)
                {
                    series.Add(new Series(cell.value, (Color)cell.color, settings));
                }
                else
                {
                    cont = false;
                }
                col++;
            }

            int row = 3;

            while (reader.ReadCell(row, 1).value is not null)
            {
                cell = reader.ReadCell(row, 2);
                double xval = 0;
                if (cell.value is not null)
                {
                    xval = PolyAxisGraph.ReadStringToDouble(cell.value);
                }
                if (xval > lastx)
                {
                    lastx = xval;

                    col = 3;
                    foreach (var ser in series)
                    {
                        cell = reader.ReadCell(row, col);
                        if (cell.value is not null)
                        {
                            ser.Add(xval, PolyAxisGraph.ReadStringToDouble(cell.value));
                        }
                        col++;
                    }
                }
                row++;
            }
            reader.Disconnect();
        }
    }
}

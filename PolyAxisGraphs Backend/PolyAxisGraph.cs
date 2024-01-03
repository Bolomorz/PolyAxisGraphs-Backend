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
    /// <summary>
    /// stored data of series of chart.
    /// </summary>
    public class PolyAxisGraph
    {
        /// <summary>
        /// list of series.
        /// </summary>
        public List<Series> series { get; set; }
        /// <summary>
        /// name of x axis.
        /// </summary>
        public string xaxisname { get; set; }
        /// <summary>
        /// title of chart.
        /// </summary>
        public string charttitle { get; set; }

        /// <summary>
        /// currently opened settings.
        /// </summary>
        public Settings settings { get; set; }

        /// <summary>
        /// currently opened data file.
        /// </summary>
        public string filepath { get; set; }

        /// <summary>
        /// last added x value.
        /// </summary>
        public double lastx { get; set; }
        /// <summary>
        /// minimum of x values.
        /// </summary>
        public int x1 { get; set; }
        /// <summary>
        /// maximum of x values.
        /// </summary>
        public int x2 { get; set; }
        /// <summary>
        /// default minimum of x values.
        /// </summary>
        public int defx1 { get; set; }
        /// <summary>
        /// default maximum of x values.
        /// </summary>
        public int defx2 { get; set; }

        /// <summary>
        /// create pag with settings
        /// </summary>
        /// <param name="_settings">currently opened settings file.</param>
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

        /// <summary>
        /// read string value to integer number.
        /// </summary>
        /// <param name="val">string value.</param>
        /// <returns>integer number.</returns>
        public static int ReadStringToInt(string val)
        {
            string newval = "";

            foreach(var c in val)
            {
                if(PolyAxisGraph.IsNumeric(c)) newval += c;
            }

            if (newval != "") return Convert.ToInt32(newval); else return 0;
        }

        /// <summary>
        /// read string value to double number.
        /// </summary>
        /// <param name="val">string value.</param>
        /// <returns>double number.</returns>
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

        /// <summary>
        /// test if character is numeric.
        /// </summary>
        /// <param name="val">character value.</param>
        /// <returns>true if numeric, false otherwise.</returns>
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

        /// <summary>
        /// calculate regression function of specified series as specified type.
        /// </summary>
        /// <param name="series">specified series.</param>
        /// <param name="type">function type.</param>
        /// <param name="order">order of polynomial function.</param>
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

        /// <summary>
        /// calculate corresponding y value to x value with function and type.
        /// </summary>
        /// <param name="xvalue">x value.</param>
        /// <param name="function">regression function.</param>
        /// <param name="type">function type.</param>
        /// <returns>corresponding y value.</returns>
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

        /// <summary>
        /// set title of chart.
        /// </summary>
        /// <param name="title">title of chart.</param>
        public void SetChartTitle(string title)
        {
            charttitle = title;
        }

        /// <summary>
        /// set current language.
        /// </summary>
        /// <param name="file">language file .lng.</param>
        /// <returns>error if thrown, null otherwise.</returns>
        public string? SetLanguage(string file)
        {
            try
            {
                settings.currentlang = new LanguagePack(file);
                return null;
            }
            catch (Exception ex) 
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// read data from currently opened data file.
        /// </summary>
        public void ReadData()
        {
            bool exists = File.Exists(filepath);
            string extension = Path.GetExtension(filepath);
            if(exists && extension == ".txt")
            {
                series.Clear();
                lastx = double.MinValue;

                int count = 0;

                foreach(string line in File.ReadLines(filepath))
                {
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

        /// <summary>
        /// set currently opened data file.
        /// </summary>
        /// <param name="file">path to file.</param>
        public void SetFilePath(string file)
        {
            filepath = file;
        }

        /// <summary>
        /// read first line in .txt or .csv file. contains names of x axis and y axes.
        /// </summary>
        /// <param name="line">line to read.</param>
        /// <param name="separator">separator (' ', ';')</param>
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

        /// <summary>
        /// read line in .txt or .csv file. contains data of x values and series values.
        /// </summary>
        /// <param name="line">line to read.</param>
        /// <param name="separator">separator (' ', ';')</param>
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

        /// <summary>
        /// read .xlsx file. contains names and data of x axis and y axes.
        /// </summary>
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

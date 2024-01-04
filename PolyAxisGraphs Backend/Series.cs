using System.ComponentModel;
using System.Drawing;

namespace PolyAxisGraphs_Backend
{
    /// <summary>
    /// stored data of series as x values and y values.
    /// </summary>
    public class Series
    {
        /// <summary>
        /// string of regression function.
        /// </summary>
        public struct FunctionString
        {
            /// <summary>
            /// text of function.
            /// </summary>
            public string function { get; set; }
            /// <summary>
            /// specify wether text is supposed to be written in superscript.
            /// </summary>
            public bool superscript { get; set; } 
            /// <summary>
            /// color of series
            /// </summary>
            public Color fcolor { get; set; }
        }
        /// <summary>
        /// x values of series.
        /// </summary>
        public List<double> XValues { get; set; }
        /// <summary>
        /// y values of series.
        /// </summary>
        public List<double> YValues { get; set; }

        /// <summary>
        /// minimum of y values.
        /// </summary>
        public int min { get; set; }
        /// <summary>
        /// maximum of y values.
        /// </summary>
        public int max { get; set; }

        /// <summary>
        /// currently set minimum of y values.
        /// </summary>
        public double setmin { get; set; }
        /// <summary>
        /// currently set maximum of y values.
        /// </summary>
        public double setmax { get; set; }
        /// <summary>
        /// current interval of y values according to grid divions count.
        /// </summary>
        public double interval { get; set; }

        /// <summary>
        /// name of series.
        /// </summary>
        public string name { get; set; }  
        /// <summary>
        /// color of series.
        /// </summary>
        public System.Drawing.Color color { get; set; }

        /// <summary>
        /// specify wether series is to be drawn on canvas.
        /// </summary>
        public bool active { get; set; }
        /// <summary>
        /// regression function of current data.
        /// </summary>
        public double[] regressionfunction { get; set; }
        /// <summary>
        /// type of current regression function.
        /// </summary>
        public Regression.FunctionType rft { get; set; }
        /// <summary>
        /// specify wether function is to be drawn on canvas.
        /// </summary>
        public bool showfunction { get; set; }
        /// <summary>
        /// precision of displayed function.
        /// </summary>
        public int precision { get; set; }

        /// <summary>
        /// currently opened settings file.
        /// </summary>
        private Settings settings { get; set; }

        /// <summary>
        /// create series with specified name and color.
        /// </summary>
        /// <param name="_name">name of series.</param>
        /// <param name="_color">color of series.</param>
        /// <param name="_settings">currently opened settings file.</param>
        public Series(string _name, System.Drawing.Color _color, Settings _settings)
        {
            name = _name;
            color = _color;

            XValues = new List<double>();
            YValues = new List<double>();

            min = int.MaxValue;
            max = int.MinValue;

            active = true;
            showfunction = false;

            regressionfunction = new double[1];
            regressionfunction[0] = double.NaN;
            rft = Regression.FunctionType.NaF;
            precision = 5;

            settings = _settings;
        }

        /// <summary>
        /// add pair of x and y value to series data.
        /// </summary>
        /// <param name="x">x value.</param>
        /// <param name="y">y value.</param>
        public void Add(double x, double y)
        {
            XValues.Add(x);
            YValues.Add(y);
            CompareMax(y);
            CompareMin(y);
            setmin = min;
            setmax = max;
            if (settings.chartgridinterval is not null)
            {
                interval = (double)(setmax - setmin) / (double)settings.chartgridinterval;
            }
            else
            {
                interval = (double)(setmax - setmin) / 20.0;
            }
        }

        /// <summary>
        /// set maximum y value.
        /// </summary>
        /// <param name="_max">maximum y value.</param>
        public void SetMax(double _max)
        {
            if(_max > setmin)
            {
                setmax = _max;
                if (settings.chartgridinterval is not null)
                {
                    interval = (double)(setmax - setmin) / (double)settings.chartgridinterval;
                }
                else
                {
                    interval = (double)(setmax - setmin) / 20.0;
                }
            }
        }

        /// <summary>
        /// set minimum y value.
        /// </summary>
        /// <param name="_min">minimum y value.</param>
        public void SetMin(double _min)
        {
            if(_min < setmax)
            {
                setmin = _min;
                if (settings.chartgridinterval is not null)
                {
                    interval = (double)(setmax - setmin) / (double)settings.chartgridinterval;
                }
                else
                {
                    interval = (double)(setmax - setmin) / 20.0;
                }
            }
        }

        /// <summary>
        /// reset maximum y value to default value.
        /// </summary>
        public void ResetMax()
        {
            setmax = max;
            if (settings.chartgridinterval is not null)
            {
                interval = (double)(setmax - setmin) / (double)settings.chartgridinterval;
            }
            else
            {
                interval = (double)(setmax - setmin) / 20.0;
            }
        }

        /// <summary>
        /// reset minimum y value to default value.
        /// </summary>
        public void ResetMin()
        {
            setmin = min;
            if (settings.chartgridinterval is not null)
            {
                interval = (double)(setmax - setmin) / (double)settings.chartgridinterval;
            }
            else
            {
                interval = (double)(setmax - setmin) / 20.0;
            }
        }

        /// <summary>
        /// get currently calculated function as list of function strings.
        /// </summary>
        /// <returns>list of function strings.</returns>
        public List<FunctionString> GetFunction()
        {
            List<FunctionString> functions = new List<FunctionString>();
            string str = string.Empty;

            switch (rft)
            {
                case Regression.FunctionType.Line:
                    double l1 = Math.Round(regressionfunction[0], precision);
                    double l2 = Math.Round(regressionfunction[1], precision);
                    string lo = "+";
                    if(l2 < 0)
                    {
                        lo = "-";
                        l2 *= -1;
                    }
                    str = String.Format("y = {0} {2} {1} * x", l1, l2, lo);
                    functions.Add(new FunctionString() { function = str, superscript = false, fcolor = color});
                    break;
                case Regression.FunctionType.Exponential:
                    double e1 = Math.Round(regressionfunction[0], precision);
                    double e2 = Math.Round(regressionfunction[1], precision);
                    str = String.Format("y = {0} * exp({1} * x)", e1, e2);
                    functions.Add(new FunctionString() { function = str, superscript = false, fcolor = color });
                    break;
                case Regression.FunctionType.Logarithm:
                    double log1 = Math.Round(regressionfunction[0], precision);
                    double log2 = Math.Round(regressionfunction[1], precision);
                    string logo = "+";
                    if (log2 < 0)
                    {
                        logo = "-";
                        log2 *= -1;
                    }
                    str = String.Format("y = {0} {2} {1} * ln(x)", log1, log2, logo);
                    functions.Add(new FunctionString() { function = str, superscript = false, fcolor = color });
                    break;
                case Regression.FunctionType.Polynomial:
                    str = String.Format("y = {0}", Math.Round(regressionfunction[0], precision));
                    functions.Add(new FunctionString() { function = str, superscript = false, fcolor = color });
                    for(int i = 1; i < regressionfunction.Length; i++)
                    {
                        double p1 = Math.Round(regressionfunction[i], precision);
                        string po = " + ";
                        if(p1 < 0)
                        {
                            po = " - ";
                            p1 *= -1;
                        }
                        str = po + p1 + " * x";
                        functions.Add(new FunctionString() { function = str, superscript = false, fcolor = color });
                        str = i.ToString();
                        functions.Add(new FunctionString() { function = str, superscript = true, fcolor = color }); 
                    }
                    break;
                case Regression.FunctionType.Power:
                    double pow1 = Math.Pow(regressionfunction[0], precision);
                    double pow2 = Math.Pow(regressionfunction[1], precision);
                    str = String.Format("y = {0} * x", pow1);
                    functions.Add(new FunctionString() { function = str, superscript = false, fcolor = color });
                    str = pow2.ToString();
                    functions.Add(new FunctionString() { function = str, superscript = true, fcolor = color });
                    break;
                case Regression.FunctionType.NaF:
                    str = string.Empty;
                    functions.Add(new FunctionString() { function = str, superscript = true, fcolor = color }); 
                    break;
            }
            return functions;
        }

        /// <summary>
        /// compare new added value to current max value.
        /// </summary>
        /// <param name="value">new added value.</param>
        private void CompareMax(double value)
        {
            if (value > max)
            {
                int val = 1;

                while (value > val)
                {
                    val = val * 10;
                }
                if (val == 1)
                {
                    val = 1;
                }
                else
                {
                    val = val / 10;
                }
                max = 0;
                while (value > max)
                {
                    max = max + val;
                }
            }
        }

        /// <summary>
        /// compare new added value to current min value.
        /// </summary>
        /// <param name="value">new added value.</param>
        private void CompareMin(double value)
        {
            if (value < min)
            {
                int val;
                if (value < 0)
                {
                    val = -1;
                    while (value < val)
                    {
                        val = val * 10;
                    }
                    if (val == -1)
                    {
                        val = -1;
                    }
                    else
                    {
                        val = val / 10;
                    }
                    min = 0;
                    while (value < min)
                    {
                        min = min + val;
                    }
                }
                else
                {
                    val = 1;
                    while (value > val)
                    {
                        val = val * 10;
                    }
                    if (val == 1)
                    {
                        val = 1;
                    }
                    else
                    {
                        val = val / 10;
                    }
                    min = 0;
                    while (value > min + val)
                    {
                        min = min + val;
                    }
                }
            }

            if (min < 0)
            {
                if ((-1) * min < max / 10)
                {
                    min = (-1) * (max / 10);
                }
            }
            else
            {
                if (min < max / 10)
                {
                    min = 0;
                }
            }
        }
    }
}
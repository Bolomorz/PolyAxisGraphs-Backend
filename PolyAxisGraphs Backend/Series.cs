using System.ComponentModel;

namespace PolyAxisGraphs_Backend
{
    public class Series
    {
        public struct FunctionString
        {
            public string function { get; set; }
            public bool superscript { get; set; } 
        }
        public List<double> XValues { get; set; }
        public List<double> YValues { get; set; }

        public int min { get; set; }
        public int max { get; set; }

        public double setmin { get; set; }
        public double setmax { get; set; }
        public double interval { get; set; }

        public string name { get; set; }  
        public System.Drawing.Color color { get; set; }

        public bool active { get; set; }
        public double[] regressionfunction { get; set; }
        public Regression.FunctionType rft { get; set; }
        public bool showfunction { get; set; }
        public int precision { get; set; }

        private Settings settings { get; set; }

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
                    functions.Add(new FunctionString() { function = str, superscript = false});
                    break;
                case Regression.FunctionType.Exponential:
                    double e1 = Math.Round(regressionfunction[0], precision);
                    double e2 = Math.Round(regressionfunction[1], precision);
                    str = String.Format("y = {0} * exp({1} * x)", e1, e2);
                    functions.Add(new FunctionString() { function = str, superscript = false});
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
                    functions.Add(new FunctionString() { function = str, superscript = false});
                    break;
                case Regression.FunctionType.Polynomial:
                    str = String.Format("y = {0}", Math.Round(regressionfunction[0], precision));
                    functions.Add(new FunctionString() { function = str, superscript = false});
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
                        functions.Add(new FunctionString() { function = str, superscript = false});
                        str = i.ToString();
                        functions.Add(new FunctionString() { function = str, superscript = true}); 
                    }
                    break;
                case Regression.FunctionType.Power:
                    double pow1 = Math.Pow(regressionfunction[0], precision);
                    double pow2 = Math.Pow(regressionfunction[1], precision);
                    str = String.Format("y = {0} * x", pow1);
                    functions.Add(new FunctionString() { function = str, superscript = false});
                    str = pow2.ToString();
                    functions.Add(new FunctionString() { function = str, superscript = true});
                    break;
                case Regression.FunctionType.NaF:
                    str = string.Empty;
                    functions.Add(new FunctionString() { function = str, superscript = true}); 
                    break;
            }
            return functions;
        }

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
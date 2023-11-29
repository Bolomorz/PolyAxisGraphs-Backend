using MathNet.Numerics;
using MathNet.Numerics.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyAxisGraphs_Backend
{
    public class Regression
    {
        public enum FunctionType { NaF, Line, Polynomial, Logarithm, Power, Exponential}
        
        protected List<double> XValues { get; set; }
        protected List<double> YValues { get; set; }
        protected Settings settings { get; set; }

        public Regression(List<double> _xvalues, List<double> _yvalues, Settings _settings)
        {
            XValues = _xvalues;
            YValues = _yvalues;
            settings = _settings;
        }

        public double[] PolynomialRegression(int order)
        {
            double[] x = XValues.ToArray();
            double[] y = YValues.ToArray();

            double[] result;

            if(x.Length > order + 1) 
            {
                result = Fit.Polynomial(x, y, order);
            }
            else
            {
                result = new double[1];
                result[0] = double.NaN;
            }
            return result;
        }

        public double[] LinearRegression()
        {
            double[] x = XValues.ToArray();
            double[] y = YValues.ToArray();

            var values = Fit.Line(x, y);

            double[] result = new double[2];
            result[0] = values.A; 
            result[1] = values.B;

            return result;
        }

        public double[] LogarithmicRegression()
        {
            double[] x = XValues.ToArray();
            double[] y = YValues.ToArray();

            var values = Fit.Logarithm(x, y);

            double[] result = new double[2];
            result[0] = values.A;
            result[1] = values.B;

            return result;
        }

        public double[] PowerRegression() 
        {
            double[] x = XValues.ToArray();
            double[] y = YValues.ToArray();

            var values = Fit.Power(x, y);

            double[] result = new double[2];
            result[0] = values.A;
            result[1] = values.B;

            return result;
        }

        public double[] ExponentialRegression()
        {
            double[] x = XValues.ToArray();
            double[] y = YValues.ToArray();

            var values = Fit.Exponential(x, y);

            double[] result = new double[2];
            result[0] = values.A;
            result[1] = values.R;

            return result;
        }
    }
}

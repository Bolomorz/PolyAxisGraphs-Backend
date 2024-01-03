using MathNet.Numerics;
using MathNet.Numerics.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyAxisGraphs_Backend
{
    /// <summary>
    /// calculate corresponding regression function to series x/y values.
    /// </summary>
    public class Regression
    {
        /// <summary>
        /// type of regression function.
        /// </summary>
        public enum FunctionType { NaF, Line, Polynomial, Logarithm, Power, Exponential}
        
        /// <summary>
        /// list of x values.
        /// </summary>
        protected List<double> XValues { get; set; }
        /// <summary>
        /// list of y values.
        /// </summary>
        protected List<double> YValues { get; set; }
        /// <summary>
        /// currently opened settings file.
        /// </summary>
        protected Settings settings { get; set; }

        /// <summary>
        /// regression function of series x and y values.
        /// </summary>
        /// <param name="_xvalues">x values of series.</param>
        /// <param name="_yvalues">y values of series.</param>
        /// <param name="_settings">currently opened settings file.</param>
        public Regression(List<double> _xvalues, List<double> _yvalues, Settings _settings)
        {
            XValues = _xvalues;
            YValues = _yvalues;
            settings = _settings;
        }

        /// <summary>
        /// calculate polynomial regression.
        /// </summary>
        /// <param name="order">order of polynom.</param>
        /// <returns>polynomial function.</returns>
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

        /// <summary>
        /// calculate linear regression.
        /// </summary>
        /// <returns>linear function.</returns>
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

        /// <summary>
        /// calculate logarithmic regression.
        /// </summary>
        /// <returns>logarithmic function.</returns>
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

        /// <summary>
        /// calculate power regression.
        /// </summary>
        /// <returns>power function.</returns>
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

        /// <summary>
        /// calculate exponential regression.
        /// </summary>
        /// <returns>exponential function.</returns>
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

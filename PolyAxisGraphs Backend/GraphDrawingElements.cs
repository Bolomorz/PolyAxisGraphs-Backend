using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyAxisGraphs_Backend
{
    public class GraphDrawingElements
    {
        public struct Line
        {
            public Point start, end;
            public System.Drawing.Color color;
            public double thickness;
        }

        public struct Text
        {
            public double left, top, right, bottom;
            public string text;
        }

        public struct Rectangle
        {
            public double left, top, right, bottom;
            public double width, height;
        }

        public struct Point
        {
            public double x, y;
        }

        public struct ChartData
        {
            public string? err;
            public Rectangle? titlearea, datearea, chartarea, legendarea, yaxisarea;
            public List<Line>? lines;
            public List<Text>? texts;
        }

        protected double canvaswidth { get; set; }
        protected double canvasheight { get; set; }
        protected PolyAxisGraph pag { get; set; }
        protected Settings settings { get; set; }
        protected int seriescount { get; set; }

        protected Rectangle _titlearea { get; set; }
        protected Rectangle _datearea { get; set; }
        protected Rectangle _chartarea { get; set; }
        protected Rectangle _legendarea { get; set; }
        protected Rectangle _yaxisarea { get; set; }

        protected List<Line> _lines {  get; set; }
        protected List<Text> _texts { get; set; }
        
        public GraphDrawingElements(double _canvaswidth, double _canvasheight, PolyAxisGraph _pag, Settings _settings) 
        {
            canvaswidth = _canvaswidth;
            canvasheight = _canvasheight;
            pag = _pag;
            settings = _settings;
            seriescount = pag.series.Count;
            _lines = new List<Line>();
            _texts = new List<Text>();
            _titlearea = new Rectangle();
            _datearea = new Rectangle();
            _legendarea = new Rectangle();
            _yaxisarea = new Rectangle();
            _chartarea = new Rectangle();
        }

        public ChartData CalculateChart()
        {
            var _err = CalculateChartAreas();
            if (_err is not null) return new ChartData() { err = _err};

            return new ChartData()
            {
                err = null,
                titlearea = _titlearea,
                datearea = _datearea,
                legendarea = _legendarea,
                yaxisarea = _yaxisarea,
                chartarea = _chartarea,
                lines = _lines,
                texts = _texts,
            };
        }

        private string? CalculateChartAreas()
        {
            /*
             * x = width, y = height
             * d = calc dynamically according to seriescount and yaxiswidth
             * 
             * legendarea   titlearea   datearea
             * yaxisarea    chartarea   chartarea
             * 
             * legendarea   (x: 1% - d      | y: 1% - 20%)
             * titlearea    (x: d - 90%     | y: 1% - 20%)
             * datearea     (x: 90% - 99%   | y: 1% - 20%)
             * yaxisarea    (x: 1% - d      | y: 21% - 95%)
             * chartarea    (x: d - 95%     | y: 21% - 95%)
             */

            double d = (settings.yaxiswidth is null) ? seriescount * 20 : seriescount * (double)settings.yaxiswidth;
            if (d > 0.5 * canvaswidth) return "canvas area to small to display graph";
            d = d / canvaswidth;

            CalculateRectangle(_legendarea, 0.01, d, 0.01, 0.2);
            CalculateRectangle(_titlearea, d + 0.05, 0.9, 0.01, 0.2);
            CalculateRectangle(_datearea, 0.91, 0.99, 0.01, 0.2);
            CalculateRectangle(_yaxisarea, 0.01, d, 0.21, 0.95);
            CalculateRectangle(_chartarea, d + 0.05, 0.95, 0.21, 0.95);

            return null;
        }

        private void CalculateRectangle(Rectangle rect, double x1, double x2, double y1, double y2)
        {
            rect.left = canvaswidth * x1;
            rect.top = canvasheight * y1;
            rect.right = canvaswidth * x2;
            rect.bottom = canvasheight * y2;
            rect.width = rect.right - rect.left;
            rect.height = rect.bottom - rect.top;
        }


    }
}

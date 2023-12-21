using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            public double fontsize;
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

        public struct Seriespoint
        {
            public Series series;
            public Point point;
        }

        public struct SeriesData
        {
            public Series series;
            public List<Point> seriespoint;
            public List<Point> chartpoint;
        }

        public struct ChartData
        {
            public string? err;
            public Rectangle? titlearea, datearea, chartarea, legendarea, yaxisarea, functionarea;
            public List<Line>? lines;
            public List<Text>? texts;
            public List<List<Series.FunctionString>>? functions;
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
        protected Rectangle _functionarea { get; set; }

        protected List<Line> _lines {  get; set; }
        protected List<Text> _texts { get; set; }
        protected List<List<Series.FunctionString>> _functions { get; set; }
        protected List<SeriesData> _seriesdata { get; set; }

        public GraphDrawingElements(double _canvaswidth, double _canvasheight, PolyAxisGraph _pag, Settings _settings) 
        {
            canvaswidth = _canvaswidth;
            canvasheight = _canvasheight;
            pag = _pag;
            settings = _settings;
            seriescount = 0;
            foreach(var series in pag.series)
            {
                if(series.active) seriescount++;
            }
            _lines = new List<Line>();
            _texts = new List<Text>();
            _functions = new List<List<Series.FunctionString>>();
            _seriesdata = new List<SeriesData>();
            _titlearea = new Rectangle();
            _datearea = new Rectangle();
            _legendarea = new Rectangle();
            _yaxisarea = new Rectangle();
            _chartarea = new Rectangle();
            _functionarea = new Rectangle();
        }

        public Seriespoint? TranslateChartPointToSeriesPoint(Point point)
        {
            if(point.x < _chartarea.left || point.x > _chartarea.right || point.y < _chartarea.top || point.y > _chartarea.bottom) return null;

            foreach (var _series in _seriesdata)
            {
                if (_series.series.active)
                {
                    for (int i = 0; i < _series.seriespoint.Count; i++)
                    {
                        var seriespoint = _series.seriespoint[i];
                        var chartpoint = _series.chartpoint[i];
                        if (chartpoint.x == point.x && chartpoint.y == point.y) return new Seriespoint()
                        {
                            series = _series.series,
                            point = seriespoint,
                        };
                    }
                }
            }

            return null;
        }

        public void ChangeTitle(string title, double fontsize)
        {
            if(_texts.Count > 0)
            {
                var text = _texts[0];
                text.text = title;
                text.fontsize = fontsize;
            }
        }

        public ChartData CalculateChart()
        {
            _lines.Clear();
            _functions.Clear();
            _texts.Clear();
            _seriesdata.Clear();

            Debug.WriteLine("Calculate Chartareas...");
            
            var _err = CalculateChartAreas();
            if (_err is not null) return new ChartData() { err = _err};
            Debug.WriteLine(_err);

            Debug.WriteLine("Add Title and Date...");
            double fontsize = (settings.chartfontsize is null) ? 8 : (double)settings.chartfontsize;
            double titlefontsize = (settings.charttitlefontsize is null) ? 15 : (double)settings.charttitlefontsize;
            AddTitle(titlefontsize, pag.charttitle);
            AddDate(fontsize);

            Debug.WriteLine("Add Chart...");
            _err = (settings.chartgridinterval is null) ? AddChart(20, pag.x1, pag.x2, fontsize) : AddChart((int)settings.chartgridinterval, pag.x1, pag.x2, fontsize);
            if (_err is not null) return new ChartData() { err = _err };
            Debug.WriteLine(_err);

            Debug.WriteLine("Add Series...");
            double xarea = _yaxisarea.left;
            double xareaintervall = (settings.yaxiswidth is null) ? 20 : (double)settings.yaxiswidth;
            foreach(var series in pag.series)
            {
                if(series.active)
                {
                    Debug.WriteLine("Add active series...");
                    _err = (settings.chartgridinterval is null) ? AddSeries(series, 20, pag.x1, pag.x2, fontsize, xarea) : AddSeries(series, (int)settings.chartgridinterval, pag.x1, pag.x2, fontsize, xarea);
                    if (_err is not null) return new ChartData() { err = _err };
                    Debug.WriteLine(_err);
                    xarea += xareaintervall;
                }
            }

            return new ChartData()
            {
                err = null,
                titlearea = _titlearea,
                datearea = _datearea,
                legendarea = _legendarea,
                yaxisarea = _yaxisarea,
                chartarea = _chartarea,
                functionarea = _functionarea,
                lines = _lines,
                texts = _texts,
                functions = _functions,
            };
        }

        private string? AddSeries(Series series, int gridintervallcount, int x1, int x2, double fontsize, double xarea)
        {
            double yintervall = _yaxisarea.height / gridintervallcount;
            double numintervall = (series.setmax - series.setmin) / gridintervallcount;
            if(yintervall < fontsize) return String.Format("gridintervall too large to display graph. choose smaller chartgridintervall.\nAddSeries(): yintervall {0}, fontsize {1}",yintervall, fontsize);

            Debug.WriteLine("<<< add y axis");
            //Add Y Axis
            Point start = new Point() { x = xarea, y = _yaxisarea.bottom };
            Point end = new Point() { x = xarea, y = _yaxisarea.top };
            AddLine(start, end, series.color, 1);

            Debug.WriteLine("<<< add y axis grid + text");
            //Add Y Axis Grid + Text
            double length = (settings.yaxiswidth is null) ? 5 : (double)settings.yaxiswidth/4;
            start = new Point() { x = xarea, y = _yaxisarea.bottom };
            end = new Point() { x = xarea + length, y = _yaxisarea.bottom };
            double text = Math.Round(series.setmin, 2);
            while (start.y >= _yaxisarea.top)
            {
                AddLine(start, end, series.color, 0.5);
                AddText(start.x, start.x + length*4, start.y - fontsize, start.y, text.ToString(), fontsize);
                start.y -= yintervall;
                end.y -= yintervall;
                text += numintervall;
                text = Math.Round(text, 2);
            }

            Debug.WriteLine("<<< add legend");
            //Add Legend
            start = new Point() { x = xarea, y = _legendarea.bottom };
            end = new Point() { x = xarea, y = _legendarea.top };
            AddLine(start, end, series.color, 1);
            double midpoint = (_legendarea.bottom + _legendarea.top) / 2;
            AddText(xarea, start.x + length * 4, midpoint - fontsize / 2, midpoint + fontsize / 2, series.name, fontsize);

            Debug.WriteLine("<<< add functionstring");
            //Add Functionstring
            var function = series.GetFunction();
            _functions.Add(function);

            Debug.WriteLine("<<< draw series");
            //Draw Series
            List<Point> seriespoints = new List<Point>();
            for(int i = 0; i < series.XValues.Count; i++)
            {
                double xval = series.XValues[i];
                double yval = series.YValues[i];
                if(xval >= x1 && xval <= x2 && yval >= series.setmin && yval <= series.setmax) seriespoints.Add(new Point() { x = xval, y = yval });
            }
            if(seriespoints.Count > 0)
            {
                Point? seriesstart = null;
                int i = 0;
                SeriesData sd = new SeriesData() { series = series, chartpoint = new List<Point>(), seriespoint = new List<Point>() };
                while(seriesstart is null && i < seriespoints.Count)
                {
                    seriesstart = TranslateSeriesPointToChartPoint(seriespoints[i].x, seriespoints[i].y, x1, x2, series.setmin, series.setmax);
                    i++;
                }
                if (seriesstart is not null)
                {
                    sd.seriespoint.Add(seriespoints[i - 1]);
                    sd.chartpoint.Add((Point)seriesstart);
                    while (i < seriespoints.Count)
                    {
                        var seriesend = TranslateSeriesPointToChartPoint(seriespoints[i].x, seriespoints[i].y, x1, x2, series.setmin, series.setmax);
                        if (seriesend is not null)
                        {
                            AddLine((Point)seriesstart, (Point)seriesend, series.color, 1);
                            seriesstart = seriesend;
                            sd.seriespoint.Add(seriespoints[i]);
                            sd.chartpoint.Add((Point)seriesstart);
                        }
                        i++;
                    }
                    _seriesdata.Add(sd);
                }
            }

            Debug.WriteLine("<<< draw function");
            //Draw Regressionfunction
            if (series.showfunction && series.rft != Regression.FunctionType.NaF)
            {
                List<Point> functionpoints = new List<Point>();
                double xintervall = (x2 - x1) / 100;
                double xval = x1;
                while (xval <= x2)
                {
                    double yval = pag.CalculateValue(xval, series.regressionfunction, series.rft);
                    if (xval >= x1 && xval <= x2 && yval >= series.setmin && yval <= series.setmax) functionpoints.Add(new Point() { x = xval, y = yval });
                    xval += xintervall;
                }
                if(functionpoints.Count > 0)
                {
                    Point? functionstart = null;
                    int i = 0;
                    while(functionstart is null && i < functionpoints.Count)
                    {
                        functionstart = TranslateSeriesPointToChartPoint(functionpoints[i].x, functionpoints[i].y, x1, x2, series.setmin, series.setmax);
                        i++;
                    }
                    if (functionstart is not null)
                    {
                        bool draw = true;
                        while (i < functionpoints.Count)
                        {
                            var functionend = TranslateSeriesPointToChartPoint(functionpoints[i].x, functionpoints[i].y, x1, x2, series.setmin, series.setmax);
                            if (functionend is not null && draw)
                            {
                                AddLine((Point)functionstart, (Point)functionend, series.color, 0.5);
                                functionstart = functionend;
                                draw = false;
                            }
                            else if (!draw)
                            {
                                draw = true;
                            }
                            i++;
                        }
                    }
                }
            }

            return null;
        }

        private Point? TranslateSeriesPointToChartPoint(double x, double y, double x1, double x2, double y1, double y2)
        {
            if(x < x1 || x > x2 || y < y1  || y > y2) return null;

            double xpercent = (x - x1) / (x2 - x1);
            double chartoffsetx = (_chartarea.right - _chartarea.left) * xpercent;
            double chartx = _chartarea.left + chartoffsetx;

            double ypercent = (y - y1) / (y2 - y1);
            double chartoffsety = (_chartarea.bottom - _chartarea.top) * ypercent;
            double charty = _chartarea.bottom - chartoffsety;

            return new Point() { x = chartx, y = charty };
        }

        private string? AddChart(int gridintervallcount, int x1, int x2, double fontsize)
        {
            double xintervall = _chartarea.width / (double)gridintervallcount;
            double yintervall = _chartarea.height / (double)gridintervallcount;
            double numintervall = (double)(x2 - x1) / (double)gridintervallcount;
            if(xintervall < 1 ||  yintervall < fontsize) String.Format("gridintervall too large to display graph. choose smaller chartgridintervall.\nAddChart(): yintervall {0}, fontsize {1}, xintervall {2}", yintervall, fontsize, xintervall);
            
            //Add Y Axis
            Point start = new Point() { x = _chartarea.left, y = _chartarea.top };
            Point end = new Point() { x = _chartarea.left, y = _chartarea.bottom };
            double text = x1;
            AddLine(start, end, Color.Black, 1);
            if (fontsize > (canvasheight - 1) - (end.y + 1)) fontsize = (canvasheight - 1) - (end.y + 1) - 1;
            AddText(start.x, start.x + xintervall / 2, end.y + 1, canvasheight - 1, text.ToString(), fontsize);
            while(start.x <= _chartarea.right)
            {
                start.x += xintervall;
                end.x += xintervall;
                text += numintervall;
                AddLine(start, end, Color.Gray, 0.5);
                AddText(start.x, start.x + xintervall/2, end.y + 1, canvasheight - 1, text.ToString(), fontsize);
            }
            AddText(_chartarea.right + 1, canvaswidth - 1, _chartarea.bottom - fontsize / 2, _chartarea.bottom + fontsize / 2, pag.xaxisname, fontsize);

            Debug.WriteLine("<<< add x axis");
            Debug.WriteLine(yintervall);
            //Add X Axis
            start = new Point() { x = _chartarea.left, y = _chartarea.bottom };
            end = new Point() { x = _chartarea.right, y = _chartarea.bottom };
            AddLine(start, end, Color.Black, 1);
            while(start.y >= _chartarea.top)
            {
                start.y -= yintervall;
                end.y -= yintervall;
                AddLine(start, end, Color.Gray, 0.5);
            }

            return null;
        }

        private void AddDate(double fontsize)
        {
            double midpoint = (_datearea.bottom + _datearea.top) / 2;
            var today = DateTime.Today;
            if (fontsize + 2 < _datearea.height)
            {
                AddText(_datearea.left + 1, _datearea.right - 1, midpoint - fontsize - 1, midpoint + fontsize + 1, today.ToShortDateString(), fontsize);
            }
            else
            {
                AddText(_datearea.left + 1, _datearea.right - 1, _datearea.top + 1, _datearea.bottom - 1, today.ToShortDateString(), _datearea.height - 2);
            }
        }

        private void AddTitle(double fontsize, string title)
        {
            double midpoint = (_titlearea.bottom + _titlearea.top) / 2;
            if (fontsize + 2 < _titlearea.height)
            {
                AddText(_titlearea.left + 1, _titlearea.right - 1, midpoint - fontsize - 1, midpoint + fontsize + 1, title, fontsize);
            }
            else
            {
                AddText(_titlearea.left + 1, _titlearea.right - 1, _titlearea.top + 1, _titlearea.bottom - 1, title, _titlearea.height - 2);
            }
        }

        private string? CalculateChartAreas()
        {
            /*
             * x = width, y = height
             * d = calc dynamically according to seriescount and yaxiswidth
             * 
             * legendarea   titlearea   datearea
             * yaxisarea    chartarea   functionarea
             * 
             * legendarea   (x: 1% - d      | y: 1% - 10%)
             * titlearea    (x: d - 90%     | y: 1% - 10%)
             * datearea     (x: 91% - 99%   | y: 1% - 10%)
             * yaxisarea    (x: 1% - d      | y: 11% - 95%)
             * chartarea    (x: d - 90%     | y: 11% - 95%)
             * functionarea (x: 91% - 99 %  | y: 11% - 95%)
             */

            double d = (settings.yaxiswidth is null) ? seriescount * 20 : seriescount * (double)settings.yaxiswidth;
            if (d > 0.5 * canvaswidth) return "canvas area too small to display graph";
            if (d == 0) d = 20;
            d = d / canvaswidth;

            CalculateRectangle(Area.Legend, 0.01, d, 0.01, 0.1);
            CalculateRectangle(Area.Title, d + 0.01, 0.9, 0.01, 0.1);
            CalculateRectangle(Area.Date, 0.91, 0.99, 0.01, 0.1);
            CalculateRectangle(Area.YAxis, 0.01, d, 0.11, 0.95);
            CalculateRectangle(Area.Chart, d + 0.01, 0.90, 0.11, 0.95);
            CalculateRectangle(Area.Function, 0.91, 0.99, 0.11, 0.95);

            return null;
        }

        enum Area { Legend, Title, Date, YAxis, Chart, Function}
        private void CalculateRectangle(Area area, double x1, double x2, double y1, double y2)
        {
            Rectangle rect = new Rectangle();
            rect.left = canvaswidth * x1;
            rect.top = canvasheight * y1;
            rect.right = canvaswidth * x2;
            rect.bottom = canvasheight * y2;
            rect.width = canvaswidth * x2 - canvaswidth * x1;
            rect.height = canvasheight * y2 - canvasheight * y1;
            switch (area)
            {
                case Area.Legend: _legendarea = rect; break;
                case Area.Title: _titlearea = rect; break;
                case Area.Date: _datearea = rect; break;
                case Area.YAxis: _yaxisarea = rect; break;
                case Area.Chart: _chartarea = rect; break;
                case Area.Function: _functionarea = rect; break;
            }
        }

        private void AddLine(Point _start, Point _end, Color _color, double _thickness)
        {
            if (_start.x == _end.x && _start.y == _end.y) return;
            _lines.Add(new Line() 
            {
                start = _start,
                end = _end,
                color = _color,
                thickness = _thickness
            });
        }

        private void AddText(double _left, double _right, double _top, double _bottom, string _text, double _fontsize) 
        {
            if(_left == _right || _bottom - _top < _fontsize) return;
            _texts.Add(new Text()
            {
                left = _left,
                right = _right,
                top = _top,
                bottom = _bottom,
                text = _text,
                fontsize = _fontsize
            });
        }

    }
}

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
            public double left, top;
            public string text;
        }

        public struct Point
        {
            public double x, y;
        }

        protected double canvaswidth { get; set; }
        protected double canvasheight { get; set; }
        protected PolyAxisGraph pag { get; set; }
        protected Settings settings { get; set; }
        protected int seriescount { get; set; }
        public List<Line> lines {  get; set; }
        public List<Text> texts { get; set; }
        
        public GraphDrawingElements(double _canvaswidth, double _canvasheight, PolyAxisGraph _pag, Settings _settings) 
        {
            canvaswidth = _canvaswidth;
            canvasheight = _canvasheight;
            pag = _pag;
            settings = _settings;
            seriescount = pag.series.Count;
            lines = new List<Line>();
            texts = new List<Text>();
        }
    }
}

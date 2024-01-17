using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PolyAxisGraphs_Backend
{
    /// <summary>
    /// data structure to store data saveable as file.
    /// </summary>
    [DataContract(Name = "SDF", IsReference = true)]
    public class SaveDataFileClass
    {
        /// <summary>
        /// stores data of series.
        /// </summary>
        [DataContract(Name = "series", IsReference = true)]
        public struct SDF_SERIES
        {
            [DataMember]
            public string name {  get; set; }
            [DataMember]
            public List<double> x { get; set; }
            [DataMember]
            public List<double> y { get; set; }
            [DataMember]
            public int min { get; set; }
            [DataMember]
            public int max { get; set; }
            [DataMember]
            public int precision { get; set; }
            [DataMember]
            public double setmin { get; set; }
            [DataMember]
            public double setmax { get; set; }
            [DataMember]
            public double interval { get; set; }
            [DataMember]
            public Color color { get; set; }
            [DataMember]
            public bool active { get; set; }
            [DataMember]
            public bool showfunction { get; set; }
            [DataMember]
            public double[] function { get; set; }
            [DataMember]
            public Regression.FunctionType rft { get; set; }
        }

        /// <summary>
        /// stores data of x axis and pag.
        /// </summary>
        [DataContract(Name = "pag", IsReference = true)]
        public struct SDF_PAG
        {
            [DataMember]
            public string xaxisname { get; set; }
            [DataMember]
            public string filepath { get; set; }
            [DataMember]
            public string charttitle { get; set; }
            [DataMember]
            public double lastx { get; set; }
            [DataMember]
            public int x1 { get; set; }
            [DataMember]
            public int x2 { get; set; }
            [DataMember]
            public int defx1 { get; set; }
            [DataMember]
            public int defx2 { get; set; }
        }

        /// <summary>
        /// list of stored series.
        /// </summary>
        [DataMember]
        List<SDF_SERIES> series { get; set; }
        /// <summary>
        /// stored pag.
        /// </summary>
        [DataMember]
        SDF_PAG pag { get; set; }

        public SaveDataFileClass()
        {
            series = new List<SDF_SERIES>();
            pag = new SDF_PAG();
        }

        /// <summary>
        /// load file to data structure from savedatafileclass.
        /// </summary>
        /// <param name="_sdfc"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static PolyAxisGraph LoadFile(SaveDataFileClass _sdfc, Settings settings)
        {
            PolyAxisGraph _pag = ReadPAG(_sdfc.pag, settings);
            foreach(var _series in _sdfc.series) _pag.series.Add(ReadSeries(_series, settings));
            return _pag;
        }

        /// <summary>
        /// store data in data structure which can be saved as file.
        /// </summary>
        /// <param name="_pag"></param>
        /// <returns></returns>
        public static SaveDataFileClass SaveFile(PolyAxisGraph _pag)
        {
            SaveDataFileClass sdfc = new SaveDataFileClass();
            sdfc.pag = SavePAG(_pag);
            foreach(var _series in _pag.series) sdfc.series.Add(SaveSeries(_series));
            return sdfc;
        }

        private static SDF_SERIES SaveSeries(Series sseries)
        {
            SDF_SERIES sdfseries = new SDF_SERIES()
            {
                name = sseries.name,
                x = sseries.XValues,
                y = sseries.YValues,
                min = sseries.min, 
                max = sseries.max,
                precision = sseries.precision,
                setmin = sseries.setmin,
                setmax = sseries.setmax,
                interval = sseries.interval,
                color = sseries.color,
                active = sseries.active,
                showfunction = sseries.showfunction,
                function = sseries.regressionfunction,
                rft = sseries.rft
            };

            return sdfseries;
        }

        private static Series ReadSeries(SDF_SERIES sdf, Settings settings)
        {
            Series sseries = new Series(sdf.name, sdf.color, settings)
            {
                name = sdf.name,
                XValues = sdf.x,
                YValues = sdf.y,
                min = sdf.min,
                max = sdf.max,
                precision = sdf.precision,
                setmin = sdf.setmin,
                setmax = sdf.setmax,
                interval = sdf.interval,
                color = sdf.color,
                active = sdf.active,
                showfunction = sdf.showfunction,
                regressionfunction = sdf.function,
                rft = sdf.rft
            };

            return sseries;
        }

        private static SDF_PAG SavePAG(PolyAxisGraph ppag)
        {
            SDF_PAG sdfpag = new SDF_PAG()
            {
                xaxisname = ppag.xaxisname,
                filepath = ppag.filepath,
                charttitle = ppag.charttitle,
                lastx = ppag.lastx,
                x1 = ppag.x1,
                x2 = ppag.x2,
                defx1 = ppag.defx1,
                defx2 = ppag.defx2
            };

            return sdfpag;
        }

        private static PolyAxisGraph ReadPAG(SDF_PAG sdf, Settings settings)
        {
            PolyAxisGraph ppag = new PolyAxisGraph(settings)
            {
                xaxisname = sdf.xaxisname,
                filepath = sdf.filepath,
                charttitle = sdf.charttitle,
                lastx = sdf.lastx,
                x1 = sdf.x1,
                x2 = sdf.x2,
                defx1 = sdf.defx1,
                defx2 = sdf.defx2
            };

            return ppag;
        }
    }
}

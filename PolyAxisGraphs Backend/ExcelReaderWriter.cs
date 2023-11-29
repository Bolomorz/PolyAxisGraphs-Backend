using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using excel = Microsoft.Office.Interop.Excel;

namespace PolyAxisGraphs_Backend
{
    public class ExcelReaderWriter
    {
        excel.Application? application { get; set; }
        excel.Workbook? workbook { get; set; }
        excel.Sheets? sheets { get; set; }
        excel.Worksheet? worksheet { get; set; }
        bool opened { get; set; }

        string file { get; set; }

        Settings settings { get; set; }

        public ExcelReaderWriter(string _file, Settings _settings)
        {
            file = _file;
            settings = _settings;
            opened = false;
        }

        public string FindNextFileName()
        {
            string ret = string.Empty;
            int i = 0;
            bool cont = true;

            while (cont)
            {
                string path = settings.initialdirectory + "TestExcel" + i + ".xlsx";
                if (!File.Exists(path))
                {
                    ret = path;
                    cont = false;
                }
                else
                {
                    i++;
                }
            }
            return ret;
        }

        private void EstablishConnection()
        {
            if (File.Exists(file))
            {
                try
                {
                    application = new excel.Application();
                    workbook = application.Workbooks.Open(file);
                    sheets = workbook.Sheets;
                    worksheet = (excel.Worksheet?)sheets[1];
                    opened = true;
                }
                catch
                {

                }
            }
            else
            {
                try
                {
                    application = new excel.Application();
                    workbook = application.Workbooks.Add();
                    sheets = workbook.Sheets;
                    worksheet = (excel.Worksheet?)workbook.ActiveSheet;
                    opened = true;
                }
                catch
                {

                }
            }
        }

        private void Disconnect()
        {
            SaveChanges();
            Quit();
        }

        private void SaveChanges()
        {
            try
            {
                if (workbook is not null)
                {
                    if (File.Exists(file))
                    {
                        workbook.Save();
                    }
                    else
                    {
                        workbook.SaveAs(file);
                    }
                }
            }
            catch
            {
                
            }
        }

        private void Quit()
        {
            try
            {
                if(workbook is not null && application is not null)
                {
                    workbook.Close(0);
                    application.Quit();
                    opened = false;
                }
            }
            catch
            {

            }
        }
    }
}

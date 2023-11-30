using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using excel = Microsoft.Office.Interop.Excel;

namespace PolyAxisGraphs_Backend
{
    public class ExcelReaderWriter
    {
        public struct Cell
        {
            public string? value { get; set; }
            public Color? color { get; set; }
        }
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

        public static string? FindNextFileName(Settings set)
        {
            int i = 0;

            while (set.initialdirectory is not null)
            {
                string path = set.initialdirectory + "TestExcel" + i + ".xlsx";
                if (!File.Exists(path))
                {
                    return path;
                }
                else
                {
                    i++;
                }
            }
            return null;
        }

        public Cell ReadCell(int row, int col)
        {
            string? value = null;
            Color? color = null;

            EstablishConnection();

            if (opened && worksheet is not null)
            {
                try
                {
                    excel.Range range = (excel.Range)worksheet.Cells[row, col];
                    value = range.Value2.ToString();
                    excel.Interior interior = range.Interior;
                    color = ColorTranslator.FromOle((int)interior.Color);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            Disconnect();

            return new Cell() { value = value, color = color };
        }

        public void WriteCell(int row , int col, object value) 
        {
            EstablishConnection();

            if(opened && worksheet is not null)
            {
                try
                {
                    excel.Range range = (excel.Range)worksheet.Cells[row, col];
                    range.Value2 = value;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            Disconnect();
        }

        public void SetColor(int row, int col, Color color)
        {
            EstablishConnection();

            if(opened && worksheet is not null)
            {
                try
                {
                    excel.Range range = (excel.Range)worksheet.Cells[row, col];
                    excel.Interior interior = range.Interior;
                    interior.Color = ColorTranslator.ToOle(color);
                }
                catch (Exception ex) 
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            Disconnect();
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
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
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
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}

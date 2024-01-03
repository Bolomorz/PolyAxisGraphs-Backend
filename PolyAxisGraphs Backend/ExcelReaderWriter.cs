using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCL = Microsoft.Office.Interop.Excel;

namespace PolyAxisGraphs_Backend
{
    /// <summary>
    /// reading and writing of excel file (.xlsx).
    /// </summary>
    public class ExcelReaderWriter
    {
        /// <summary>
        /// cell in excel file.
        /// </summary>
        public struct Cell
        {
            /// <summary>
            /// value of cell.
            /// </summary>
            public string? value { get; set; }
            /// <summary>
            /// color of cell.
            /// </summary>
            public Color? color { get; set; }
        }
        XCL.Application? application { get; set; }
        XCL.Workbook? workbook { get; set; }
        XCL.Sheets? sheets { get; set; }
        XCL.Worksheet? worksheet { get; set; }
        bool opened { get; set; }

        string file { get; set; }

        Settings settings { get; set; }

        /// <summary>
        /// create ERW with filepath and settings.
        /// </summary>
        /// <param name="_file">filepath to excel file.</param>
        /// <param name="_settings">settings of pag.</param>
        public ExcelReaderWriter(string _file, Settings _settings)
        {
            file = _file;
            settings = _settings;
            opened = false;
        }

        /// <summary>
        /// find next unused file name in settings.initialdirectory.
        /// </summary>
        /// <param name="set">settings of pag.</param>
        /// <returns>next unused file name in settings.initialdirectory. returns null if settings.initaldirectory is null.</returns>
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

        /// <summary>
        /// read cell of specified row and column.
        /// </summary>
        /// <param name="row">row in file. starts with 1.</param>
        /// <param name="col">column in file. starts with 1.</param>
        /// <returns>cell in file.</returns>
        public Cell ReadCell(int row, int col)
        {
            string? value = null;
            Color? color = null;

            if (opened && worksheet is not null)
            {
                try
                {
                    XCL.Range range = (XCL.Range)worksheet.Cells[row, col];
                    value = range.Value2.ToString();
                    XCL.Interior interior = range.Interior;
                    color = ColorTranslator.FromOle((int)interior.Color);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            return new Cell() { value = value, color = color };
        }

        /// <summary>
        /// write cell of specified row and column with input value.
        /// </summary>
        /// <param name="row">row in file. starts with 1.</param>
        /// <param name="col">column in file. starts with 1.</param>
        /// <param name="value">input value.</param>
        public void WriteCell(int row , int col, object value) 
        {
            if(opened && worksheet is not null)
            {
                try
                {
                    XCL.Range range = (XCL.Range)worksheet.Cells[row, col];
                    range.Value2 = value;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// set color of cell of specified row and column with input color.
        /// </summary>
        /// <param name="row">row in file. starts with 1.</param>
        /// <param name="col">column in file. starts with 1.</param>
        /// <param name="color">input color.</param>
        public void SetColor(int row, int col, Color color)
        {
            if(opened && worksheet is not null)
            {
                try
                {
                    XCL.Range range = (XCL.Range)worksheet.Cells[row, col];
                    XCL.Interior interior = range.Interior;
                    interior.Color = ColorTranslator.ToOle(color);
                }
                catch (Exception ex) 
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// start excel process and open file.
        /// </summary>
        public void EstablishConnection()
        {
            if (File.Exists(file))
            {
                try
                {
                    application = new XCL.Application();
                    workbook = application.Workbooks.Open(file);
                    sheets = workbook.Sheets;
                    worksheet = (XCL.Worksheet?)sheets[1];
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
                    application = new XCL.Application();
                    workbook = application.Workbooks.Add();
                    sheets = workbook.Sheets;
                    worksheet = (XCL.Worksheet?)workbook.ActiveSheet;
                    opened = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// save changes, close file and quit excel process.
        /// </summary>
        public void Disconnect()
        {
            SaveChanges();
            Quit();
        }

        /// <summary>
        /// save changes to file.
        /// </summary>
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

        /// <summary>
        /// close file and quit excel process
        /// </summary>
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

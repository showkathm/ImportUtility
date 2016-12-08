namespace ATTS.FileDataReader.Readers
{
    using System;
    using Intefaces;
    using System.IO;
    using System.Runtime.InteropServices;
    using Excel = Microsoft.Office.Interop.Excel;

    public class ExcelDataReader : IFileDataReader
    {
        //Create COM Objects. Create a COM object for everything that is referenced
        Excel.Application _xlApp;
        Excel.Workbook _xlWorkbook;
        Excel._Worksheet _xlWorksheet;
        Excel.Range _xlRange;
        private int _rowIndex;
        private string[] _values;


        #region IFileDataReader implementation
        public bool Read()
        {
            //as data is collected via row index, just update the index
            _rowIndex++;

            if (_rowIndex <= RowCount)
            {
                Array values = _xlRange.Cells.Rows[_rowIndex].Value2 as Array;
                if (values != null)
                {
                    _values = ConvertToStringArray(values);
                    return true;
                }
            }
            return false;
        }

        public int RowCount { get; private set; }

        public decimal? GetValueAsDecimal(int index)
        {
            if (index >= _values.Length) throw new IndexOutOfRangeException();

            decimal rtn;
            if (decimal.TryParse(_values[index], out rtn))
            {
                return rtn;
            }
            return null;
        }

        public string GetValue(int index)
        {
            if (index >= _values.Length) throw new IndexOutOfRangeException();

            return _values[index];
        }

        #endregion


        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {

                //cleanup
                GC.Collect();
                GC.WaitForPendingFinalizers();

                //release com objects to fully kill excel process from running in the background
                Marshal.ReleaseComObject(_xlRange);
                Marshal.ReleaseComObject(_xlWorksheet);

                //close and release
                _xlWorkbook.Close();
                Marshal.ReleaseComObject(_xlWorkbook);

                //quit and release
                _xlApp.Quit();
                Marshal.ReleaseComObject(_xlApp);

                _disposedValue = true;
            }
        }


        ~ExcelDataReader()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        internal void Open(string path, bool firstRowIsHeader)
        {
            if (!File.Exists(path)) throw new ArgumentException("Path is not valid", "path");

            _xlApp = new Excel.Application();
            _xlWorkbook = _xlApp.Workbooks.Open(path);
            _xlApp.DisplayAlerts = false;
            _xlWorksheet = _xlWorkbook.Sheets[1];
            _xlRange = _xlWorksheet.UsedRange;

            RowCount = _xlRange.Rows.Count;
            
            //If the first row is header, update the index
            if (firstRowIsHeader)
            {
                _rowIndex++;
            }
        }

        private string[] ConvertToStringArray(Array values)
        {

            // create a new string array
            string[] theArray = new string[values.Length];

            // loop through the 2-D System.Array and populate the 1-D String Array
            for (int i = 1; i <= values.Length; i++)
            {
                if (values.GetValue(1, i) == null)
                    theArray[i - 1] = "";
                else
                    theArray[i - 1] = values.GetValue(1, i).ToString();
            }

            return theArray;
        }
    }
}
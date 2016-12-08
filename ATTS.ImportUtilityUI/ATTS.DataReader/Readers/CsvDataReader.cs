namespace ATTS.FileDataReader.Readers
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using Intefaces;

    public class CsvDataReader : IFileDataReader
    {
        private static readonly Regex CsvSplitter = new Regex(@",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))");
        private TextReader _textReader;
        private string[] _values;
        

        #region IFileDataReader implementation
        public bool Read()
        {
            if(_textReader==null) throw new NullReferenceException("CSV file object is null");

            string line = _textReader.ReadLine();

            if (line == null)
            {
                return false;
            }
            _values = CsvSplitter.Split(line);
            return true;
        }

        public int RowCount { get; private set; }

        public decimal? GetValueAsDecimal(int index)
        {
            if (index >= _values.Length) throw new IndexOutOfRangeException();

            decimal rtn;
            if(decimal.TryParse(_values[index], out rtn))
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
                if (disposing)
                {
                    if (_textReader != null)
                    {
                        _textReader.Close();
                        _textReader.Dispose();
                    }
                }
                _disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion

        internal void Open(string path, bool firstRowIsHeader)
        {
            if (!File.Exists(path)) throw new ArgumentException("Path is not valid", "path");

            _textReader = new StreamReader(path);

            //If the first row is header, read the firsrt row now
            if (firstRowIsHeader)
            {
                _textReader.ReadLine();
            }

            //set the row count
            RowCount = GetFileRowCount(path, firstRowIsHeader);
        }

        private int GetFileRowCount(string path, bool firstRowIsHeader)
        {
            int rowCount = 0;

            //not a great way to get the number of rows, but can not use read all just in case 
            //the file is too large for available memory
            using (TextReader reader = new StreamReader(path))
            {
                while (reader.ReadLine() != null)
                {
                    rowCount++;
                }
            }

            if (firstRowIsHeader)
            {
                rowCount--;
            }

            return rowCount;
        }
    }
}

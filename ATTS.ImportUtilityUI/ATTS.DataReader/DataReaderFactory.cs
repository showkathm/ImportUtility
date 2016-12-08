namespace ATTS.FileDataReader
{
    using System;
    using Intefaces;
    using Readers;

    public static class DataReaderFactory
    {
        public static IFileDataReader Get(string path, bool firstRowIsHeader)
        {
            IFileDataReader reader;

            if (path.ToLower().EndsWith(".csv"))
            {
                reader = new CsvDataReader();
                ((CsvDataReader)reader).Open(path, firstRowIsHeader);
            }
            else if (path.ToLower().EndsWith(".xlsx"))
            {
                reader = new ExcelDataReader();
                ((ExcelDataReader)reader).Open(path, firstRowIsHeader);
            }
            else
            {
                throw new ArgumentException("Unrecognised file format", "path");
            }

            return reader;
        }
    }
}

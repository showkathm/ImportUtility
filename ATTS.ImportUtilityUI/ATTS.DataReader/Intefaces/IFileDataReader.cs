namespace ATTS.FileDataReader.Intefaces
{
    using System;

    public interface IFileDataReader : IDisposable
    {

        int RowCount { get; }

        bool Read();
        
        string GetValue(int index);

        decimal? GetValueAsDecimal(int index);
        
    }
}

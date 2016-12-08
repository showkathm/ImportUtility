namespace ATTS.ImportUtilityUI.Models
{
    public class RowError
    {
        public string Error { get; set; }

        public int RowId { get; set; }

        public override string ToString()
        {
            return string.Format("Row {0} - {1}", RowId, Error);
        }
    }
}

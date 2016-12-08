namespace ATTS.DataAccess.Models
{
    using System.ComponentModel.DataAnnotations;
    using CustomValidations;

    public class UploadItem
    {
        public int Id { get; set; }

        [Required]
        public string Account { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Currency Code")]
        [ValidCurrencyCode]
        public string CurrencyCode { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = @"Please enter a value bigger than {1}")]
        public decimal? Value { get; set; }
    }
}

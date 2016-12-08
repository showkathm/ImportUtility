namespace ATTS.DataAccess.CustomValidations
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;

    public class ValidCurrencyCode : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool isValid = true;
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                string cc = value.ToString();

                IEnumerable<string> currencySymbols = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                    //Only specific cultures contain region information
                    .Select(x => (new RegionInfo(x.LCID)).ISOCurrencySymbol)
                    .Distinct()
                    .OrderBy(x => x);

                if (!currencySymbols.Contains(cc))
                {
                    isValid = false;
                }
            }

            if (isValid)
            {
                return ValidationResult.Success;
            }
            return
                new ValidationResult("Currency Code must be a valid ISO 4217 currency code");

        }
    }
}

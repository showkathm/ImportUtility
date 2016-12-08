using System.Collections.Generic;
using System.Data.Entity.Validation;

namespace ATTS.DataAccess
{
    using System;
    using System.Linq;
    using Models;

    public class AttsDbService : IDisposable
    {
        private readonly AttsDbContext _context;

        public AttsDbService()
        {
            _context = new AttsDbContext();
        }
        
        public bool AddItemToDataBase(UploadItem item)
        {
            if (_context != null)
            {
                //add our item to the list
                _context.UploadItems.Add(item);
                ValidationErrors = Validate();

                if (ValidationErrors.Any())
                {
                    //remove the recent item
                    _context.UploadItems.Remove(item);
                }
                else
                {
                    _context.SaveChanges();
                }
            }

            return !ValidationErrors.Any();
        }

        public List<string> ValidationErrors { get; private set; }

        private List<string> Validate()
        {
            List<string> validations = new List<string>();

            var validationresults = _context.GetValidationErrors();

            foreach (DbEntityValidationResult validationresult in validationresults)
            {
                if (!validationresult.IsValid)
                {
                    foreach (DbValidationError error in validationresult.ValidationErrors)
                    {
                        validations.Add(error.ErrorMessage);
                    }
                }
            }

            return validations;
        }

        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (_context != null)
                    {
                        _context.Dispose();
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
    }
}

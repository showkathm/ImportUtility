namespace ATTS.ImportUtilityUI.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using Models;
    using FileDataReader.Intefaces;
    using FileDataReader;
    using Helpers;
    using DataAccess.Models;
    using DataAccess;

    public class ImportDateViewModel : ViewModelBase
    {
        #region Local variables

        private string _path;
        private bool _firstRowIsHeader;
        private bool _isImporting;
        private bool _showImportProgress;
        private int _numberOfRows;
        private int _currentRow;
        private int _failedRowsCount;
        private ObservableCollection<RowError> _errors = new ObservableCollection<RowError>();
        private RelayCommand _doImportCommand;
        private RelayCommand _displayFileDialogCommand;

        #endregion

        #region Public UI properties 
        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    OnPropertyChanged();
                }
            }

        }

        public bool FirstRowIsHeader
        {
            get { return _firstRowIsHeader; }
            set
            {
                if (_firstRowIsHeader != value)
                {
                    _firstRowIsHeader = value;
                    OnPropertyChanged();
                }
            }

        }

        public bool ShowImportProgress
        {
            get { return _showImportProgress; }
            set
            {
                if (_showImportProgress != value)
                {
                    _showImportProgress = value;
                    OnPropertyChanged();
                }
            }

        }

        public int NumberOfRows
        {
            get { return _numberOfRows; }
            set
            {
                if (_numberOfRows != value)
                {
                    _numberOfRows = value;
                    OnPropertyChanged();
                }
            }

        }

        public int CurrentRow
        {
            get { return _currentRow; }
            set
            {
                if (_currentRow != value)
                {
                    _currentRow = value;
                    OnPropertyChanged();
                }
            }

        }

        public int FailedRowsCount
        {
            get { return _failedRowsCount; }
            set
            {
                if (_failedRowsCount != value)
                {
                    _failedRowsCount = value;
                    OnPropertyChanged();
                }
            }

        }

        public ObservableCollection<RowError> Errors
        {
            get { return _errors; }
            set
            {
                if (_errors != value)
                {
                    _errors = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand DoImportCommand
        {
            get
            {
                if (_doImportCommand == null)
                {
                    _doImportCommand = new RelayCommand(param => DoImport(), 
                                                        param => CanDoImportCommand());
                }
                return _doImportCommand;
            }
        }

        public ICommand DisplayFileDialogCommand
        {
            get
            {
                if (_displayFileDialogCommand == null)
                {
                    _displayFileDialogCommand = new RelayCommand(param => DisplayFileDialog());
                }
                return _displayFileDialogCommand;
            }
        }

        #endregion

        #region Helper functions
        private bool CanDoImportCommand()
        {
            return !_isImporting;
        }

        private async Task DoImport()
        {
            if (!string.IsNullOrEmpty(Path))
            {
                CurrentRow = 0;
                _isImporting = true;
                ShowImportProgress = true;
                Errors = new ObservableCollection<RowError>();

                await Task.Run(() =>
                {
                    DoImportWorker();
                });

                _isImporting = false;
                
            }
        }

        private void DoImportWorker()
        {
            //create our data reader for the passed in file
            using (IFileDataReader dataReader = DataReaderFactory.Get(Path, FirstRowIsHeader))
            {
                using (AttsDbService service = new AttsDbService())
                {
                    //set the number of rows
                    NumberOfRows = dataReader.RowCount;
                    
                    //for each row in the file, upload to database
                    while (dataReader.Read())
                    {
                        try
                        {
                            CurrentRow++;
                            
                            if (!service.AddItemToDataBase(new UploadItem
                            {
                                Account = dataReader.GetValue(0),
                                Description = dataReader.GetValue(1),
                                CurrencyCode = dataReader.GetValue(2),
                                Value = dataReader.GetValueAsDecimal(3)
                            }))
                            {
                                foreach (var error in service.ValidationErrors)
                                {
                                    System.Windows.Application.Current.Dispatcher.Invoke(delegate
                                    {
                                        Errors.Add(new RowError() { Error = error, RowId = CurrentRow });
                                        FailedRowsCount = _errors.GroupBy(x => x.RowId).Count();
                                    });
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(delegate
                            {
                                Errors.Add(new RowError() { Error = e.Message, RowId = CurrentRow });
                                FailedRowsCount = _errors.GroupBy(x => x.RowId).Count();
                            });
                        }
                    }
                }
            }
        }

        private void DisplayFileDialog()
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".png",
                Filter = "Excel Files (*.xlsx)|*.xlsx|CSV Files (*.csv)|*.csv"
            };


            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result.HasValue && result.Value)
            {
                Path = dlg.FileName;
            }
        }
        
        #endregion

    }
}

namespace ATTS.ImportUtilityUI
{
    using System.Windows;
    using ViewModels;
    using Views;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create the ViewModel to which the main window binds.
            var viewModel = new ImportDateViewModel();

            // Allow all controls in the window to bind to the ViewModel by setting the
            // DataContext, which propagates down the element tree.
            var view = new ImportDateView {DataContext = viewModel};

            //show the window
            view.Show();
        }
    }
}

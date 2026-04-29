using SmartSaving.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SmartSaving.Views
{
    public partial class SearchWindow : Window
    {
        public SearchWindow(SearchViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.CloseAction = () =>
            {
                if (IsLoaded) Hide();
            };

            viewModel.ReopenAction = () =>
            {
                if (IsLoaded) Show();
            };

            Closed += (s, e) =>
            {
                viewModel.CloseAction = null;
                viewModel.ReopenAction = null;
            };
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is SearchViewModel vm)
                vm.OpenTransactionCommand.Execute(null);
        }
    }
}

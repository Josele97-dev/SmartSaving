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
        }
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is SearchViewModel vm)
                vm.OpenTransactionCommand.Execute(null);
        }
    }
}

using SmartSaving.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace SmartSaving.Views
{
  
    public partial class MonthlyTransactionsWindow : Window
    {
        public MonthlyTransactionsWindow(MonthlyTransactionsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MonthlyTransactionsViewModel vm)
                vm.OpenTransactionCommand.Execute(null);
        }
    }
}

using SmartSaving.ViewModels;
using System.Windows;


namespace SmartSaving.Views
{
    
    public partial class TransferWindow  : Window
    {
        public TransferWindow(TransferViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.CloseAction = () => Close();
        }
    }
}

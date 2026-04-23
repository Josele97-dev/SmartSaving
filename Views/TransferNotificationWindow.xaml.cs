using SmartSaving.ViewModels;
using System.Windows;

namespace SmartSaving.Views
{
   
    public partial class TransferNotificationWindow : Window
    {
        private readonly TransferNotificationViewModel _viewModel;

        public TransferNotificationWindow(TransferNotificationViewModel viewModel)
        {
           InitializeComponent();
           DataContext = viewModel;
           _viewModel = viewModel;
        }
        private async void DismissButton_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.MarkAsReadAsync();
            Close();
        }
    }
}

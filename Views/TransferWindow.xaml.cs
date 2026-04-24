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

            viewModel.RequestConfirmAction = async (recipientName, amount) =>
            {
                this.Hide();

                var dialog = new TransferConfirmDialog(
                    $"You are about to send €{amount:N2} to {recipientName}.\n\nAre you sure?");
                dialog.Show();

                var confirmed = await dialog.WaitForResultAsync();

                if (!confirmed)
                {
                    viewModel.ClearForm();
                    this.Show();
                    return false;
                }

                return true;
            };

            viewModel.ShowSuccessAction = (recipientName, amount) =>
            {
                // Find the open confirm dialog and show success in it
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is TransferConfirmDialog dialog)
                    {
                        dialog.ShowSuccess($"€{amount:N2} successfully sent to {recipientName}!");
                        break;
                    }
                }
            };
            viewModel.ShowErrorAction = (message) =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is TransferConfirmDialog dialog)
                    {
                        dialog.ShowError(message);
                        dialog.Activate();
                        break;
                    }
                }
                this.Show();
            };
        }
    }
}

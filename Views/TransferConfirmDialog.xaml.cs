using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SmartSaving.Views
{

    public enum TransferConfirmResult
    {
        Confirmed,
        Cancelled
    }

    public partial class TransferConfirmDialog : Window
    {
        private readonly TaskCompletionSource<bool> _tcs = new();

        public Task<bool> WaitForResultAsync() => _tcs.Task;

        public TransferConfirmDialog(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        public void ShowSuccess(string message)
        {
            ConfirmContent.Visibility = Visibility.Collapsed;
            SuccessContent.Visibility = Visibility.Visible;
            SuccessText.Text = message;

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += (s, args) => { timer.Stop(); Close(); };
            timer.Start();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
            _tcs.SetResult(true);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _tcs.SetResult(false);
            Close();
        }
    }
}

using System.Windows;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SmartSaving.Views
{
    public enum BudgetWarningResult
    {
        Proceed,
        UpdateLimit,
        Cancel
    }

    public partial class BudgetWarningDialog : Window
    {
        public BudgetWarningResult Result { get; private set; } = BudgetWarningResult.Cancel;

        public BudgetWarningDialog(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        private  void ProceedButton_Click(object sender, RoutedEventArgs e)
        {
            Result = BudgetWarningResult.Proceed;

            
            WarningContent.Visibility = Visibility.Collapsed;
            SuccessContent.Visibility = Visibility.Visible;

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                Close();
            };
            timer.Start();
        }

        private void UpdateLimitButton_Click(object sender, RoutedEventArgs e)
        {
            Result = BudgetWarningResult.UpdateLimit;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = BudgetWarningResult.Cancel;
            Close();
        }
    }
}

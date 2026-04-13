using System.Windows;

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

        private void ProceedButton_Click(object sender, RoutedEventArgs e)
        {
            Result = BudgetWarningResult.Proceed;
            Close();
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

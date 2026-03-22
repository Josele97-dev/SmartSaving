using SmartSaving.ViewModels;
using System.Windows;

namespace SmartSaving.Views
{
    public partial class TransactionWindow : Window
    {
        public TransactionWindow(TransactionViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }
    }
}
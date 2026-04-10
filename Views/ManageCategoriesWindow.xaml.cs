using SmartSaving.ViewModels;
using System.Windows;

namespace SmartSaving.Views
{
    public partial class ManageCategoriesWindow : Window
    {
        public ManageCategoriesWindow(ManageCategoriesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}

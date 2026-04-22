using SmartSaving.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SmartSaving.Views
{

    public partial class TransfersRecordWindow : Window
    {
        public TransfersRecordWindow(TransfersRecordViewModel viewModel)
        {

            InitializeComponent();
            DataContext = viewModel;
        }
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListView lv) lv.SelectedItem = null;
        }
    }

}


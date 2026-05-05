using SmartSaving.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartSaving.Views
{

    

    public partial class RegisterWindow : Window
    {
        public RegisterWindow(RegisterViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb && DataContext is RegisterViewModel vm)
            {
                vm.Password = pb.Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb && DataContext is RegisterViewModel vm)
            {
                vm.ConfirmPassword = pb.Password;
            }
        }
    }

    

}
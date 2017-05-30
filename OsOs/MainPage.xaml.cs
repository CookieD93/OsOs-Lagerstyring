using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using OsOs.Utilities;
using OsOs.ViewModel;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OsOs
{
    public sealed partial class MainPage : Page
    {
        private LoginViewModel viewModel;
        public MainPage()
        {
            this.InitializeComponent();
            viewModel = new LoginViewModel();
            DataContext = viewModel;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (sender == null || viewModel.Username == null)
            {
                MessageDialogHelper.Show("Udfyld venligst dit brugernavn og adgangskode", "Fejl");
                return;
            }

            PasswordBox pwBox = passwordBox;
            var user = from Employee in viewModel.Employees
                where Employee.Username.ToLower().Equals(viewModel.Username.ToLower())
                where Employee.Password.Equals(viewModel.Crypt.ConvertData(pwBox.Password))
                select Employee;

            if (user.Any())
            {
                Singleton.GetInstance().EmployeeId= user.First().Id;
                Singleton.GetInstance().EmployeeFunction = user.First().Employee_Function.Id;

                if (user.First().Employee_Function.Id == 1)
                    this.Frame.Navigate(typeof(View.ForsideView));
                else if (user.First().Employee_Function.Id == 2)
                    this.Frame.Navigate(typeof(View.ForsideView));
                else
                    this.Frame.Navigate(typeof(View.Lager.OpgavekoView));
            }
            else
                MessageDialogHelper.Show("Brugernavn/Adgangskode stemmer ikke overens", "Fejl");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using OsOs.Annotations;
using OsOs.Handler;
using OsOs.Model;
using OsOs.Utilities;


namespace OsOs.ViewModel
{
    class LoginViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Employee> Employees { get; set; }
        public HashConverter Crypt;

        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); }
        }

        public LoginViewModel()
        {
            if (Singleton.GetInstance().EmployeeId > 0 || Singleton.GetInstance().EmployeeFunction > 0)
            {
                Singleton.GetInstance().EmployeeId = -1;
                Singleton.GetInstance().EmployeeFunction = -1;
            }
            Singleton.GetInstance().LoadEmployeesAndFunctions();
            Employees = Singleton.GetInstance().Employees;
            Crypt = new HashConverter();
        }

        #region INotify
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

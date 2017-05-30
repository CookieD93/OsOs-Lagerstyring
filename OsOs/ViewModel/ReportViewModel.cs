using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OsOs.Annotations;
using OsOs.Handler;
using OsOs.Model;

namespace OsOs.ViewModel
{
    class ReportViewModel : INotifyPropertyChanged
    {
        public ReportHandler reportHandler { get; set; }
        public ObservableCollection<Unit> Units { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Product_Location> Locations { get; set; }

        public ReportViewModel()
        {
            reportHandler=new ReportHandler(this);
            Units = Singleton.GetInstance().Units;
            Products = Singleton.GetInstance().Products;
            Locations = Singleton.GetInstance().Locations;
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

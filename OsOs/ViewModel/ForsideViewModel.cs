using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using OsOs.Utilities;

namespace OsOs.ViewModel
{
    class ForsideViewModel
    {

        public Visibility LagerBigView { get; set; }
        public Visibility GrundBigView { get; set; }
        public Visibility OrderBigView { get; set; }
        public Visibility ReportBigView { get; set; }
        public Visibility LagerSmallView { get; set; }
        public Visibility GrundSmallView { get; set; }
        public Visibility OrderSmallView { get; set; }
        public Visibility ReportSmallView { get; set; }

        public ForsideViewModel()
        {
            Singleton.GetInstance().FetchCollections();
            LagerBigView = Visibility.Visible;
            GrundBigView = Visibility.Visible;
            OrderBigView = Visibility.Visible;
            ReportBigView = Visibility.Visible;
            LagerSmallView = Visibility.Visible;
            GrundSmallView = Visibility.Visible;
            OrderSmallView = Visibility.Visible;
            ReportSmallView = Visibility.Visible;
            
            if (Singleton.GetInstance().EmployeeFunction == 2)
            {
                LagerBigView = Visibility.Collapsed;
                LagerSmallView = Visibility.Collapsed;
            }
            else if(Singleton.GetInstance().EmployeeFunction==3)
            {
                GrundBigView = Visibility.Collapsed;
                OrderBigView = Visibility.Collapsed;
                ReportBigView = Visibility.Collapsed;
                GrundSmallView = Visibility.Collapsed;
                OrderSmallView = Visibility.Collapsed;
                ReportSmallView = Visibility.Collapsed;
            }
        }
    }
}

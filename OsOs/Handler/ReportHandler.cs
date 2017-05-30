using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsOs.Model;
using OsOs.ViewModel;

namespace OsOs.Handler
{
    class ReportHandler
    {
        public ReportViewModel ViewModel { get; set; }
        public ReportHandler(ReportViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        

    }
}

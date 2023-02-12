using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Mega7_2023
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjExODM3QDMyMzAyZTM0MmUzMG9Ya01SMlRIMWJPRGlaVGNrT2huUExMRlFSYjkxcTZLMWp0c3c2NGJ5Vjg9");
        }

    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Daz_Package_Manager
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Please report error at: https://github.com/TheNoobDucky/Daz-Package-Manager" +
                "An unhandled exception just occurred: " + e.Exception.Message + "\n\n\n" +

                "Stack trace:\n"+ e.Exception.StackTrace, "Error Massage", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }
    }
}

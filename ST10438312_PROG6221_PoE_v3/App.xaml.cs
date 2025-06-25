using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ST10438312_PROG6221_PoE_v3.Windows;

namespace ST10438312_PROG6221_PoE_v3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            var mainWindow = new MainWindow();
            this.MainWindow = mainWindow;
            mainWindow.Hide();

            var splashWindow = new StartupWindow();
            splashWindow.Show();

            // Wait 4 seconds
            await Task.Delay(4000);

            // Close splash and show main window
            splashWindow.Close();
            mainWindow.Show();

            base.OnStartup(e);
        }
    }
}

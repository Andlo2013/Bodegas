using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using AutomatizerSQL.Bodega.GUI.ViewModels;
using AutomatizerSQL.Core;
using AutomatizerSQL.Data.SqlServer;
using DevExpress.Mvvm.POCO;
using SesionStarterGui;

namespace PruebaBodega
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnAppStartup(object sender, StartupEventArgs e)
        {
            DevExpress.Xpf.Core.ApplicationThemeHelper.UpdateApplicationThemeName();
            var sesion = SesionStarterHelper.GetSesion( new SqlServerDataAccesResolver()  );
            var factory = ViewModelSource.Factory((Sesion s) => new MainViewViewModel(s));
            var mainViewModel = factory(sesion); //new MainViewViewModel(sesion);
            var mainWindow  =  new MainWindow();
            mainWindow.DataContext = mainViewModel;
            mainWindow.ShowDialog();




        }




    }
}

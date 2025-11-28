using System.Windows;
using Malom.ViewModel;
using Malom_WPF.View;

namespace Malom_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainViewModel? mainViewModel;
        MainWindow? mainWindow;
        protected override void OnStartup(StartupEventArgs e)
        {
            mainViewModel = new MainViewModel(new DialogService());
            mainWindow = new();
            this.MainWindow.DataContext = mainViewModel;
            mainWindow.Show();
        }

    }

}

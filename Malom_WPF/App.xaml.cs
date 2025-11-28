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
        MainViewModel mainViewModel;
        MainWindow mainWindow;
        public App()
        {
            mainViewModel = new MainViewModel(new DialogService());
            InitializeComponent();
            mainWindow = new();
            this.MainWindow.DataContext = mainViewModel;
        }

    }

}

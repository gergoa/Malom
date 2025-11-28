using System.Windows;
using Malom.Service;
using Malom.ViewModel;

namespace Malom_WPF.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel mainViewModel;
        public MainWindow()
        {
            mainViewModel = new MainViewModel(new DialogService());
            DataContext = mainViewModel;
            InitializeComponent();
        }
    }
}
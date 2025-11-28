using System.Windows;
using Malom.Model;

namespace Malom_WPF.View
{
    public class DialogService : IDialogService
    {
        public bool DisplayYesNoMessageBox(string message, string title)
        {
            return MessageBox.Show(message,
                            title,
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Asterisk) == MessageBoxResult.Yes;
        }
        public string DisplayOpenFileDialog(string defaultPath, string filter)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = defaultPath,
                Filter = filter
            };
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                return openFileDialog.FileName;
            }
            return string.Empty;
        }

        public string DisplaySaveFileDialog(string defaultPath, string filter)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new()
            {
                InitialDirectory = defaultPath,
                Filter = filter
            };
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                return saveFileDialog.FileName;
            }
            return string.Empty;
        }

        public void CloseApplication()
        {
            Application.Current.Shutdown();
        }
    }
}

namespace Malom.Model
{
    public interface IDialogService
    {
        public bool DisplayYesNoMessageBox(string message, string title);
        public string DisplayOpenFileDialog(string defaultPath, string filter);
        public string DisplaySaveFileDialog(string defaultPath, string filter);
        public void CloseApplication();
    }
}
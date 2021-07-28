using System.Windows.Forms;

namespace OsHelper
{
    public class SelectFolder
    {
        public static (bool success, string location) AskForLocation()
        {
            FolderBrowserDialog dialog = new();
            var result = dialog.ShowDialog();
            return result == DialogResult.OK ? (true, dialog.SelectedPath) : (false, null);
        }
    }
    public class SelectFile
    {
        public static (bool success, string location) AskForOpenLocation()
        {
            OpenFileDialog dialog = new();
            var result = dialog.ShowDialog();
            return result == DialogResult.OK ? (true, dialog.FileName) : (false, null);
        }

        public static (bool success, string location) AskForSaveLocation(string filter = "json file (*.json)|*.json|text file (*.txt)|*.txt|All files|*.*")
        {
            SaveFileDialog dialog = new();
            dialog.Filter = filter;
            var result = dialog.ShowDialog();
            return result == DialogResult.OK ? (true, dialog.FileName) : (false, null);
        }
    }
}


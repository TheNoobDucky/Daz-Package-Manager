using System.Windows.Forms;

namespace OsHelper
{
    public class SelectFolder
    {
        public static (bool, string) AskForLocation()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return (true, dialog.SelectedPath);
            }
            return (false, null);
        }
    }
    public class SelectFile
    {
        public static (bool success, string location) AskForLocation()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return (true, dialog.FileName);
            }
            return (false, null);
        }
    }
}


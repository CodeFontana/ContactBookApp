using Microsoft.Win32;

namespace ContactBook.Services;

public class WindowDialogService : IDialogService
{
    public string OpenFile(string filter)
    {
        OpenFileDialog dialog = new();

        if (dialog.ShowDialog() == true)
        {
            return dialog.FileName;
        }

        return null;
    }

    public void ShowMessageBox(string message)
    {
        
    }
}

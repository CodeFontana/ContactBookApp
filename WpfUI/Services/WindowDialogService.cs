using System.Windows;
using Microsoft.Win32;

namespace WpfUI.Services;

public class WindowDialogService : IDialogService
{
    public string? OpenFile(string filter)
    {
        OpenFileDialog dialog = new()
        {
            Filter = filter
        };

        if (dialog.ShowDialog() == true)
        {
            return dialog.FileName;
        }

        return null;
    }

    public void ShowMessageBox(string message)
    {
        MessageBox.Show(
            message,
            "Contact Book",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    public bool Confirm(string message)
    {
        MessageBoxResult result = MessageBox.Show(
            message,
            "Contact Book",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);

        return result == MessageBoxResult.Yes;
    }
}

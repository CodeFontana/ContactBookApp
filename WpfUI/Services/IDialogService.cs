namespace WpfUI.Services;

public interface IDialogService
{
    string OpenFile(string filter);
    void ShowMessageBox(string message);
}

namespace WpfUI.Services;

public interface IDialogService
{
    string? OpenFile(string filter);

    void ShowMessageBox(string message);

    bool Confirm(string message);
}

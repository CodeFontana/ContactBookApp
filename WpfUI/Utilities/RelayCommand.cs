using System;
using System.Windows.Input;

namespace WpfUI.Utilities;

public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Func<T, bool> _canExecute;

    public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute ?? (_ => true);
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) =>
        TryGetParameter(parameter, out var value) && _canExecute(value);

    public void Execute(object? parameter)
    {
        if (!TryGetParameter(parameter, out var value))
            return;

        _execute(value);
    }

    private static bool TryGetParameter(object? parameter, out T value)
    {
        // Handles:
        // - parameter already of type T
        // - null flowing into reference-type or nullable value-type T
        if (parameter is T t)
        {
            value = t;
            return true;
        }

        if (parameter is null && default(T) is null)
        {
            value = default!;
            return true;
        }

        value = default!;
        return false;
    }
}

public class RelayCommand : RelayCommand<object?>
{
    public RelayCommand(Action execute)
        : base(_ => execute()) { }

    public RelayCommand(Action execute, Func<bool> canExecute)
        : base(_ => execute(), _ => canExecute()) { }
}

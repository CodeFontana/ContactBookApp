using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DataAccessLibrary.Models;

public class ObservableObject : INotifyPropertyChanged, INotifyDataErrorInfo
{
    // Per-property error lists. Empty when the object is valid.
    private readonly Dictionary<string, List<string>> _errors = [];

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public bool HasErrors
    {
        get { return _errors.Count > 0; }
    }

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return _errors.Values.SelectMany(v => v);
        }

        return _errors.TryGetValue(propertyName, out List<string>? errors)
            ? errors
            : Enumerable.Empty<string>();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        ValidateProperty(propertyName);
    }

    protected virtual bool OnPropertyChanged<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value))
        {
            return false;
        }

        backingField = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void ValidateProperty(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return;
        }

        PropertyInfo? prop = GetType().GetProperty(propertyName);

        // Skip computed / read-only / unknown properties — nothing to validate.
        if (prop is null || !prop.CanWrite)
        {
            return;
        }

        object? value = prop.GetValue(this);
        ValidationContext context = new(this) { MemberName = propertyName };
        List<ValidationResult> results = [];
        Validator.TryValidateProperty(value, context, results);

        bool changed;
        if (results.Count == 0)
        {
            changed = _errors.Remove(propertyName);
        }
        else
        {
            _errors[propertyName] = results
                .Select(r => r.ErrorMessage ?? string.Empty)
                .ToList();
            changed = true;
        }

        if (changed)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            // HasErrors is read-only and has no validation attributes, so its OnPropertyChanged
            // is a one-shot — it won't recurse back through ValidateProperty in any meaningful way.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasErrors)));
        }
    }
}

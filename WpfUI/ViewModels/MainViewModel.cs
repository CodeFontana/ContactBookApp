using WpfUI.Services;
using WpfUI.Utilities;
using DataAccessLibrary;

namespace WpfUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        // For design mode
    }

    public MainViewModel(ContactDbContextFactory dbContext, 
                         IDialogService dialogService)
    {
        BookVM = new BookViewModel(dbContext, dialogService);
        CurrentViewModel = BookVM;

    }

    private object _currentViewModel;
    public object CurrentViewModel
    {
        get
        {
            return _currentViewModel;
        }
        set
        {
            OnPropertyChanged(ref _currentViewModel, value);
        }
    }

    private BookViewModel _bookVM;
    public BookViewModel BookVM
    {
        get
        {
            return _bookVM;
        }
        set
        {
            OnPropertyChanged(ref _bookVM, value);
        }
    }
}

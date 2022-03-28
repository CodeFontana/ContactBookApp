using ContactBook.Services;
using ContactBook.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ContactBook.ViewModels;
public class BookViewModel : ObservableObject
{
    private readonly IContactDataService _dataService;
    private readonly IDialogService _dialogService;

    public BookViewModel(IContactDataService dataService, 
                         IDialogService dialogService)
    {
        ContactsVM = new ContactsViewModel(dataService, dialogService);
        LoadContactsCommand = new RelayCommand(LoadContacts);
        LoadFavoritesCommand = new RelayCommand(LoadFavorites);
        _dataService = dataService;
        _dialogService = dialogService;
        LoadContacts();
    }

    private ContactsViewModel _contactsVM;
    public ContactsViewModel ContactsVM
    {
        get
        {
            return _contactsVM;
        }
        set
        {
            OnPropertyChanged(ref _contactsVM, value);
        }
    }

    public ICommand LoadContactsCommand { get; private set; }
    public ICommand LoadFavoritesCommand { get; private set; }

    private void LoadContacts()
    {
        ContactsVM.LoadContacts(_dataService.GetContacts());
    }

    private void LoadFavorites()
    {
        IEnumerable<Models.Contact> favorites = _dataService.GetContacts().Where(c => c.IsFavorite);
        ContactsVM.LoadContacts(favorites);
    }
}

using ContactBook.Services;
using ContactBook.Utilities;
using DataAccessLibrary;
using DataAccessLibrary.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ContactBook.ViewModels;
public class BookViewModel : ObservableObject
{
    private readonly ContactDbContextFactory _dbContext;
    private readonly IDialogService _dialogService;

    public BookViewModel(ContactDbContextFactory dbContext, 
                         IDialogService dialogService)
    {
        ContactsVM = new ContactsViewModel(dbContext, dialogService);
        LoadContactsCommand = new RelayCommand(LoadContacts);
        LoadFavoritesCommand = new RelayCommand(LoadFavorites);
        _dbContext = dbContext;
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
        using ContactDbContext db = _dbContext.CreateDbContext();
        ContactsVM.LoadContacts(db.Contacts.ToList());
    }

    private void LoadFavorites()
    {
        using ContactDbContext db = _dbContext.CreateDbContext();
        IEnumerable<Person> favorites = db.Contacts.Where(c => c.IsFavorite).ToList();
        ContactsVM.LoadContacts(favorites);
    }
}

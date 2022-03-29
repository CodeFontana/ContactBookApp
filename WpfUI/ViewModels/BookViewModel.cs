using WpfUI.Services;
using WpfUI.Utilities;
using DataAccessLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using WpfUI.Models;
using Microsoft.EntityFrameworkCore;

namespace WpfUI.ViewModels;
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
        IEnumerable<PersonModel> contacts = db.Contacts
            .Include(x => x.PhoneNumbers)
            .Include(x => x.EmailAddresses)
            .Include(x => x.Addresses)
            .AsSplitQuery()
            .AsNoTracking()
            .Select(x => PersonModel.ToPersonModelMap(x)).ToList();
        ContactsVM.LoadContacts(contacts);
    }

    private void LoadFavorites()
    {
        using ContactDbContext db = _dbContext.CreateDbContext();
        IEnumerable<PersonModel> favorites = db.Contacts
            .Include(x => x.PhoneNumbers)
            .Include(x => x.EmailAddresses)
            .Include(x => x.Addresses)
            .AsSplitQuery()
            .Where(c => c.IsFavorite)
            .AsNoTracking()
            .Select(x => PersonModel.ToPersonModelMap(x)).ToList();
        ContactsVM.LoadContacts(favorites);
    }
}

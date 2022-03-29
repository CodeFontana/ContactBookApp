using ContactBook.Services;
using ContactBook.Utilities;
using DataAccessLibrary;
using DataAccessLibrary.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ContactBook.ViewModels;

public class ContactsViewModel : ObservableObject
{
    private readonly ContactDbContextFactory _dbContext;
    private readonly IDialogService _dialogService;

    public ContactsViewModel(ContactDbContextFactory dbContext,
                             IDialogService dialogService)
    {
        _dbContext = dbContext;
        _dialogService = dialogService;
        CreateContactCommand = new RelayCommand(CreateContact);
        EditContactCommand = new RelayCommand(EditContact, CanEdit);
        UpdateContactCommand = new RelayCommand(UpdateContact, IsEdit);
        UpdateContactImageCommand = new RelayCommand(UpdateContactImage, IsEdit);
        FavoriteContactCommand = new RelayCommand(FavoriteContact);
        DeleteContactCommand = new RelayCommand(DeleteContact, CanDelete);
    }

    private bool _isEditMode;
    public bool IsEditMode
    {
        get
        {
            return _isEditMode;
        }
        set
        {
            OnPropertyChanged(ref _isEditMode, value);
            OnPropertyChanged(nameof(IsDisplayMode));
        }
    }

    public bool IsDisplayMode
    {
        get { return !_isEditMode; }
    }

    public ObservableCollection<Person> Contacts { get; set; }

    private Person _selectedContact;

    public Person SelectedContact
    {
        get
        {
            return _selectedContact;
        }
        set
        {
            OnPropertyChanged(ref _selectedContact, value);
        }
    }

    public ICommand EditContactCommand { get; private set; }
    public ICommand UpdateContactCommand { get; private set; }
    public ICommand FavoriteContactCommand { get; private set; }
    public ICommand UpdateContactImageCommand { get; private set; }
    public ICommand CreateContactCommand { get; private set; }
    public ICommand DeleteContactCommand { get; private set; }

    public void LoadContacts(IEnumerable<Person> contacts)
    {
        Contacts = new ObservableCollection<Person>(contacts);
        OnPropertyChanged(nameof(Contacts));
    }

    private bool CanEdit()
    {
        if (SelectedContact == null)
        {
            return false;
        }

        return !IsEditMode;
    }

    private bool CanDelete()
    {
        return SelectedContact == null ? false : true;
    }

    private void EditContact()
    {
        IsEditMode = true;
    }

    private bool IsEdit()
    {
        return IsEditMode;
    }

    private void CreateContact()
    {
        Person newContact = new()
        {
            FirstName = "Firstname",
            LastName = "Lastname"
        };

        Contacts.Add(newContact);
        using ContactDbContext db = _dbContext.CreateDbContext();
        db.Add(newContact);
        db.SaveChanges();
        SelectedContact = newContact;
        OnPropertyChanged(nameof(SelectedContact));
    }

    private void UpdateContact()
    {
        Person contact = Contacts.FirstOrDefault(p => p.Id == SelectedContact.Id);

        if (contact != null)
        {
            using ContactDbContext db = _dbContext.CreateDbContext();
            db.Update(contact);
            db.SaveChanges();
            IsEditMode = false;
            OnPropertyChanged(nameof(SelectedContact));
        }
    }

    private void UpdateContactImage()
    {
        string filePath = _dialogService.OpenFile("Image files|*.bmp;*.jpg;*.jpeg;*.png;|All files");
        SelectedContact.ImagePath = filePath;

        Person contact = Contacts.FirstOrDefault(p => p.Id == SelectedContact.Id);

        if (contact != null)
        {
            using ContactDbContext db = _dbContext.CreateDbContext();
            db.Update(contact);
            db.SaveChanges();
            OnPropertyChanged(nameof(SelectedContact));
        }
    }

    private void FavoriteContact()
    {
        Person contact = Contacts.FirstOrDefault(p => p.Id == SelectedContact.Id);

        if (contact != null)
        {
            using ContactDbContext db = _dbContext.CreateDbContext();
            db.Update(contact);
            db.SaveChanges();
            OnPropertyChanged(nameof(SelectedContact));
        }
    }

    private void DeleteContact()
    {
        if (SelectedContact != null)
        {
            using ContactDbContext db = _dbContext.CreateDbContext();
            db.Remove(SelectedContact);
            db.SaveChanges();
            Contacts.Remove(SelectedContact);
            SelectedContact = null;
            OnPropertyChanged(nameof(SelectedContact));
        }
    }
}

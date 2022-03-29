using WpfUI.Services;
using WpfUI.Utilities;
using DataAccessLibrary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WpfUI.Models;
using DataAccessLibrary.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace WpfUI.ViewModels;

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
        AddPhoneNumber = new RelayCommand(AddContactPhone, IsEdit);
        AddEmailAddress = new RelayCommand(AddContactEmail, IsEdit);
        AddPhysicalAddress = new RelayCommand(AddContactAddress, IsEdit);
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

    public ObservableCollection<PersonModel> Contacts { get; set; }

    private PersonModel _selectedContact;

    public PersonModel SelectedContact
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
    public ICommand AddPhoneNumber { get; private set; }
    public ICommand AddEmailAddress { get; private set; }
    public ICommand AddPhysicalAddress { get; private set; }
    public ICommand UpdateContactCommand { get; private set; }
    public ICommand FavoriteContactCommand { get; private set; }
    public ICommand UpdateContactImageCommand { get; private set; }
    public ICommand CreateContactCommand { get; private set; }
    public ICommand DeleteContactCommand { get; private set; }

    public void LoadContacts(IEnumerable<PersonModel> contacts)
    {
        Contacts = new ObservableCollection<PersonModel>(contacts);
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

        using ContactDbContext db = _dbContext.CreateDbContext();
        EntityEntry<Person> person = db.Contacts.Add(newContact);
        db.SaveChanges();

        PersonModel p = PersonModel.ToPersonModelMap(newContact);
        Contacts.Add(p);
        SelectedContact = p;
    }

    private void UpdateContact()
    {
        using ContactDbContext db = _dbContext.CreateDbContext();
        Person person = db.Contacts.FirstOrDefault(x => x.Id == SelectedContact.Id);

        if (person != null)
        {
            person.FirstName = SelectedContact.FirstName;
            person.LastName = SelectedContact.LastName;
            person.Addresses = SelectedContact.Addresses;
            person.PhoneNumbers = SelectedContact.PhoneNumbers;
            person.EmailAddresses = SelectedContact.EmailAddresses;
            person.ImagePath = SelectedContact.ImagePath;
            person.IsFavorite = SelectedContact.IsFavorite;
            db.SaveChanges();
        }

        IsEditMode = false;
        OnPropertyChanged(nameof(SelectedContact));
    }
    private void AddContactPhone()
    {
        SelectedContact.PhoneNumbers.Add(new Phone());
    }
    private void AddContactEmail()
    {
        SelectedContact.EmailAddresses.Add(new Email());
    }

    private void AddContactAddress()
    {
        SelectedContact.Addresses.Add(new Address());
    }

    private void UpdateContactImage()
    {
        string filePath = _dialogService.OpenFile("Image files|*.bmp;*.jpg;*.jpeg;*.png;|All files");
        SelectedContact.ImagePath = filePath;

        using ContactDbContext db = _dbContext.CreateDbContext();
        Person person = db.Contacts.FirstOrDefault(x => x.Id == SelectedContact.Id);

        if (person != null)
        {
            person.ImagePath = SelectedContact.ImagePath;
            db.SaveChanges();
        }

        OnPropertyChanged(nameof(SelectedContact));
    }

    private void FavoriteContact()
    {
        using ContactDbContext db = _dbContext.CreateDbContext();
        Person person = db.Contacts.FirstOrDefault(x => x.Id == SelectedContact.Id);

        if (person != null)
        {
            person.IsFavorite = SelectedContact.IsFavorite;
            db.SaveChanges();
        }

        OnPropertyChanged(nameof(SelectedContact));
    }

    private void DeleteContact()
    {
        using ContactDbContext db = _dbContext.CreateDbContext();
        Person person = db.Contacts.FirstOrDefault(x => x.Id == SelectedContact.Id);

        if (person != null)
        {
            db.Contacts.Remove(person);
            db.SaveChanges();
        }

        Contacts.Remove(SelectedContact);
        SelectedContact = null;
        IsEditMode = false;
    }
}

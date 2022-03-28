using ContactBook.Models;
using ContactBook.Services;
using ContactBook.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ContactBook.ViewModels;

public class ContactsViewModel : ObservableObject
{
    private readonly IContactDataService _dataService;
    private readonly IDialogService _dialogService;

    public ContactsViewModel(IContactDataService dataService,
                             IDialogService dialogService)
    {
        _dataService = dataService;
        _dialogService = dialogService;
        EditCommand = new RelayCommand(Edit, CanEdit);
        SaveCommand = new RelayCommand(Save, IsEdit);
        UpdateCommand = new RelayCommand(Update);
        BrowseImageCommand = new RelayCommand(BrowseImage, IsEdit);
        AddContactCommand = new RelayCommand(AddContact);
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

    private Contact _selectedContact;

    public Contact SelectedContact
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

    public ObservableCollection<Contact> Contacts { get; private set; }

    public ICommand EditCommand { get; private set; }
    public ICommand SaveCommand { get; private set; }
    public ICommand UpdateCommand { get; private set; }
    public ICommand BrowseImageCommand { get; private set; }
    public ICommand AddContactCommand { get; private set; }
    public ICommand DeleteContactCommand { get; private set; }

    private bool CanEdit()
    {
        if (SelectedContact == null)
        {
            return false;
        }

        return !IsEditMode;
    }

    private void Edit()
    {
        IsEditMode = true;
    }

    private bool IsEdit()
    {
        return IsEditMode;
    }

    private void Save()
    {
        _dataService.Save(Contacts);
        IsEditMode = false;
        OnPropertyChanged(nameof(SelectedContact));
    }

    private void Update()
    {
        _dataService.Save(Contacts);
    }

    private void BrowseImage()
    {
        string filePath = _dialogService.OpenFile("Image files|*.bmp;*.jpg;*.jpeg;*.png;|All files");
        SelectedContact.ImagePath = filePath;
    }

    private void AddContact()
    {
        Contact newContact = new()
        {
            Name = "New Contact",
            PhoneNumbers = new string[4],
            Emails = new string[3],
            Locations = new string[3]
        };

        Contacts.Add(newContact);
        SelectedContact = newContact;
    }

    private bool CanDelete()
    {
        return SelectedContact == null ? false : true;
    }

    private void DeleteContact()
    {
        Contacts.Remove(SelectedContact);
        Save();
    }

    public void LoadContacts(IEnumerable<Contact> contacts)
    {
        Contacts = new ObservableCollection<Contact>(contacts);
        OnPropertyChanged(nameof(Contacts));
    }
}

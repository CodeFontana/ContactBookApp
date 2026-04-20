using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DataAccessLibrary;
using DataAccessLibrary.Entities;
using DataAccessLibrary.Models;
using Microsoft.EntityFrameworkCore;
using WpfUI.Helpers;
using WpfUI.Services;
using WpfUI.Utilities;

namespace WpfUI.ViewModels;

public class ContactsViewModel : ObservableObject
{
    private readonly ContactDbContextFactory _dbContext;
    private readonly IDialogService _dialogService;

    // Pre-edit snapshot for an existing contact, used to revert on Cancel.
    // Null when not editing or when the current edit target is a brand-new draft.
    private PersonModel? _editSnapshot;

    // True when SelectedContact has never been persisted (Id == 0 and only lives in the UI list).
    private bool _selectedIsDraft;

    public ContactsViewModel(ContactDbContextFactory dbContext,
                             IDialogService dialogService)
    {
        _dbContext = dbContext;
        _dialogService = dialogService;
        CreateContactCommand = new RelayCommand(CreateContact, CanCreate);
        EditContactCommand = new RelayCommand(EditContact, CanEdit);
        CancelEditCommand = new RelayCommand(CancelEdit, IsEdit);
        AddPhoneNumberCommand = new RelayCommand(AddContactPhone, IsEdit);
        AddEmailAddressCommand = new RelayCommand(AddContactEmail, IsEdit);
        AddPhysicalAddressCommand = new RelayCommand(AddContactAddress, IsEdit);
        RemovePhoneNumberCommand = new RelayCommand<PhoneModel>(RemoveContactPhone);
        RemoveEmailAddressCommand = new RelayCommand<EmailModel>(RemoveContactEmail);
        RemovePhysicalAddressCommand = new RelayCommand<AddressModel>(RemoveContactAddress);
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

    public ObservableCollection<PersonModel> Contacts { get; } = [];

    private PersonModel? _selectedContact;

    public PersonModel? SelectedContact
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
    public ICommand CancelEditCommand { get; private set; }
    public ICommand AddPhoneNumberCommand { get; private set; }
    public ICommand AddEmailAddressCommand { get; private set; }
    public ICommand AddPhysicalAddressCommand { get; private set; }
    public ICommand RemovePhoneNumberCommand { get; private set; }
    public ICommand RemoveEmailAddressCommand { get; private set; }
    public ICommand RemovePhysicalAddressCommand { get; private set; }
    public ICommand UpdateContactCommand { get; private set; }
    public ICommand FavoriteContactCommand { get; private set; }
    public ICommand UpdateContactImageCommand { get; private set; }
    public ICommand CreateContactCommand { get; private set; }
    public ICommand DeleteContactCommand { get; private set; }

    public void LoadContacts(IEnumerable<PersonModel> contacts)
    {
        // A list reload while editing would orphan the snapshot/draft state — bail out cleanly.
        if (IsEditMode)
        {
            CancelEdit();
        }

        Contacts.Clear();

        foreach (PersonModel contact in contacts)
        {
            Contacts.Add(contact);
        }
    }

    private bool CanCreate()
    {
        return !IsEditMode;
    }

    private bool CanEdit()
    {
        if (SelectedContact is null)
        {
            return false;
        }

        return !IsEditMode;
    }

    private bool CanDelete()
    {
        return SelectedContact is not null;
    }

    private bool IsEdit()
    {
        return IsEditMode;
    }

    private void EditContact()
    {
        if (SelectedContact is null)
        {
            return;
        }

        _editSnapshot = CloneContact(SelectedContact);
        _selectedIsDraft = false;
        IsEditMode = true;
    }

    private void CreateContact()
    {
        // No DB write here — the contact stays in-memory until the user clicks Save.
        // This avoids the old behavior of dropping a "Firstname Lastname" placeholder row
        // into the database the moment the + button was clicked.
        PersonModel draft = new();
        Contacts.Add(draft);
        SelectedContact = draft;
        _selectedIsDraft = true;
        _editSnapshot = null;
        IsEditMode = true;
    }

    private void CancelEdit()
    {
        if (!IsEditMode)
        {
            return;
        }

        bool wasDraft = _selectedIsDraft;
        PersonModel? snapshot = _editSnapshot;
        PersonModel? selected = SelectedContact;

        // Clear edit state BEFORE manipulating the list; otherwise the ListView's selection
        // change (when we remove the draft) would re-enter this method via command requery.
        _editSnapshot = null;
        _selectedIsDraft = false;
        IsEditMode = false;

        if (wasDraft && selected is not null)
        {
            // The draft was never persisted — just drop it from the UI list.
            Contacts.Remove(selected);
        }
        else if (snapshot is not null && selected is not null)
        {
            // Restore values in place so existing bindings keep working.
            RestoreFromSnapshot(selected, snapshot);
        }
    }

    private void UpdateContact()
    {
        if (SelectedContact is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedContact.FirstName)
            && string.IsNullOrWhiteSpace(SelectedContact.LastName))
        {
            _dialogService.ShowMessageBox("Please enter a first or last name before saving.");
            return;
        }

        using ContactDbContext db = _dbContext.CreateDbContext();

        if (_selectedIsDraft)
        {
            InsertNewContact(db, SelectedContact);
        }
        else
        {
            bool stillExists = UpdateExistingContact(db, SelectedContact);

            if (!stillExists)
            {
                _dialogService.ShowMessageBox("This contact no longer exists in the database.");
                Contacts.Remove(SelectedContact);
                SelectedContact = null;
                _editSnapshot = null;
                _selectedIsDraft = false;
                IsEditMode = false;
                return;
            }
        }

        _editSnapshot = null;
        _selectedIsDraft = false;
        IsEditMode = false;
        OnPropertyChanged(nameof(SelectedContact));
    }

    private static void InsertNewContact(ContactDbContext db, PersonModel model)
    {
        Person entity = new()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            ImagePath = model.ImagePath,
            IsFavorite = model.IsFavorite,
        };

        List<(PhoneModel Model, Phone Entity)> phonePairs = [];
        foreach (PhoneModel pm in model.PhoneNumbers)
        {
            Phone newPhone = new() { PhoneNumber = pm.PhoneNumber };
            entity.PhoneNumbers.Add(newPhone);
            phonePairs.Add((pm, newPhone));
        }

        List<(EmailModel Model, Email Entity)> emailPairs = [];
        foreach (EmailModel em in model.EmailAddresses)
        {
            Email newEmail = new() { EmailAddress = em.EmailAddress };
            entity.EmailAddresses.Add(newEmail);
            emailPairs.Add((em, newEmail));
        }

        List<(AddressModel Model, Address Entity)> addressPairs = [];
        foreach (AddressModel am in model.Addresses)
        {
            Address newAddress = new()
            {
                StreetAddress = am.StreetAddress,
                City = am.City,
                State = am.State,
                ZipCode = am.ZipCode,
            };
            entity.Addresses.Add(newAddress);
            addressPairs.Add((am, newAddress));
        }

        db.Contacts.Add(entity);
        db.SaveChanges();

        // Surface generated identity values to the UI model so subsequent edits/deletes
        // target the correct rows.
        model.Id = entity.Id;
        ApplyGeneratedIds(phonePairs, (m, id, pid) => { m.Id = id; m.PersonId = pid; }, e => (e.Id, e.PersonId));
        ApplyGeneratedIds(emailPairs, (m, id, pid) => { m.Id = id; m.PersonId = pid; }, e => (e.Id, e.PersonId));
        ApplyGeneratedIds(addressPairs, (m, id, pid) => { m.Id = id; m.PersonId = pid; }, e => (e.Id, e.PersonId));
    }

    private static bool UpdateExistingContact(ContactDbContext db, PersonModel model)
    {
        Person? person = db.Contacts
            .Include(x => x.PhoneNumbers)
            .Include(x => x.EmailAddresses)
            .Include(x => x.Addresses)
            .AsSplitQuery()
            .FirstOrDefault(x => x.Id == model.Id);

        if (person is null)
        {
            return false;
        }

        person.FirstName = model.FirstName;
        person.LastName = model.LastName;
        person.ImagePath = model.ImagePath;
        person.IsFavorite = model.IsFavorite;

        // Diff each child collection: add new (Id == 0), remove missing, update existing.
        // The previous code reassigned the navigation property, which EF treats as "delete
        // every old row, insert every new row" — losing identity and shuffling PKs.
        List<(PhoneModel Model, Phone Entity)> newPhones = SyncPhones(person.PhoneNumbers, model.PhoneNumbers, person.Id);
        List<(EmailModel Model, Email Entity)> newEmails = SyncEmails(person.EmailAddresses, model.EmailAddresses, person.Id);
        List<(AddressModel Model, Address Entity)> newAddresses = SyncAddresses(person.Addresses, model.Addresses, person.Id);

        db.SaveChanges();

        ApplyGeneratedIds(newPhones, (m, id, pid) => { m.Id = id; m.PersonId = pid; }, e => (e.Id, e.PersonId));
        ApplyGeneratedIds(newEmails, (m, id, pid) => { m.Id = id; m.PersonId = pid; }, e => (e.Id, e.PersonId));
        ApplyGeneratedIds(newAddresses, (m, id, pid) => { m.Id = id; m.PersonId = pid; }, e => (e.Id, e.PersonId));

        return true;
    }

    private static List<(PhoneModel Model, Phone Entity)> SyncPhones(
        ICollection<Phone> entities, ICollection<PhoneModel> models, int personId)
    {
        List<(PhoneModel, Phone)> created = [];
        HashSet<int> keepIds = models.Where(m => m.Id != 0).Select(m => m.Id).ToHashSet();
        foreach (Phone removed in entities.Where(e => !keepIds.Contains(e.Id)).ToList())
        {
            entities.Remove(removed);
        }
        foreach (PhoneModel m in models)
        {
            if (m.Id == 0)
            {
                Phone newPhone = new() { PersonId = personId, PhoneNumber = m.PhoneNumber };
                entities.Add(newPhone);
                created.Add((m, newPhone));
            }
            else
            {
                Phone? existing = entities.FirstOrDefault(e => e.Id == m.Id);
                if (existing is not null)
                {
                    existing.PhoneNumber = m.PhoneNumber;
                }
            }
        }
        return created;
    }

    private static List<(EmailModel Model, Email Entity)> SyncEmails(
        ICollection<Email> entities, ICollection<EmailModel> models, int personId)
    {
        List<(EmailModel, Email)> created = [];
        HashSet<int> keepIds = models.Where(m => m.Id != 0).Select(m => m.Id).ToHashSet();
        foreach (Email removed in entities.Where(e => !keepIds.Contains(e.Id)).ToList())
        {
            entities.Remove(removed);
        }
        foreach (EmailModel m in models)
        {
            if (m.Id == 0)
            {
                Email newEmail = new() { PersonId = personId, EmailAddress = m.EmailAddress };
                entities.Add(newEmail);
                created.Add((m, newEmail));
            }
            else
            {
                Email? existing = entities.FirstOrDefault(e => e.Id == m.Id);
                if (existing is not null)
                {
                    existing.EmailAddress = m.EmailAddress;
                }
            }
        }
        return created;
    }

    private static List<(AddressModel Model, Address Entity)> SyncAddresses(
        ICollection<Address> entities, ICollection<AddressModel> models, int personId)
    {
        List<(AddressModel, Address)> created = [];
        HashSet<int> keepIds = models.Where(m => m.Id != 0).Select(m => m.Id).ToHashSet();
        foreach (Address removed in entities.Where(e => !keepIds.Contains(e.Id)).ToList())
        {
            entities.Remove(removed);
        }
        foreach (AddressModel m in models)
        {
            if (m.Id == 0)
            {
                Address newAddress = new()
                {
                    PersonId = personId,
                    StreetAddress = m.StreetAddress,
                    City = m.City,
                    State = m.State,
                    ZipCode = m.ZipCode,
                };
                entities.Add(newAddress);
                created.Add((m, newAddress));
            }
            else
            {
                Address? existing = entities.FirstOrDefault(e => e.Id == m.Id);
                if (existing is not null)
                {
                    existing.StreetAddress = m.StreetAddress;
                    existing.City = m.City;
                    existing.State = m.State;
                    existing.ZipCode = m.ZipCode;
                }
            }
        }
        return created;
    }

    private static void ApplyGeneratedIds<TModel, TEntity>(
        IEnumerable<(TModel Model, TEntity Entity)> pairs,
        System.Action<TModel, int, int> setIds,
        System.Func<TEntity, (int Id, int PersonId)> getIds)
    {
        foreach ((TModel m, TEntity e) in pairs)
        {
            (int id, int personId) = getIds(e);
            setIds(m, id, personId);
        }
    }

    private void AddContactPhone()
    {
        SelectedContact?.PhoneNumbers.Add(new PhoneModel());
    }

    private void AddContactEmail()
    {
        SelectedContact?.EmailAddresses.Add(new EmailModel());
    }

    private void AddContactAddress()
    {
        SelectedContact?.Addresses.Add(new AddressModel());
    }

    private void RemoveContactPhone(PhoneModel phone)
    {
        // Remove by reference, not by Id. Unsaved rows all share Id == 0, so removing
        // by Id would non-deterministically pick one (and could remove the wrong one).
        SelectedContact?.PhoneNumbers.Remove(phone);
    }

    private void RemoveContactEmail(EmailModel email)
    {
        SelectedContact?.EmailAddresses.Remove(email);
    }

    private void RemoveContactAddress(AddressModel address)
    {
        SelectedContact?.Addresses.Remove(address);
    }

    private void UpdateContactImage()
    {
        if (SelectedContact is null)
        {
            return;
        }

        string? filePath = _dialogService.OpenFile(
            "Image files (*.bmp;*.jpg;*.jpeg;*.png;*.heic;*.heif)|*.bmp;*.jpg;*.jpeg;*.png;*.heic;*.heif|All files (*.*)|*.*");

        if (filePath is null)
        {
            return;
        }

        // HEIC/HEIF requires the Microsoft HEIF Image Extensions (and HEVC Video Extensions for the
        // actual H.265 decode). If those aren't installed, or the file is corrupt, TryLoad returns null.
        if (ImagePathToBitmapSourceConverter.TryLoad(filePath) is null)
        {
            _dialogService.ShowMessageBox(
                $"The selected image could not be loaded:\n\n{filePath}\n\n" +
                "If this is a HEIC or HEIF file, install the \"HEIF Image Extensions\" " +
                "(and \"HEVC Video Extensions\") from the Microsoft Store and try again.");
            return;
        }

        // Set the path on the UI model only — the change will be persisted by Save (UpdateContact)
        // along with everything else, so Cancel can revert it cleanly.
        SelectedContact.ImagePath = filePath;
    }

    private void FavoriteContact()
    {
        if (SelectedContact is null)
        {
            return;
        }

        // Favoriting is a one-click action available outside edit mode, so it persists immediately.
        // For an unsaved draft (Id == 0) there's nothing to update yet — the favorite flag will be
        // written when the draft is saved.
        if (SelectedContact.Id == 0)
        {
            return;
        }

        using ContactDbContext db = _dbContext.CreateDbContext();
        Person? person = db.Contacts.FirstOrDefault(x => x.Id == SelectedContact.Id);

        if (person is not null)
        {
            person.IsFavorite = SelectedContact.IsFavorite;
            db.SaveChanges();
        }

        OnPropertyChanged(nameof(SelectedContact));
    }

    private void DeleteContact()
    {
        if (SelectedContact is null)
        {
            return;
        }

        // For an unsaved draft (Id == 0) there's no DB row — just drop it from the UI list.
        // The previous code did `FirstOrDefault(x => x.Id == 0)` which would silently delete
        // some other unsaved row, or no row at all.
        if (SelectedContact.Id != 0)
        {
            using ContactDbContext db = _dbContext.CreateDbContext();
            Person? person = db.Contacts.FirstOrDefault(x => x.Id == SelectedContact.Id);

            if (person is not null)
            {
                db.Contacts.Remove(person);
                db.SaveChanges();
            }
        }

        Contacts.Remove(SelectedContact);
        SelectedContact = null;
        _editSnapshot = null;
        _selectedIsDraft = false;
        IsEditMode = false;
    }

    private static PersonModel CloneContact(PersonModel source)
    {
        PersonModel clone = new()
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            ImagePath = source.ImagePath,
            IsFavorite = source.IsFavorite,
        };

        foreach (PhoneModel p in source.PhoneNumbers)
        {
            clone.PhoneNumbers.Add(new PhoneModel { Id = p.Id, PersonId = p.PersonId, PhoneNumber = p.PhoneNumber });
        }
        foreach (EmailModel e in source.EmailAddresses)
        {
            clone.EmailAddresses.Add(new EmailModel { Id = e.Id, PersonId = e.PersonId, EmailAddress = e.EmailAddress });
        }
        foreach (AddressModel a in source.Addresses)
        {
            clone.Addresses.Add(new AddressModel
            {
                Id = a.Id,
                PersonId = a.PersonId,
                StreetAddress = a.StreetAddress,
                City = a.City,
                State = a.State,
                ZipCode = a.ZipCode,
            });
        }

        return clone;
    }

    private static void RestoreFromSnapshot(PersonModel target, PersonModel snapshot)
    {
        target.FirstName = snapshot.FirstName;
        target.LastName = snapshot.LastName;
        target.ImagePath = snapshot.ImagePath;
        target.IsFavorite = snapshot.IsFavorite;

        // Mutate the existing ObservableCollections so the UI keeps the same instance bound.
        target.PhoneNumbers.Clear();
        foreach (PhoneModel p in snapshot.PhoneNumbers)
        {
            target.PhoneNumbers.Add(p);
        }
        target.EmailAddresses.Clear();
        foreach (EmailModel e in snapshot.EmailAddresses)
        {
            target.EmailAddresses.Add(e);
        }
        target.Addresses.Clear();
        foreach (AddressModel a in snapshot.Addresses)
        {
            target.Addresses.Add(a);
        }
    }
}

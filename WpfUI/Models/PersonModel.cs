using WpfUI.Utilities;
using DataAccessLibrary.Entities;
using System.Collections.ObjectModel;

namespace WpfUI.Models;

public class PersonModel : ObservableObject
{
    public PersonModel()
    {
        _firstName = "";
        _lastName = "";
        _addresses = new ObservableCollection<Address>();
        _phoneNumbers = new ObservableCollection<Phone>();
        _emailAddresses = new ObservableCollection<Email>();
        _imagePath = null;
        _isFavorite = false;
    }

    public int Id { get; set; }

    private string _firstName;
    public string FirstName
    {
        get
        {
            return _firstName;
        }
        set
        {
            OnPropertyChanged(ref _firstName, value);
            OnPropertyChanged(nameof(FullName));
        }
    }

    private string _lastName;
    public string LastName
    {
        get
        {
            return _lastName;
        }
        set
        {
            OnPropertyChanged(ref _lastName, value);
            OnPropertyChanged(nameof(FullName));
        }
    }

    public string FullName 
    { 
        get => $"{FirstName} {LastName}"; 
    }

    private ObservableCollection<Address> _addresses;
    public ObservableCollection<Address> Addresses
    {
        get
        {
            return _addresses;
        }
        set
        {
            OnPropertyChanged(ref _addresses, value);
        }
    }

    private ObservableCollection<Phone> _phoneNumbers;
    public ObservableCollection<Phone> PhoneNumbers
    {
        get
        {
            return _phoneNumbers;
        }
        set
        {
            OnPropertyChanged(ref _phoneNumbers, value);
        }
    }

    private ObservableCollection<Email> _emailAddresses;

    public ObservableCollection<Email> EmailAddresses
    {
        get { return _emailAddresses; }
        set { _emailAddresses = value; }
    }

    private string _imagePath;
    public string ImagePath
    {
        get
        {
            return _imagePath;
        }
        set
        {
            OnPropertyChanged(ref _imagePath, value);
        }
    }

    private bool _isFavorite;
    public bool IsFavorite
    {
        get
        {
            return _isFavorite;
        }
        set
        {
            OnPropertyChanged(ref _isFavorite, value);
        }
    }

    public static PersonModel ToPersonModelMap(Person person)
    {
        return new PersonModel()
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            Addresses = person.Addresses,
            PhoneNumbers = person.PhoneNumbers,
            EmailAddresses = person.EmailAddresses,
            ImagePath = person.ImagePath,
            IsFavorite = person.IsFavorite
        };
    }
}

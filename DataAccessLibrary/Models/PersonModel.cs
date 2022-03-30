using DataAccessLibrary.Entities;
using System.Collections.ObjectModel;
using System.Linq;

namespace DataAccessLibrary.Models;

public class PersonModel : ObservableObject
{
    public PersonModel()
    {
        _firstName = "";
        _lastName = "";
        _addresses = new ObservableCollection<AddressModel>();
        _phoneNumbers = new ObservableCollection<PhoneModel>();
        _emailAddresses = new ObservableCollection<EmailModel>();
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

    private ObservableCollection<AddressModel> _addresses;
    public ObservableCollection<AddressModel> Addresses
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

    private ObservableCollection<PhoneModel> _phoneNumbers;
    public ObservableCollection<PhoneModel> PhoneNumbers
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

    private ObservableCollection<EmailModel> _emailAddresses;

    public ObservableCollection<EmailModel> EmailAddresses
    {
        get 
        { 
            return _emailAddresses; 
        }
        set 
        {
            OnPropertyChanged(ref _emailAddresses, value);
        }
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
            Addresses = new ObservableCollection<AddressModel>(person.Addresses.ToList().Select(x => AddressModel.ToAddressModelMap(x))),
            PhoneNumbers = new ObservableCollection<PhoneModel>(person.PhoneNumbers.ToList().Select(x => PhoneModel.ToPhoneModelMap(x))),
            EmailAddresses = new ObservableCollection<EmailModel>(person.EmailAddresses.ToList().Select(x => EmailModel.ToEmailModelMap(x))),
            ImagePath = person.ImagePath,
            IsFavorite = person.IsFavorite
        };
    }
}

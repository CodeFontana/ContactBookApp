using DataAccessLibrary.Entities;

namespace DataAccessLibrary.Models;

public class AddressModel : ObservableObject
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    private string _streetAddress;
    public string StreetAddress
    {
        get
        {
            return _streetAddress;
        }

        set
        {
            OnPropertyChanged(ref _streetAddress, value);
            OnPropertyChanged(nameof(FullAddress));
        }
    }

    private string _city;
    public string City
    {
        get
        {
            return _city;
        }

        set
        {
            OnPropertyChanged(ref _city, value);
            OnPropertyChanged(nameof(FullAddress));
        }
    }

    private string _state;
    public string State
    {
        get
        {
            return _state;
        }

        set
        {
            OnPropertyChanged(ref _state, value);
            OnPropertyChanged(nameof(FullAddress));
        }
    }

    private string _zipCode;
    public string ZipCode
    {
        get
        {
            return _zipCode;
        }

        set
        {
            OnPropertyChanged(ref _zipCode, value);
            OnPropertyChanged(nameof(FullAddress));
        }
    }

    public string FullAddress { get => $"{StreetAddress} {City}, {State}  {ZipCode}"; }

    public static AddressModel ToAddressModelMap(Address address)
    {
        return new AddressModel()
        {
            Id = address.Id,
            PersonId = address.PersonId,
            StreetAddress = address.StreetAddress,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode
        };
    }

    public static Address ToAddressMap(AddressModel address)
    {
        return new Address()
        {
            Id = address.Id,
            PersonId = address.PersonId,
            StreetAddress = address.StreetAddress,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode
        };
    }
}

using System.ComponentModel.DataAnnotations;
using DataAccessLibrary.Entities;

namespace DataAccessLibrary.Models;

public class AddressModel : ObservableObject
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    private ContactType _type = ContactType.Home;
    public ContactType Type
    {
        get { return _type; }
        set { OnPropertyChanged(ref _type, value); }
    }

    private string? _streetAddress;

    [MaxLength(100, ErrorMessage = "Street address must be 100 characters or less")]
    public string? StreetAddress
    {
        get { return _streetAddress; }
        set
        {
            OnPropertyChanged(ref _streetAddress, value);
            OnPropertyChanged(nameof(FullAddress));
        }
    }

    private string? _city;

    [MaxLength(50, ErrorMessage = "City must be 50 characters or less")]
    public string? City
    {
        get { return _city; }
        set
        {
            OnPropertyChanged(ref _city, value);
            OnPropertyChanged(nameof(FullAddress));
        }
    }

    private string? _state;

    [MaxLength(50, ErrorMessage = "State must be 50 characters or less")]
    public string? State
    {
        get { return _state; }
        set
        {
            OnPropertyChanged(ref _state, value);
            OnPropertyChanged(nameof(FullAddress));
        }
    }

    private string? _zipCode;

    [MaxLength(10, ErrorMessage = "Zip code must be 10 characters or less")]
    public string? ZipCode
    {
        get { return _zipCode; }
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
            Type = address.Type,
            StreetAddress = address.StreetAddress,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
        };
    }

    public static Address ToAddressMap(AddressModel address)
    {
        return new Address()
        {
            Id = address.Id,
            PersonId = address.PersonId,
            Type = address.Type,
            StreetAddress = address.StreetAddress,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
        };
    }
}

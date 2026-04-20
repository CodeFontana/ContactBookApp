using System.ComponentModel.DataAnnotations;
using DataAccessLibrary.Entities;

namespace DataAccessLibrary.Models;

public class PhoneModel : ObservableObject
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    private ContactType _type = ContactType.Mobile;
    public ContactType Type
    {
        get { return _type; }
        set { OnPropertyChanged(ref _type, value); }
    }

    private string? _phoneNumber;

    [Required(ErrorMessage = "Phone number is required")]
    [MaxLength(20, ErrorMessage = "Phone number must be 20 characters or less")]
    public string? PhoneNumber
    {
        get { return _phoneNumber; }
        set { OnPropertyChanged(ref _phoneNumber, value); }
    }

    public static PhoneModel ToPhoneModelMap(Phone phone)
    {
        return new PhoneModel()
        {
            Id = phone.Id,
            PersonId = phone.PersonId,
            Type = phone.Type,
            PhoneNumber = phone.PhoneNumber,
        };
    }

    public static Phone ToPhoneMap(PhoneModel phone)
    {
        return new Phone()
        {
            Id = phone.Id,
            PersonId = phone.PersonId,
            Type = phone.Type,
            PhoneNumber = phone.PhoneNumber,
        };
    }
}

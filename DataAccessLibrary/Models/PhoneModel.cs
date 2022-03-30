using DataAccessLibrary.Entities;

namespace DataAccessLibrary.Models;

public class PhoneModel : ObservableObject
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    private string _phoneNumber;
    public string PhoneNumber
    {
        get
        {
            return _phoneNumber;
        }

        set
        {
            OnPropertyChanged(ref _phoneNumber, value);
        }
    }

    public static PhoneModel ToPhoneModelMap(Phone phone)
    {
        return new PhoneModel()
        {
            Id = phone.Id,
            PersonId = phone.PersonId,
            PhoneNumber = phone.PhoneNumber
        };
    }
}

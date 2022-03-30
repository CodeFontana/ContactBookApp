using DataAccessLibrary.Entities;

namespace DataAccessLibrary.Models;

public class EmailModel : ObservableObject
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    private string _emailAddress;
    public string EmailAddress
    {
        get
        {
            return _emailAddress;
        }

        set
        {
            OnPropertyChanged(ref _emailAddress, value);
        }
    }

    public static EmailModel ToEmailModelMap(Email email)
    {
        return new EmailModel()
        {
            Id = email.Id,
            PersonId = email.PersonId,
            EmailAddress = email.EmailAddress
        };
    }

    public static Email ToEmailMap(EmailModel email)
    {
        return new Email()
        {
            Id = email.Id,
            PersonId = email.PersonId,
            EmailAddress = email.EmailAddress
        };
    }
}

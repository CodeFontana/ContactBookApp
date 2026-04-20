using System.ComponentModel.DataAnnotations;
using DataAccessLibrary.Entities;

namespace DataAccessLibrary.Models;

public class EmailModel : ObservableObject
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    private ContactType _type = ContactType.Home;
    public ContactType Type
    {
        get { return _type; }
        set { OnPropertyChanged(ref _type, value); }
    }

    private string? _emailAddress;

    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    [MaxLength(100, ErrorMessage = "Email address must be 100 characters or less")]
    public string? EmailAddress
    {
        get { return _emailAddress; }
        set { OnPropertyChanged(ref _emailAddress, value); }
    }

    public static EmailModel ToEmailModelMap(Email email)
    {
        return new EmailModel()
        {
            Id = email.Id,
            PersonId = email.PersonId,
            Type = email.Type,
            EmailAddress = email.EmailAddress,
        };
    }

    public static Email ToEmailMap(EmailModel email)
    {
        return new Email()
        {
            Id = email.Id,
            PersonId = email.PersonId,
            Type = email.Type,
            EmailAddress = email.EmailAddress,
        };
    }
}

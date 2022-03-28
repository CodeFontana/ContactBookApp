using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Entities;

public class Email
{
    public int Id { get; set; }

    [MaxLength(100, ErrorMessage = "Email address must be 100 characters or less")]
    public string EmailAddress { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLibrary.Entities;

public class Email
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int PersonId { get; set; }

    [MaxLength(100, ErrorMessage = "Email address must be 100 characters or less")]
    public string EmailAddress { get; set; }
}

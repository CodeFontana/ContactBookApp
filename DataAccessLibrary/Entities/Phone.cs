using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Entities;

public class Phone
{
    public int Id { get; set; }

    [MaxLength(20, ErrorMessage = "Phone number must be 20 characters or less")]
    public string PhoneNumber { get; set; }
}

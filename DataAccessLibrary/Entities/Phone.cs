using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLibrary.Entities;

public class Phone
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(20, ErrorMessage = "Phone number must be 20 characters or less")]
    public string PhoneNumber { get; set; }
}

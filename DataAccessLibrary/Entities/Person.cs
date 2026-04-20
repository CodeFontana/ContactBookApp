using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLibrary.Entities;

public class Person
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50, ErrorMessage = "First name must be 50 characters or less")]
    public string FirstName { get; set; } = "";

    [Required]
    [MaxLength(50, ErrorMessage = "Last name must be 50 characters or less")]
    public string LastName { get; set; } = "";

    [NotMapped]
    public string FullName { get => $"{FirstName} {LastName}"; }

    public ICollection<Address> Addresses { get; set; } = new List<Address>();

    public ICollection<Phone> PhoneNumbers { get; set; } = new List<Phone>();

    public ICollection<Email> EmailAddresses { get; set; } = new List<Email>();

    public string? ImagePath { get; set; }

    public bool IsFavorite { get; set; } = false;
}

using System.ComponentModel.DataAnnotations;

namespace DataAccessLibrary.Entities;

public class Address
{
    public int Id { get; set; }

    [MaxLength(100, ErrorMessage = "Street address must be 100 characters or less")]
    public string StreetAddress { get; set; }

    [MaxLength(50, ErrorMessage = "City must be 50 characters or less")]
    public string City { get; set; }

    [MaxLength(50, ErrorMessage = "State must be 50 characters or less")]
    public string State { get; set; }

    [MaxLength(50, ErrorMessage = "Zip code must be 10 characters or less")]
    public string ZipCode { get; set; }
}

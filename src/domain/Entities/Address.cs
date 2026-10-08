namespace domain.Entities;

public class Address
{
    public required string StreetAddress { get; set; }
    public required int PostCode { get; set; }
    public required string City { get; set; }
}

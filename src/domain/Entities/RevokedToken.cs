namespace domain.Entities;

public class RevokedToken
{
    public int Id { get; set; }
    public required string Jti { get; set; }
}

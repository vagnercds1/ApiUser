namespace ApiUserTest.Domain.Entities;

public class EntityUser : BaseEntity
{
    public string FullName { get; set; }
    
    public string Document { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }
}

using WebAutoTest.Validations;



namespace WebAutoTest.Models;

public class User
{
    public int Index { get; set; }
    [UserName]
    public string? Name { get; set; }
    //[Required]
    [Phone]
    public string? PhoneNumber { get; set; }
    //[Required]
    [Password]
    public string? Password { get; set; }
}

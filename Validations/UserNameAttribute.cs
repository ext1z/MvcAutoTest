using System.ComponentModel.DataAnnotations;

namespace WebAutoTest.Validations;

public class UserNameAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var _value = (string?)value;

        ErrorMessage = "Name is must be no empty.";

        return !string.IsNullOrEmpty((string)value);
    }
}

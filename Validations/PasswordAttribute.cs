using System.ComponentModel.DataAnnotations;

namespace WebAutoTest.Validations;

public class PasswordAttribute : ValidationAttribute
{
    private readonly int MinLenght = 6;

    public override bool IsValid(object? value)
    {
        var _value = (string?)value;
        if (string.IsNullOrEmpty(_value))
        {
            ErrorMessage = "Passwor is must be no empty.";
        }
        else if (_value.Length <= MinLenght)
        {
            ErrorMessage = $"Your password must be at less {MinLenght}";
        }

        return !string.IsNullOrEmpty(_value) && _value.Length <= MinLenght;

    }    
}

using System.ComponentModel.DataAnnotations;

namespace WebAutoTest.Validations;

public class PhoneAttribute : ValidationAttribute
{


    private readonly int PhoneMaxLength = 9;

    public override bool IsValid(object? value)
    {
        var _value = (string?)value;


        if (string.IsNullOrEmpty(_value))
        {
            ErrorMessage = "Phone Number is must be no empty.";
        }
        //else if ()

        
        //ErrorMessage = "Your Phone Number must be no more than 9";


        return !string.IsNullOrEmpty(_value) && _value.Length >= PhoneMaxLength; 
    }
}

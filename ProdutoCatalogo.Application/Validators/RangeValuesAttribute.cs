using System.ComponentModel.DataAnnotations;

namespace ProdutoCatalogo.Application.Validators;


public class RangeValuesAttribute : ValidationAttribute
{
    private readonly int[] _validValues;

    public RangeValuesAttribute(params int[] validValues)
    {
        _validValues = validValues;
    }

    public override bool IsValid(object value)
    {
        if (value == null)
            return true;

        return _validValues.Contains((int)value);
    }
}
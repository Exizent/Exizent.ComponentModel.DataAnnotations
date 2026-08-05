namespace Exizent.ComponentModel.DataAnnotations;

public class MaxDecimalPlacesAttribute : MaxDecimalPlacesBaseAttribute
{
    public MaxDecimalPlacesAttribute(int maxDecimalPlaces)
        : base(maxDecimalPlaces, $"The field {{0}} must have a max of {maxDecimalPlaces} decimal places.")
    {
    }

    protected override int EffectiveMaxDecimalPlaces => MaxDecimalPlaces;
}

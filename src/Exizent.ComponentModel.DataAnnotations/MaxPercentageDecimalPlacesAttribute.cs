namespace Exizent.ComponentModel.DataAnnotations;

// Expects the value to be a normalised percentage, e.g. 98.1234% is passed in as 0.981234.
public class MaxPercentageDecimalPlacesAttribute : MaxDecimalPlacesBaseAttribute
{
    public MaxPercentageDecimalPlacesAttribute(int maxDecimalPlaces)
        : base(maxDecimalPlaces,
            $"The field {{0}} must be passed as a decimal fraction with a maximum of {maxDecimalPlaces + 2} decimal places (a percentage with up to {maxDecimalPlaces} decimal places, e.g. 98.1234% as 0.981234).")
    {
    }

    protected override int EffectiveMaxDecimalPlaces => MaxDecimalPlaces + 2;
}

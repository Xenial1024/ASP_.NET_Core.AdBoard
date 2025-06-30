using System.ComponentModel.DataAnnotations;

namespace AdBoard.Attributes
{
    public class RangeIfNotNullAttribute(double minimum, double maximum) : ValidationAttribute
    {
        private readonly double _minimum = minimum;
        private readonly double _maximum = maximum;

        public override bool IsValid(object value)
        {
            if (value == null) return true;

            if (value is decimal decimalValue)
                return decimalValue >= (decimal)_minimum && decimalValue <= (decimal)_maximum;

            return false;
        }
    }
}
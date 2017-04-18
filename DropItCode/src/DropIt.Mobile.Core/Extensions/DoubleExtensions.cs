namespace DropIt.Mobile.Core.Extensions
{
    public static class DoubleExtensions
    {
        public static double? ToNullableNumber(this double val)
        {
            double? result = null;
            if (!double.IsNaN(val))
                result = val;

            return result;
        }
    }
}

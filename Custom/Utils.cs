namespace SofyTrender.Pages
{
    public static class Utils
    {
        public const int MinYear = 1920;
        public static int MaxYear => DateTime.Now.Year;

        public static (double min, double max) ConvertFromYears(int minYear, int maxYear)
        {
            float difference = MaxYear - MinYear;
            return (Math.Clamp((minYear - MinYear) / difference, 0f, 1f), Math.Clamp((maxYear - MinYear) / difference, 0f, 1f));
        }

        public static (int minYear, int maxYear) ConvertFromValues(double min, double max)
        {
            var difference = MaxYear - MinYear;
            return ((int)(MinYear + difference * min), (int)(MinYear + difference * max));
        }

        public static int ConvertFromValue(double value)
        {
            var difference = MaxYear - MinYear;
            return (int)(MinYear + difference * value);
        }

        public static float ConvertFromYear(int year)
        {
            float difference = MaxYear - MinYear;
            return Math.Clamp((year - MinYear) / difference, 0f, 1f);
        }

        public static T[] AddOrCreate<T>(T[] array, T newItem)
        {
            if (array == null)
            {
                return [newItem];
            }

            var list = new List<T>(array) { newItem };
            return list.ToArray();
        }

        public static T[] Remove<T>(T[] array, T item)
        {
            var list = array.ToList();
            list.Remove(item);
            return list.ToArray();
        }
    }
}

namespace RunaTiming.Shared.ClassConverters
{
    public static class Kondis2022ClassConverter
    {
        public static string Convert(bool isMale, DateTime birthDate)
        {
            var age = DateTime.Today.Year - birthDate.Year;
            var prefix = isMale ? "M" : "K";

            switch (age)
            {
                case >= 10 and <= 11:
                    return $"{prefix}10-11";
                case >= 12 and <= 13:
                    return $"{prefix}12-13";
                case >= 14 and <= 15:
                    return $"{prefix}14-15";
                case >= 16 and <= 17:
                    return $"{prefix}16-17";
                case >= 18 and <= 19:
                    return $"{prefix}18-19";
                case >= 20 and <= 22:
                    return $"{prefix}20-22";
                case >= 23 and <= 34:
                    return $"{prefix}23-34";
                case >= 35 and <= 39:
                    return $"{prefix}35-39";
            }

            if (age >= 40 && age <= 200)
            {
                var ageClassDifference = age % 5;
                var baseAge = age - ageClassDifference;
                return $"{prefix}{baseAge}-{baseAge + 4}";
            }

            return string.Empty;
        }
    }
}
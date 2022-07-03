using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using RunaTiming.Shared.Upload;

namespace RunaTiming.Csv
{
    public static class CsvTimingHelper
    {
        public static IReadOnlyList<CsvTimingFile> ParseFile(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(
                reader,
                new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound = null
                });
            csv.Context.RegisterClassMap<CsvTimingFileMap>();
            return csv.GetRecords<CsvTimingFile>().ToList();
        }

        public static ResultItem ConvertToResultItem(CsvTimingFile csvFile)
        {
            return new ResultItem
            {
                Bib = csvFile.Bib,
                RaceName = csvFile.RaceName,
                FirstName = csvFile.FirstName,
                LastName = csvFile.LastName,
                Sex = Enum.Parse<Db.Models.Sex>(csvFile.Sex.ToString()),
                BirthDate = csvFile.BirthDate,
                StartTime = csvFile.StartTime,
                //ChipStartTime = csvFile.ChipStartTime,
                FinishingTime = ParseSeconds(csvFile.FinishingTime),
                Splits = GetSplits(csvFile.Splits)
            };
        }

        private static List<double> GetSplits(string value)
        {
            var result = new List<double>();

            if (string.IsNullOrWhiteSpace(value))
            {
                return result;
            }

            var splits = value.Split("|");

            foreach (var split in splits)
            {
                var duration = ParseSeconds(split);

                if (duration == null)
                {
                    throw new InvalidOperationException($"Expected valid duration string: \"{split}\"");
                }

                result.Add(duration.Value);
            }

            var totalDuration = 0.0;

            // Normalize values so 1,1,2,1 becomes 1,2,4,5
            for (var durationIndex = 0; durationIndex < result.Count; durationIndex++)
            {
                var tempResultValue = result[durationIndex];
                result[durationIndex] = Math.Round(totalDuration + tempResultValue, 2);
                totalDuration += tempResultValue;
            }

            return result;
        }

        private static double? ParseSeconds(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            // Expects string format: 00:00:00.00
            var match = Regex.Match(value, @"([\d]{2}):([\d]{2}):([\d]{2}).([\d]{2})");
            var hours = double.Parse(match.Groups[1].Value);
            var minutes = double.Parse(match.Groups[2].Value);
            var seconds = double.Parse(match.Groups[3].Value);
            var milliseconds = double.Parse(match.Groups[4].Value);

            var totalSeconds =
                (hours * 60 * 60) +
                (minutes * 60) +
                seconds +
                (milliseconds / 100);

            return totalSeconds;
        }
    }
}
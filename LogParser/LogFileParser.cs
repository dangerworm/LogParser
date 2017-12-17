using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LogParser
{
    public class LogFileParser
    {
        private List<LogFileField> _fields;

        public event EventHandler FileLineProcessed;

        public LogFileParser()
        {
            _fields = new List<LogFileField>();
        }

        public List<LogFileField> GetFields(string fileName)
        {
            List<string> fieldNames;

            using (var input = new StreamReader(new FileStream(fileName, FileMode.Open)))
            {
                // Not used - ignore
                input.ReadLine(); // Software
                input.ReadLine(); // Version
                input.ReadLine(); // Date

                fieldNames = input.ReadLine()? // Fields
                    .Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }

            if (fieldNames == null || !fieldNames.Any())
            {
                return new List<LogFileField>();
            }

            fieldNames.RemoveAt(0); // Remove "#Fields" string from list

            _fields = fieldNames
                .Select((name, index) => new LogFileField
                {
                    FileName = fileName,
                    Index = index,
                    Name = name,
                    IsDateField = name.ToLower().Contains("date"),
                    IsTimeField = name.ToLower().Contains("time")
                }).ToList();

            return _fields;
        }

        public List<string[]> ProcessFile(string fileName, DateRange dateRange, IEnumerable<Constraint> constraints)
        {
            var output = new List<string[]>();
            var updateHandler = FileLineProcessed;

            using (var input = new StreamReader(new FileStream(fileName, FileMode.Open)))
            {
                // Useless
                input.ReadLine(); // Software
                input.ReadLine(); // Version
                input.ReadLine(); // Date
                input.ReadLine(); // Fields

                string line;
                while (!string.IsNullOrEmpty(line = input.ReadLine()))
                {
                    var row = line.Split(' ');
                    var fields = _fields.Where(f => f.FileName.Equals(fileName)).ToArray();

                    var dateField = fields.FirstOrDefault(f => f.IsDateField);
                    var timeField = fields.FirstOrDefault(f => f.IsTimeField);

                    DateTime? rowDate = null;
                    if (dateField != null)
                    {
                        rowDate = DateTime.Parse(row[dateField.Index]);

                        if (timeField != null)
                        {
                            rowDate += TimeSpan.Parse(row[timeField.Index]);

                        }
                    }

                    var inDateBand = true;
                    if (dateField != null && rowDate.HasValue)
                    {
                        inDateBand = rowDate.Value >= dateRange.GetMinDateTime() && 
                                     rowDate <= dateRange.GetMaxDateTime();
                    }

                    var isMatch = true;
                    foreach (var constraint in constraints)
                    {
                        isMatch = row[constraint.FieldIndex]
                            .ToLower()
                            .Contains(constraint.Value.ToLower());

                        if (!isMatch)
                        {
                            break;
                        }
                    }

                    if (inDateBand && isMatch)
                    {
                        output.Add(row.Select(field => field.Contains(",") ? $"\"{field}\"" : $"{field}").ToArray());
                    }

                    updateHandler?.Invoke(this, EventArgs.Empty);
                }
            }

            return output;
        }

        public static DateRange GetDateRange(string[] fileNames)
        {
            var dateRange = new DateRange();

            foreach (var fileName in fileNames)
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    continue;
                }

                var yymmdd = Path.GetFileName(fileName).Substring(4, 6);

                var logDate = DateTime.MinValue;
                var canParse = DateTime.TryParse(yymmdd, out logDate);
                if (!canParse)
                {
                    continue;
                }

                logDate = DateTime.ParseExact(yymmdd, "yyMMdd", CultureInfo.InvariantCulture);
                dateRange.UpdateDateRange(logDate);
            }

            if (!dateRange.IsInitialised())
            {
                dateRange = new DateRange(DateTime.Now.AddMonths(-1), DateTime.Today);
            }

            return dateRange.GetMidnightToMidnight();
        }
    }
}

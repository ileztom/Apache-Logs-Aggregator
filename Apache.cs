using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace ApacheLogs
{
    public static class Apache
    {
        public static List<LogEntry> Parse(string configPath)
        {
            var config = Config.LoadFromFile(configPath);
            if (config == null) return null;

            var logEntries = new List<LogEntry>();
            var logFiles = Directory.GetFiles(config.LogDirectory, config.LogFilePattern);

            foreach (var logFile in logFiles)
            {
                var lines = File.ReadAllLines(logFile);
                foreach (var line in lines)
                {
                    var logEntry = ParseLogEntry(line);
                    if (logEntry != null)
                    {
                        logEntries.Add(logEntry);
                    }
                }
            }

            return logEntries;
        }

        private static LogEntry ParseLogEntry(string line)
        {
            var regex = new Regex(@"^(?<ip>[\d\.]+) - - \[(?<date>[^\]]+)\] ""[^""]+"" (?<status>\d{3}) (?<size>\d+|-) ""[^""]*"" ""[^""]*""");
            var match = regex.Match(line);

            if (!match.Success) return null;

            return new LogEntry
            {
                IP = match.Groups["ip"].Value,
                Date = DateTime.ParseExact(match.Groups["date"].Value, "dd/MMM/yyyy:HH:mm:ss zzz", CultureInfo.InvariantCulture),
                Status = int.Parse(match.Groups["status"].Value),
                Size = match.Groups["size"].Value == "-" ? 0 : int.Parse(match.Groups["size"].Value)
            };
        }
    }
}

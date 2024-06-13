using System;
using System.IO;
using System.Linq;

namespace ApacheLogs
{
    public class Config
    {
        public string LogDirectory { get; set; }
        public string LogFilePattern { get; set; }
        public int MinuteOfUpdate { get; set; }
        public bool ShowCron { get; set; }

        public static Config LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                ConsoleHelper.WriteError($"Config file not found: {path}");
                return null;
            }

            var configLines = File.ReadAllLines(path)
                                  .Select(line => line.Split('='))
                                  .Where(parts => parts.Length == 2)
                                  .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());

            try
            {
                return new Config
                {
                    LogDirectory = configLines["LogDirectory"],
                    LogFilePattern = configLines["LogFilePattern"],
                    MinuteOfUpdate = int.Parse(configLines["MinuteOfUpdate"]),
                    ShowCron = bool.Parse(configLines["ShowCron"])
                };
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Error parsing config file: {ex.Message}");
                return null;
            }
        }
    }
}

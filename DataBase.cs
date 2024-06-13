using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;
using Microsoft.Data.Sqlite;

namespace ApacheLogs
{
    public static class DataBase
    {
        private const string ConnectionString = "Data Source=logs.db";

        public static void Create()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Logs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    IP TEXT NOT NULL,
                    Date TEXT NOT NULL,
                    Status INTEGER NOT NULL,
                    Size INTEGER NOT NULL
                )");
        }

        public static bool SetDatas(List<LogEntry> logs)
        {
            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();

                using var transaction = connection.BeginTransaction();
                foreach (var log in logs)
                {
                    connection.Execute(@"INSERT INTO Logs (IP, Date, Status, Size) VALUES (@IP, @Date, @Status, @Size)", log, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Error saving data to database: {ex.Message}");
                return false;
            }
        }

        public static void GetLogsByFilter(DateTime? dateFrom, DateTime? dateTo, string ip, int? status)
        {
            using var connection = new SQLiteConnection(ConnectionString);
            var logs = connection.Query<LogEntry>(@"
                SELECT * FROM Logs
                WHERE (@DateFrom IS NULL OR Date >= @DateFrom)
                  AND (@DateTo IS NULL OR Date <= @DateTo)
                  AND (@IP IS NULL OR IP = @IP)
                  AND (@Status IS NULL OR Status = @Status)",
                new { DateFrom = dateFrom, DateTo = dateTo, IP = ip, Status = status });

            foreach (var log in logs)
            {
                Console.WriteLine($"{log.Date} {log.IP} {log.Status} {log.Size}");
            }
        }
    }
}

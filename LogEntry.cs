using System;

namespace ApacheLogs
{
    public class LogEntry
    {
        public string IP { get; set; }
        public DateTime Date { get; set; }
        public int Status { get; set; }
        public int Size { get; set; }
    }
}

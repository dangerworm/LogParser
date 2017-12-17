namespace LogParser
{
    public class LogFileField
    {
        public string FileName { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public bool IsDateField { get; set; }
        public bool IsTimeField { get; set; }
    }
}

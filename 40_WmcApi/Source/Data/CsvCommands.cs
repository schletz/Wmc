using System.Security.Cryptography.X509Certificates;

namespace WmcApi.Data
{
    public class CsvCommands
    {
        public record Class(
            string Shortname, string ClassTeacher, string Department, int Term,
            bool Winter, bool Summer);
        public record Lesson(
            int Untisid, string Class, string Teacher, string Subject, string? Room,
            int Day, int Period);
    }
}
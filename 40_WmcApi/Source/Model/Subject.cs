using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace WmcApi.Model
{
    [Index(nameof(Shortname), IsUnique = true)]
    public class Subject : Entity<int>
    {
        private static readonly Regex _subjectCleanupExp = new Regex(@"[^A-Zxyz]", RegexOptions.Compiled);
        private static readonly Regex _subjectExp = new Regex(@"^(?<main>[A-Z]+)", RegexOptions.Compiled);

        public Subject(string shortname, string? name)
        {
            Shortname = shortname;
            Name = name;
            var shortnameCleaned = _subjectCleanupExp.Replace(shortname, string.Empty);
            var match = _subjectExp.Match(shortnameCleaned);
            MainSubject = match.Success ? match.Groups["main"].Value : null;
            IsMainSubject = MainSubject == shortnameCleaned;
        }

        public string Shortname { get; set; }
        public string? Name { get; set; }
        public string? MainSubject { get; set; }
        public bool IsMainSubject { get; set; }
    }
}
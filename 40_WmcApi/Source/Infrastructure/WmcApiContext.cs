using Bogus;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WmcApi.Data;
using WmcApi.Model;

namespace Spengernews.Application.Infrastructure
{
    public class WmcApiContext : DbContext
    {
        public WmcApiContext(DbContextOptions<WmcApiContext> opt) : base(opt)
        {
        }

        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Period> Periods => Set<Period>();
        public DbSet<Lesson> Lessons => Set<Lesson>();
        public DbSet<Schoolclass> Schoolclasses => Set<Schoolclass>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Grade> Grades => Set<Grade>();

        /// <summary>
        /// Creates the database. Called once at application startup.
        /// </summary>
        public void CreateDatabase(bool isDevelopment)
        {
            if (isDevelopment) { Database.EnsureDeleted(); }
            // EnsureCreated only creates the model if the database does not exist or it has no
            // tables. Returns true if the schema was created.  Returns false if there are
            // existing tables in the database. This avoids initializing multiple times.
            if (Database.EnsureCreated()) { Initialize(); }
            if (isDevelopment) Seed();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().OwnsOne(s => s.Address);
            modelBuilder.Entity<Grade>().HasIndex("StudentId", "SubjectShortname").IsUnique();

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Generic config for all entities
                // ON DELETE RESTRICT instead of ON DELETE CASCADE
                foreach (var key in entityType.GetForeignKeys())
                    key.DeleteBehavior = DeleteBehavior.Restrict;

                foreach (var prop in entityType.GetDeclaredProperties())
                {
                    // Define Guid as alternate key. The database can create a guid fou you.
                    if (prop.Name == "Guid")
                    {
                        modelBuilder.Entity(entityType.ClrType).HasAlternateKey("Guid");
                        prop.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                    }
                    // Default MaxLength of string Properties is 255.
                    if (prop.ClrType == typeof(string) && prop.GetMaxLength() is null) prop.SetMaxLength(255);
                    // Seconds with 3 fractional digits.
                    if (prop.ClrType == typeof(DateTime)) prop.SetPrecision(3);
                    if (prop.ClrType == typeof(DateTime?)) prop.SetPrecision(3);
                }
            }
        }

        /// <summary>
        /// Initialize the database with some values (holidays, ...).
        /// Unlike Seed, this method is also called in production.
        /// </summary>
        private void Initialize()
        {
        }

        /// <summary>
        /// Generates random values for testing the application. This method is only called in development mode.
        /// </summary>
        private void Seed()
        {
            Randomizer.Seed = new Random(1039);
            var faker = new Faker("de");

            var periods = ReadCsv<Period>("periods.txt");
            Periods.AddRange(periods);
            SaveChanges();

            var rooms = ReadCsv<Room>("rooms.txt");
            Rooms.AddRange(rooms);
            SaveChanges();

            var teachers = ReadCsv<Teacher>("teachers.txt");
            Teachers.AddRange(teachers);
            SaveChanges();

            var subjects = ReadCsv<Subject>("subjects.txt");
            Subjects.AddRange(subjects);
            SaveChanges();

            var schoolclasses = SeedSchoolclasses(faker, teachers, rooms);
            var lessons = SeedLessons(faker, teachers, schoolclasses, rooms, subjects, periods);
            var students = SeedStudents(schoolclasses);

            var grades = SeedGrades(faker, students, teachers, subjects);
        }

        private IList<Grade> SeedGrades(Faker faker, IList<Student> students, IList<Teacher> teachers, IList<Subject> subjects)
        {
            var grades = new List<Grade>();

            foreach (var s in students)
            {
                var lessons = s.Class.Lessons.DistinctBy(l => l.Subject.MainSubject);
                foreach (var l in lessons)
                {
                    var subject = subjects.FirstOrDefault(s => s.IsMainSubject && s.MainSubject == l.Subject.MainSubject);
                    if (subject is null || subject.MainSubject is null || subject.Name is null) { continue; }
                    var created = new DateTime(DateTime.UtcNow.Year, 11, 1).AddSeconds(faker.Random.Int(0, 14 * 3600));
                    grades.Add(new Grade(
                        teacher: l.Teacher, student: s, subjectShortname: subject.MainSubject, subjectLongname: subject.Name,
                        value: faker.Random.Int(1, 5), created: created));
                }
            }
            Grades.AddRange(grades);
            SaveChanges();
            return grades;
        }

        private IList<Lesson> SeedLessons(
            Faker faker, IList<Teacher> teachers, IList<Schoolclass> classes,
            IList<Room> rooms, IList<Subject> subjects, IList<Period> periods)
        {
            var teacherDict = teachers.ToDictionary(t => t.Shortname, t => t);
            var classesDict = classes.ToDictionary(c => c.Shortname, c => c);
            var roomDict = rooms.ToDictionary(r => r.Shortname, r => r);
            var subjectDict = subjects.ToDictionary(s => s.Shortname, s => s);
            var periodDict = periods.ToDictionary(p => p.Nr, p => p);

            var lessons = ReadCsv<CsvCommands.Lesson>("lessons.txt")
                .Where(l =>
                    l.Teacher is not null && l.Class is not null && l.Subject is not null &&
                    teacherDict.ContainsKey(l.Teacher) && classesDict.ContainsKey(l.Class) && subjectDict.ContainsKey(l.Subject))
                .Select(l =>
                {
                    if (l.Room is null || !roomDict.TryGetValue(l.Room, out var room)) room = null;
                    return new Lesson(
                        @class: classesDict[l.Class], teacher: teacherDict[l.Teacher], subject: subjectDict[l.Subject],
                        room: room, day: l.Day, period: periodDict[l.Period])
                    { Guid = faker.Random.Guid() };
                })
                .ToList();
            Lessons.AddRange(lessons);
            SaveChanges();
            return lessons;
        }

        private IList<Schoolclass> SeedSchoolclasses(Faker faker, IList<Teacher> teachers, IList<Room> rooms)
        {
            var teacherDict = teachers.ToDictionary(t => t.Shortname, t => t);
            var roomsClasses = rooms.Where(r => r.Info == "Stammklasse").ToList();
            var schoolclasses = ReadCsv<CsvCommands.Class>("classes.txt")
                .Where(c => teacherDict.ContainsKey(c.ClassTeacher))
                .Select(c =>
                {
                    return new Schoolclass(shortname: c.Shortname, department: c.Department,
                        classTeacher: teacherDict[c.ClassTeacher],
                        term: c.Term, winter: c.Winter, summer: c.Summer,
                        room: faker.Random.ListItem(roomsClasses).OrNull(faker, 0.2f))
                    { Guid = faker.Random.Guid() };
                })
                .ToList();
            Schoolclasses.AddRange(schoolclasses);
            SaveChanges();
            return schoolclasses;
        }

        private IList<Student> SeedStudents(IList<Schoolclass> schoolclasses)
        {
            var cities = new string[] { "Wien", "Eisenstadt", "Mattersburg", "Neusiedl am See", "Oberpullendorf", "Baden", "Bruck an der Leitha", "Gänserndorf", "Hollabrunn", "St. Pölten", "Wiener Neustadt", "Mödling" };
            var zip = new int[] { 1000, 7000, 7210, 7100, 7350, 2500, 2460, 2230, 2020, 3100, 2700, 2340 };
            var accountnr = 100000;
            var classesWinter = schoolclasses.Where(s => s.Winter).ToList();
            var students = new Faker<Student>("de").CustomInstantiator(f =>
            {
                var gender = f.Random.Enum<Bogus.DataSets.Name.Gender>();
                var @class = f.Random.ListItem(schoolclasses);
                var age = (int)(365.25 * (@class.Department.StartsWith("H") ? f.Random.Double(15, 20) : f.Random.Double(18, 30)));
                var addressIdx = f.Random.Int(0, cities.Length - 1);
                var lastname = f.Name.LastName(gender);
                var accountname = $"{(lastname.Length < 3 ? lastname : lastname.Substring(0, 3)).ToLower()}{accountnr++}";
                var email = $"{accountname}@spengergasse.at";
                return new Student(
                    accountname: accountname,
                    firstname: f.Name.FirstName(gender), lastname: lastname,
                    gender: gender == Bogus.DataSets.Name.Gender.Female ? Gender.Female : Gender.Male,
                    dateOfBirth: new DateTime(DateTime.UtcNow.Year, 1, 1).AddDays(-age),
                    email: email,
                    address: new Address(
                        Zip: zip[addressIdx] == 1000 ? (f.Random.Int(101, 123) * 10).ToString() : zip[addressIdx].ToString(),
                        City: cities[addressIdx],
                        Street: f.Address.StreetName(),
                        BuildingNumber: f.Address.BuildingNumber()),
                    @class: f.Random.ListItem(classesWinter))
                { Guid = f.Random.Guid() };
            })
            .Generate(classesWinter.Count * 20)
            .ToList();

            Students.AddRange(students);
            SaveChanges();
            return students;
        }

        private IList<T> ReadCsv<T>(string filename)
        {
            using var reader = new StreamReader(path: Path.Combine("Data", filename), encoding: new System.Text.UTF8Encoding(false));
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                PrepareHeaderForMatch = args => args.Header.ToLower(),
                Delimiter = "\t",
                HeaderValidated = args => { },
            };
            using var csv = new CsvReader(reader, config);
            csv.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add("");
            return csv.GetRecords<T>().ToList();
        }
    }
}
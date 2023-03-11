namespace WmcApi.Model
{
    public class Lesson : Entity<int>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        protected Lesson()
        { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Lesson(Schoolclass @class, Teacher teacher, Subject subject, int day, Period period, Room? room = null)
        {
            Class = @class;
            Teacher = teacher;
            Subject = subject;
            Room = room;
            Day = day;
            Period = period;
        }

        public Schoolclass Class { get; set; }
        public Teacher Teacher { get; set; }
        public Subject Subject { get; set; }
        public int Day { get; set; }
        public Period Period { get; set; }
        public Room? Room { get; set; }
    }
}
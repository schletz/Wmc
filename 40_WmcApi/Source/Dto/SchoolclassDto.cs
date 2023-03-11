using System;
using System.Collections.Generic;

namespace WmcApi.Dto
{
    public record SchoolclassDto(
        string Shortname, string Department, int term,
        bool winter, bool summer,
        string classTeacherShortname,
        string roomShortname,
        int StudentCount, int StudentMaleCount, int StudentFemaleCount);
    public record SchoolclassDetailsDto(
        string Shortname, string Department, int term,
        bool winter, bool summer, TeacherDto classTeacher, RoomDto room, IEnumerable<StudentDto> Students);
}
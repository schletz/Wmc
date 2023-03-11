using System;
using WmcApi.Model;

namespace WmcApi.Dto
{
    public record StudentDto(
        Guid Guid, string accountname, string Firstname, string Lastname,
        string Gender, string email, string ClassShortname);
    public record StudentDetailsDto(
        Guid Guid, string accountname, string Firstname, string Lastname,
        string Gender, string email, string DateOfBirth, Address Address, string ClassShortname);
}
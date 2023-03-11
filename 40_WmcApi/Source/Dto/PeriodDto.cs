using System;

namespace WmcApi.Dto
{
    public record PeriodDto(int Nr, string Start, string End, int Duration, bool IsEvening);
}
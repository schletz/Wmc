using Microsoft.AspNetCore.Mvc;
using WmcApi.HolidayCalendar;

namespace WmcApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CalendarController : ControllerBase
    {
        private readonly CalendarService _calendar;

        public CalendarController(CalendarService calendar)
        {
            _calendar = calendar;
        }

        [HttpGet("{year}")]
        public IActionResult GetYear(int year)
        {
            if (year < 2000 || year > 2100)
                return BadRequest("Invalid year. Valid range is from 2000 to 2100.");

            return Ok(_calendar.GetDaysOfYear(year));
        }

        [HttpGet("{year}/{month}")]
        public IActionResult GetYear(int year, int month, [FromQuery] int fullWeeks = 0)
        {
            if (year < 2000 || year > 2100)
                return BadRequest("Invalid year. Valid range is from 2000 to 2100.");
            if (month < 1 || month > 12)
                return BadRequest("Invalid month.");

            if (fullWeeks == 1) return Ok(_calendar.GetDaysOfMonthFullWeeks(year, month));
            return Ok(_calendar.GetDaysOfMonth(year, month));
        }
    }
}
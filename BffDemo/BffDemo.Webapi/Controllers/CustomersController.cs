using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BffDemo.Webapi.Controllers
{
    /// <summary>
    /// Example for a protected controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        public record CustomerDto(string Firstname, string Lastname);

        public CustomersController()
        { }

        [HttpGet]
        public IActionResult GetAllCustomers()
        {
            var data = new CustomerDto[]
            {
                new CustomerDto("Firstname1", "Lastname1"),
                new CustomerDto("Firstname2", "Lastname2"),
                new CustomerDto("Firstname3", "Lastname3")
            };
            return Ok(data);
        }
    }
}
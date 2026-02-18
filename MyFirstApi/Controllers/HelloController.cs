using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;
using MyFirstApi.Services;

namespace MyFirstApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        private readonly IGreetingService _greetingService;
        public HelloController(IGreetingService greetingService)
        {
            _greetingService = greetingService;
        }

        [HttpGet]
        public String Get()
        {
            return "Hej fra min første controller!";
        }

        [HttpGet("{name}")]
        public String Get(string name)
        {
            return _greetingService.CreateGreating(name);
        }

        [HttpPost]
        public IActionResult Post([FromBody] Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok($"Modtaget person: {person.Name}, som er {person.Age} år gammel.!");
        }
    }
}

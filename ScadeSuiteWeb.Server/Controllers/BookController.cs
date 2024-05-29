using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScadeSuiteWeb.Server.Database;
using ScadeSuiteWeb.Server.Models;
using ScadeSuiteWeb.Server.Repositories;

namespace ScadeSuiteWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController(IBookRepository bookRepository) : ControllerBase
    {
        [HttpGet]
        [Route(nameof(Tests))]
        public Task<List<Book>> Tests()
        {
            return Task.FromResult(bookRepository.GetAll());
        }
    }
}

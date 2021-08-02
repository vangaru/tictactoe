using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TicTacToeOnlineGame.Models;

namespace TicTacToeOnlineGame.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagApiController : ControllerBase
    {
        private readonly ApplicationContext _db;

        public TagApiController(ApplicationContext db)
        {
            _db = db;
        }

        [Produces("application/json")]
        [HttpGet("search")]
        public IActionResult Search()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                var tags = _db.Tags.Where(t => t.Text.Contains(term))
                    .Select(t => t.Text).ToList();
                return Ok(tags);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
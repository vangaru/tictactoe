using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TicTacToeOnlineGame.Models;

namespace TicTacToeOnlineGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Rooms.Where(r => r.Status == "Open").ToList());
        }

        public async Task<IActionResult> Room(int? id)
        {
            var room = _db.Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null) return NotFound();

            room.PlayersCount++;
            if (room.PlayersCount >= 2)
            {
                room.Status = "Closed";
            }

            await _db.SaveChangesAsync();
            
            return View(room);
        }

        public IActionResult CreateRoom()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateRoom(CreateRoomModel model)
        {
            var tags = RemoveRepeatableTags(model.Tags);

            var room = _db.Rooms.FirstOrDefault(r => r.Name == model.Name);
            
            if (room != null)
            {
                ModelState.AddModelError("", "Room with this name already exists");
                return View(model);
            }

            int.TryParse(model.FirstMoveCellId, out var cellId);
            
            room = new Room {Name = model.Name, Tags = tags, FirstMoveCellId = cellId};
            
            _db.Rooms.Add(room);

            await _db.SaveChangesAsync();

            await AddOrIncrementTags(tags);
            
            return RedirectToAction("Room", "Home", new {id = room.Id});
        }

        public async Task<IActionResult> DeleteRoom(int? id)
        {
            var roomToDelete = _db.Rooms.FirstOrDefault(r => r.Id == id);
            if (roomToDelete != null) _db.Rooms.Remove(roomToDelete);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Filter(string? tags)
        {
            if (tags == null) return RedirectToAction("Index", "Home");

            var tagList = tags.Split(' ').
                Distinct().
                ToList();

            var fileredRooms = new List<Room>();
            
            foreach (var dbRoom in _db.Rooms)
            {
                if (dbRoom.Tags.Split(' ').Any(t => tagList.Contains(t)))
                {
                    fileredRooms.Add(dbRoom);
                }
            }

            return View("Index", fileredRooms);
        }
        
        private string RemoveRepeatableTags(string tags)
        {
            var tagsArr = tags.Split(' ').ToList();
            var tagsArrUnique = tagsArr.Distinct().ToList();
            return string.Join(" ", tagsArrUnique.ToArray());
        }
        
        private async Task AddOrIncrementTags(string stringTags)
        {
            var tags = stringTags.Split(' ');
            foreach (var tag in tags)
            {
                var tagToIncrement = _db.Tags.FirstOrDefault(t => t.Text == tag);
                if (tagToIncrement == null)
                {
                    var tagToAdd = new Tag {Text = tag};
                    _db.Tags.Add(tagToAdd);
                }
                else
                {
                    tagToIncrement.Count++;
                }
            }

            await _db.SaveChangesAsync();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
using Competition_Tournament.Data;
using Competition_Tournament.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Competition_Tournament.Controllers
{
    public class TeamController : Controller
    {
        private readonly CompetitionManagementContext _context;

        public TeamController(CompetitionManagementContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Teams.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            var team = _context.Teams.Find(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        [HttpPost]
        public IActionResult Create(Team team)
        {
            var exist = _context.Teams.Where(c => c.Name == team.Name).FirstOrDefault();
         
            if(exist != null)
            {
                string first = exist.Name.ToLower();
                string second = team.Name.ToLower();
                if (first == second)
                {
                    ModelState.AddModelError("Name", "The name already exists");
                }
            }

            if (team.CreatedOn > DateTime.Today)
            {
                ModelState.AddModelError("CreatedOn", "The date cannot be in the future.");
            }


            if (ModelState.IsValid)
            {
                _context.Teams.Add(team);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(team);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                var players = _context.Players.Where(p => p.TeamId == id);
                foreach (var player in players)
                {
                    // To delete the player:
                    // _context.Players.Remove(player);

                    // To disassociate the player from the team:
                    player.TeamId = null;
                }

                _context.Teams.Remove(team);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Edit(Team team)
        {
            var exist = _context.Teams.Where(c => c.Name == team.Name).FirstOrDefault();
         
            if(exist != null)
            {
                string first = exist.Name.ToLower();
                string second = team.Name.ToLower();
                if (first == second)
                {
                    ModelState.AddModelError("Name", "The name already exists");
                }
            }

            if (team.CreatedOn > DateTime.Today)
            {
                ModelState.AddModelError("CreatedOn", "The date cannot be in the future.");
            }

            if (ModelState.IsValid)
            {
                _context.Entry(team).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(team);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Competition_Tournament.Data;
using Competition_Tournament.Models;

namespace Competition_Tournament.Controllers
{
    public class GamesController : Controller
    {
        private readonly CompetitionManagementContext _context;

        public GamesController(CompetitionManagementContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index(int? id)
        {
            var competitionManagementContext = _context.Games.Where(g => g.CompetitionId == id).Include(g => g.Competition).Include(g => g.Team1).Include(g => g.Team2);
            if (id == null)
            {
                competitionManagementContext = _context.Games.Include(g => g.Competition).Include(g => g.Team1).Include(g => g.Team2);
            }
            return View(await competitionManagementContext.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Competition)
                .Include(g => g.Team1)
                .Include(g => g.Team2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create(int? id)
        {
            Competition competition = _context.Competitions.Include(c => c.Teams).FirstOrDefault(c => c.Id == id);
            ViewData["id"] = id;
            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Name");
            ViewData["Team1Id"] = new SelectList(competition.Teams, "Id", "Name");
            ViewData["Team2Id"] = new SelectList(competition.Teams, "Id", "Name");
            ViewData["nr"] = competition.Teams.Count;
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Team1Id,Team2Id,Team1Score,Team2Score,Team1Name,Team2Name,CompetitionId,Date,Stadium")] Game game, int CompId)
        {
            Competition competition = _context.Competitions.Include(c => c.Teams).FirstOrDefault(c => c.Id == CompId);
            if (ModelState.IsValid)
            {
                if (game.Team1Id == game.Team2Id)
                {
                    ModelState.AddModelError("Team2Id", "Teams must be different");
                    ViewData["id"] = CompId;
                    ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Name");
                    ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Name");
                    ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Name");
                    ViewData["nr"] = competition.Teams.Count;
                    return View(game);
                }
                if (game.Team1Score != null && game.Team2Score != null && competition.CompetitionType == 2 && game.Team1Score == game.Team2Score)
                {
                    ModelState.AddModelError("Team2Score", "You can not have a equal game in Knock-Out");
                    ViewData["id"] = CompId;
                    ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Name");
                    ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Name");
                    ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Name");
                    ViewData["nr"] = competition.Teams.Count;
                    return View(game);
                }
                game.Team1Name = _context.Teams.Find(game.Team1Id).Name;
				game.Team2Name = _context.Teams.Find(game.Team2Id).Name;
                game.CompetitionId = CompId;
				_context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = CompId });
            }
            ViewData["id"] = CompId;
            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Name");
            ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Name");
            ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Name");
            ViewData["nr"] = competition.Teams.Count;
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Name", game.CompetitionId);
            ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Name", game.Team1Id);
            ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Name", game.Team2Id);
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Team1Id,Team2Id,Team1Score,Team2Score,Team1Name,Team2Name,CompetitionId,Date,Stadium")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            Competition competition = _context.Competitions.Include(c => c.Teams).FirstOrDefault(c => c.Id == game.CompetitionId);
            if (ModelState.IsValid)
            {
                try
                {
                    if (game.Team1Id == game.Team2Id)
                    {
                        ModelState.AddModelError("Team2Id", "Teams must be different");
                        ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Name");
                        ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Name");
                        ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Name");
                        return View(game);
                    }
                    if (game.Team1Score != null && game.Team2Score != null && competition.CompetitionType == 2 && game.Team1Score == game.Team2Score)
                    {
                        ModelState.AddModelError("Team2Score", "You can not have a equal game in Knock-Out");
                        ViewData["id"] = game.CompetitionId;
                        ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Name");
                        ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Name");
                        ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Name");
                        ViewData["nr"] = competition.Teams.Count;
                        return View(game);
                    }
                    game.Team1Name = _context.Teams.Find(game.Team1Id).Name;
					game.Team2Name = _context.Teams.Find(game.Team2Id).Name;
					_context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                int? CompId = game.CompetitionId;
                return RedirectToAction(nameof(Index), new {id = CompId});
            }
            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Id", game.CompetitionId);
            ViewData["Team1Id"] = new SelectList(_context.Teams, "Id", "Id", game.Team1Id);
            ViewData["Team2Id"] = new SelectList(_context.Teams, "Id", "Id", game.Team2Id);
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Competition)
                .Include(g => g.Team1)
                .Include(g => g.Team2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Games == null)
            {
                return Problem("Entity set 'CompetitionManagementContext.Games'  is null.");
            }
            var game = await _context.Games.FindAsync(id);
            int? CompId = game?.CompetitionId;
            if (game != null)
            {
                _context.Games.Remove(game);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = CompId });
        }

        private bool GameExists(int id)
        {
          return (_context.Games?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

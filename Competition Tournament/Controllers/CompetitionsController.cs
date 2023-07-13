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
    public class CompetitionsController : Controller
    {
        private readonly CompetitionManagementContext _context;

        public CompetitionsController(CompetitionManagementContext context)
        {
            _context = context;
        }

        // GET: Competitions
        public async Task<IActionResult> Index()
        {
            var competitionManagementContext = _context.Competitions.Include(c => c.CompetitionTypeNavigation);
            return View(await competitionManagementContext.ToListAsync());
        }

        // GET: Competitions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Competitions == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions.Include(c => c.CompetitionTypeNavigation).Include(c => c.Games).FirstOrDefaultAsync(m => m.Id == id);
            if (competition == null)
            {
                return NotFound();
            }

            return View(competition);
        }

        // GET: Competitions/Create
        public IActionResult Create()
        {
            ViewData["CompetitionType"] = new SelectList(_context.Competitiontypes, "Id", "Name");
            return View();
        }

        // POST: Competitions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,EndDate,Location,CompetitionType")] Competition competition)
        {
            if (ModelState.IsValid)
            {
                if (competition.StartDate > competition.EndDate)
                {
                    ModelState.AddModelError("EndDate", "End Date must be after Start Date.");
                    ViewData["CompetitionType"] = new SelectList(_context.Competitiontypes, "Id", "Name", competition.CompetitionType);
                    return View(competition);
                }
                _context.Add(competition);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewData["CompetitionType"] = new SelectList(_context.Competitiontypes, "Id", "Name", competition.CompetitionType);
            return View(competition);
        }

        // GET: Competitions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Competitions == null)
            {
                return NotFound();
            }

            Competition? competition = await _context.Competitions
                .Include(c => c.Teams)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (competition == null)
            {
                return NotFound();
            }
            ViewData["CompetitionType"] = new SelectList(_context.Competitiontypes, "Id", "Name", competition.CompetitionType);
            return View(competition);
        }

        // POST: Competitions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,EndDate,StartDate,Location,CompetitionType")] Competition competition)
        {
            if (id != competition.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (competition.StartDate > competition.EndDate)
                {
                    ModelState.AddModelError("EndDate", "End Date must be after Start Date.");
                    ViewData["CompetitionType"] = new SelectList(_context.Competitiontypes, "Id", "Name", competition.CompetitionType);
                    return View(competition);
                }
                try
                {
                    _context.Update(competition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompetitionExists(competition.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompetitionType"] = new SelectList(_context.Competitiontypes, "Id", "Id", competition.CompetitionType);
            return View(competition);
        }

        // GET: Competitions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Competitions == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions
                .Include(c => c.CompetitionTypeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (competition == null)
            {
                return NotFound();
            }

            return View(competition);
        }

        // POST: Competitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Competition? comp = await _context.Competitions
                .Include(c => c.Teams).Include(c => c.Games)
                .FirstOrDefaultAsync(c => c.Id == id);
            List<Game> del = new List<Game>();

            foreach (var game in comp.Games)
            {
                if (game.CompetitionId == id)
                {
                    del.Add(game);
                }
            }

            foreach (var game in del)
            {
                _context.Games.Remove(game);
            }

            if (_context.Competitions == null)
            {
                return Problem("Entity set 'CompetitionManagementContext.Competitions'  is null.");
            }
            var competition = await _context.Competitions.FindAsync(id);
            if (competition != null)
            {
                _context.Competitions.Remove(competition);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompetitionExists(int id)
        {
          return (_context.Competitions?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Teams(int competitionId)
        {
            Competition? competition = await _context.Competitions
                .Include(c => c.Teams)
                .FirstOrDefaultAsync(c => c.Id == competitionId);

            List<Team> allTeams = await _context.Teams.ToListAsync();
            if (competition == null)
            {
                return NotFound();
            }

            List<Team> participatingTeams = competition.Teams.ToList();
            List<Team> availableTeams = allTeams.Except(participatingTeams).ToList();

            competition.AllTeams = availableTeams;
            ViewData["CompetitionId"] = competitionId;
            return View(competition);
        }

        [HttpPost]
        public async Task<IActionResult> AddTeam(int competitionId, int teamId)
        {
            Competition? competition = await _context.Competitions
                .Include(c => c.Teams)
                .FirstOrDefaultAsync(c => c.Id == competitionId);

            Team? teamToAdd = await _context.Teams.FindAsync(teamId);

            if (competition == null || teamToAdd == null)
            {
                //return NotFound();
                return RedirectToAction("Teams", new { competitionId });
            }

            if (competition.Teams.Contains(teamToAdd))
            {
                return BadRequest("The team is already participating in the competition.");
            }

            competition.Teams.Add(teamToAdd);
            await _context.SaveChangesAsync();

            return RedirectToAction("Teams", new { competitionId });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteTeam(int competitionId, int teamId)
        {
            Competition? competition = await _context.Competitions
                .Include(c => c.Teams)
                    .ThenInclude(t => t.GameTeam1s)
                .Include(c => c.Teams)
                    .ThenInclude(t => t.GameTeam2s)
                .FirstOrDefaultAsync(c => c.Id == competitionId);

            if (competition == null)
            {
                return RedirectToAction("Teams", new { competitionId });
            }

            List<Game> del = new List<Game>();
            foreach (var game in competition.Games)
            {
                if (game.Team1Id == teamId || game.Team2Id == teamId)
                {
                    del.Add(game);
                }
            }

            foreach (var team in competition.Teams)
            {
                if (team.Id == teamId)
                {
                    competition.Teams.Remove(team);
                    break;
                }
            }

            foreach (var game in del)
            {
                _context.Games.Remove(game);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Teams", new { competitionId });
        }

        public async Task<IActionResult> Leaderboard(int? id)
        {
            if (id == null || _context.Competitions == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions.Include(c => c.CompetitionTypeNavigation).Include(c => c.Games).FirstOrDefaultAsync(m => m.Id == id);
            if (competition == null)
            {
                return NotFound();
            }

            return View(competition);
        }
    }
}

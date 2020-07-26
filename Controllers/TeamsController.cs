using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projects_Workshop.Models;
using Projects_Workshop.Models.Entities;
using Projects_Workshop.Models.ViewModels;

namespace Projects_Workshop.Controllers
{
    public class TeamsController : Controller
    {
        private readonly EFDBContext _context;

        public TeamsController(EFDBContext context)
        {
            _context = context;
        }

        public IActionResult AllTeams(string sortOrder)
        {
            ViewData["TeamNameSortParm"] = sortOrder == "Названию▼" ? "Названию▲" : "Названию▼";
            ViewData["MembersSortParm"] = sortOrder == "Участникам▼" ? "Участникам▲" : "Участникам▼";
            var teams = _context.Teams
                   .Include(t => t.Members)
                   .Include(t => t.Project);
            var sortedTeams = new List<Team>();
            switch (sortOrder)
            {
                case "Участникам▲":
                    sortedTeams = teams.OrderBy(p => p.Members.Count).ToList();
                    ViewData["MembersSort"] = sortOrder;
                    ViewData["TeamNameSort"] = "Названию";
                    break;
                case "Участникам▼":
                    sortedTeams = teams.OrderByDescending(p => p.Members.Count).ToList();
                    ViewData["MembersSort"] = sortOrder;
                    ViewData["TeamNameSort"] = "Названию";
                    break;
                case "Названию▲":
                    sortedTeams = teams.OrderByDescending(p => p.Name).ToList();
                    ViewData["TeamNameSort"] = sortOrder;
                    ViewData["MembersSort"] = "Участникам";
                    break;
                default:
                    sortedTeams = teams.OrderBy(p => p.Name).ToList();
                    ViewData["TeamNameSort"] = "Названию▼";
                    ViewData["TeamNameSortParm"] = "Названию▲";
                    ViewData["MembersSort"] = "Участникам";
                    break;
            }
            return View(sortedTeams);
        }

        public IActionResult TeamEditor(int? id)
        {
            var list = new List<string> { "" };
            list.AddRange(_context.Projects.OrderBy(p => p.Name).Select(p => p.Name));
            var model = new TeamEditor
            {
                AllProjects = new SelectList(list)
            };
            if (id != null)
            {
                var team = _context.Teams
                    .Include(u => u.Project)
                    .SingleOrDefault(u => u.Id == id);
                model.TeamId = team.Id;
                model.TeamName = team.Name;
                model.SelectedProject = team.Project?.Name;
            }
            return View(model);
        }

        public IActionResult AddTeam(TeamEditor model)
        {
            var project = _context.Projects.SingleOrDefault(p => p.Name == model.SelectedProject);
            if (model.TeamId == null)
            {
                var newTeam = new Team
                {
                    Name = model.TeamName,
                    Project = project
                };
                _context.Teams.Add(newTeam);
            }
            else
            {
                var teamToUpdate = _context.Teams
                    .Include(t => t.Project)
                    .Include(t => t.Members)
                    .SingleOrDefault(u => u.Id == model.TeamId);
                foreach (var student in teamToUpdate.Members)
                {
                    student.Project = project;
                }
                teamToUpdate.Name = model.TeamName;
                teamToUpdate.Project = project;
                _context.Update(teamToUpdate);
            }
            _context.SaveChanges();
            return RedirectToAction("AllTeams", "Teams");
        }

        public IActionResult DeleteTeam(int id)
        {
            var teamToRemove = _context.Teams.Include(t => t.Project).Include(t => t.Members).SingleOrDefault(t => t.Id == id);
            _context.Teams.Remove(teamToRemove);
            _context.SaveChanges();
            return RedirectToAction("AllTeams", "Teams");
        }

        [HttpPost]
        public JsonResult TeamNameCheck(string teamName, int? TeamId)
        {
            return Json(teamName != null && (!_context.Teams.Select(t => t.Name).Contains(teamName) || TeamId != null));
        }

        public void TempDataMessage(string key, string alert, string value)
        {
            TempData.Remove(key);
            TempData.Add(key, value);
            TempData.Add("alertType", alert);
        }
    }
}
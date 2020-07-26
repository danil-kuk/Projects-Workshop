using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projects_Workshop.Models;
using Projects_Workshop.Models.Entities;
using Projects_Workshop.Models.ViewModels;

namespace Projects_Workshop.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly EFDBContext _context;

        public ProjectsController(EFDBContext context)
        {
            _context = context;
        }

        public IActionResult AllProjects(string sortOrder)
        {
            ViewData["ProjectNameSortParm"] = sortOrder == "Названию▼" ? "Названию▲" : "Названию▼";
            ViewData["PartisipantsSortParm"] = sortOrder == "Участникам▼" ? "Участникам▲" : "Участникам▼";
            var projects = _context.Projects
                   .Include(u => u.Teams)
                   .Include(u => u.Participants);
            var sortedProjects = new List<Project>();
            switch (sortOrder)
            {
                case "Участникам▲":
                    sortedProjects = projects.OrderBy(p => p.Participants.Count).ToList();
                    ViewData["PartisipantsSort"] = sortOrder;
                    ViewData["ProjectNameSort"] = "Названию";
                    break;
                case "Участникам▼":
                    sortedProjects = projects.OrderByDescending(p => p.Participants.Count).ToList();
                    ViewData["PartisipantsSort"] = sortOrder;
                    ViewData["ProjectNameSort"] = "Названию";
                    break;
                case "Названию▲":
                    sortedProjects = projects.OrderByDescending(p => p.Name).ToList();
                    ViewData["ProjectNameSort"] = sortOrder;
                    ViewData["PartisipantsSort"] = "Участникам";
                    break;
                default:
                    sortedProjects = projects.OrderBy(p => p.Name).ToList();
                    ViewData["ProjectNameSort"] = "Названию▼";
                    ViewData["ProjectNameSortParm"] = "Названию▲";
                    ViewData["PartisipantsSort"] = "Участникам";
                    break;
            }
            return View(sortedProjects);
        }

        public IActionResult ProjectEditor(int? id)
        {
            var model = new ProjectEditor
            {
                AllTeams = new SelectList(_context.Teams.OrderBy(t => t.Name).Select(t => t.Name))
            };
            if (id != null)
            {
                var project = _context.Projects
                    .Include(p => p.Teams)
                    .SingleOrDefault(p => p.Id == id);
                model.ProjectId = project.Id;
                model.ProjectName = project.Name;
                model.ShortDescription = project.ShortDescription;
                model.Theme = project.Theme;
                model.GoalDescription = project.GoalDescription;
                model.ProblemDescription = project.ProblemDescription;
                model.RolesDescription = project.RolesDescription;
                model.ResultDescription = project.ResultDescription;
                model.Curator = project.Curator;
                model.CustomerOrganization = project.CustomerOrganization;
                model.CustomerContacts = project.CustomerContacts;
                model.SelectedTeams = project.Teams?.Select(t => t.Name).ToList();
            }
            return View(model);
        }


        public IActionResult AddProject(ProjectEditor model)
        {
            var teams = new List<Team>();
            model.SelectedTeams?.ForEach(t => teams.Add(_context.Teams.Include(p => p.Project).Include(u => u.Members).SingleOrDefault(i => i.Name == t)));
            var members = new List<User>();
            foreach (var team in teams)
            {
                foreach (var student in team.Members)
                {
                    members.Add(student);
                }
            }
            if (model.ProjectId == null)
            {
                var newProject = new Project
                {
                    Name = model.ProjectName,
                    ShortDescription = model.ShortDescription,
                    Teams = teams,
                    Theme = model.Theme,
                    GoalDescription = model.GoalDescription,
                    ProblemDescription = model.ProblemDescription,
                    RolesDescription = model.RolesDescription,
                    ResultDescription = model.ResultDescription,
                    Curator = model.Curator,
                    CustomerOrganization = model.CustomerOrganization,
                    CustomerContacts = model.CustomerContacts,
                    Participants = members
                };
                _context.Projects.Add(newProject);
                _context.SaveChanges();
                return RedirectToAction("AllProjects", "Projects");
            }
            else
            {
                var projectToUpdate = _context.Projects.Include(p => p.Teams).SingleOrDefault(p => p.Id == model.ProjectId);
                projectToUpdate.Name = model.ProjectName;
                projectToUpdate.ShortDescription = model.ShortDescription;
                projectToUpdate.Teams = teams;
                projectToUpdate.Theme = model.Theme;
                projectToUpdate.GoalDescription = model.GoalDescription;
                projectToUpdate.ProblemDescription = model.ProblemDescription;
                projectToUpdate.RolesDescription = model.RolesDescription;
                projectToUpdate.ResultDescription = model.ResultDescription;
                projectToUpdate.Curator = model.Curator;
                projectToUpdate.CustomerOrganization = model.CustomerOrganization;
                projectToUpdate.CustomerContacts = model.CustomerContacts;
                projectToUpdate.Participants = members;
                _context.Update(projectToUpdate);
                _context.SaveChanges();
                return RedirectToAction("ProjectPage", "Projects", new { id = model.ProjectId });
            }

        }

        public IActionResult ProjectPage(int id)
        {
            var selectedProject = _context.Projects
                   .Include(u => u.Teams)
                   .Include(u => u.Participants)
                   .SingleOrDefault(p => p.Id == id);
            return View(selectedProject);
        }

        public IActionResult DeleteProject(int id)
        {
            var projectToRemove = _context.Projects.Include(p => p.Teams).Include(p => p.Participants).SingleOrDefault(p => p.Id == id);
            _context.Projects.Remove(projectToRemove);
            _context.SaveChanges();
            return RedirectToAction("AllProjects", "Projects");
        }

        public FileResult DownloadFile()
        {
            var info = new StringBuilder();
            var header = "Проект\tНаправление\tКуратор\tКраткое описание\tЦель\tПроблема\tРоли и компетенции\tРезультат\tЗаказчик\tКонтакты заказчика\tУчастники";
            info.AppendLine(header);
            var projects = _context.Projects
                .Include(p => p.Teams)
                .Include(p => p.Participants)
                .OrderBy(p => p.Name);
            foreach (var project in projects)
            {
                var participantsInfo = new StringBuilder();
                foreach (var team in project.Teams)
                {
                    foreach (var student in team.Members.OrderBy(u => u.Name))
                    {
                        participantsInfo.Append($"{student.Name} ({team.Name}), ");
                    }
                }
                if (participantsInfo.Length > 0)
                    participantsInfo.Remove(participantsInfo.Length - 2, 2);
                var line = $"{project.Name}\t{project.Theme}\t{project.Curator}\t{project.ShortDescription}\t" +
                    $"{project.GoalDescription}\t{project.ProblemDescription}\t{project.RolesDescription}\t" +
                    $"{project.ResultDescription}\t{project.CustomerOrganization}\t{project.CustomerContacts}\t{participantsInfo}";
                info.AppendLine(line);
            }
            byte[] data = Encoding.UTF8.GetBytes(info.ToString());
            info.Length = 0;
            var today = DateTime.Today.ToString("dd.MM.yy");
            return File(data, "text/tsv;charset=utf-8", $"Проекты({today}).tsv");
        }

        [HttpPost]
        public JsonResult ProjectNameCheck(string projectName, int? ProjectId)
        {
            return Json(projectName != null && (!_context.Projects.Select(t => t.Name).Contains(projectName) || ProjectId != null));
        }

        public void TempDataMessage(string key, string alert, string value)
        {
            TempData.Remove(key);
            TempData.Add(key, value);
            TempData.Add("alertType", alert);
        }
    }
}
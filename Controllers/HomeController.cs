using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Projects_Workshop.Models;
using Projects_Workshop.Models.Entities;
using Projects_Workshop.Models.ViewModels;

namespace Projects_Workshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly EFDBContext _context;

        public HomeController(EFDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UploadData()
        {
            return View();
        }

        public IActionResult ReadProjectsData(IFormFile file)
        {
            if (file.FileName.Split('.').Last() != "tsv")
                return RedirectToAction("UploadData", "Home");
            var array = GetArray(file);
            var projects = _context.Projects.ToDictionary(p => p.Name);
            foreach (var line in array)
            {
                if (line[0].Length == 0)
                    continue;
                if (!projects.ContainsKey(line[0]))
                {
                    var newProject = new Project
                    {
                        Name = line[0]
                    };
                    projects.Add(line[0], newProject);
                    _context.Projects.Add(newProject);
                }
                var project = projects[line[0]];
                project.Theme = line[1];
                project.Curator = line[2];
                project.ShortDescription = line[3];
                project.GoalDescription = line[4];
                project.ProblemDescription = line[5];
                project.RolesDescription = line[6];
                project.ResultDescription = line[7];
                project.CustomerOrganization = line[8];
                project.CustomerContacts = line[9];
            }
            _context.SaveChanges();
            return RedirectToAction("AllProjects", "Projects");
        }

        public IActionResult ReadTeamsData(IFormFile file)
        {
            if (file.FileName.Split('.').Last() != "tsv")
                return RedirectToAction("UploadData", "Home");
            var array = GetArray(file);
            var teams = _context.Teams.Include(t => t.Project).Include(t => t.Members).ToDictionary(t => t.Name);
            var projects = _context.Projects.Include(p => p.Teams).ToDictionary(p => p.Name);
            foreach (var line in array)
            {
                if (line[0].Length == 0 || line[1].Length == 0)
                    continue;
                if (!projects.ContainsKey(line[1]))
                {
                    var newProject = new Project
                    {
                        Name = line[1]
                    };
                    projects.Add(line[1], newProject);
                    _context.Projects.Add(newProject);
                }
                if (!teams.ContainsKey(line[0]))
                {
                    var newTeam = new Team
                    {
                        Name = line[0],
                        Project = projects[line[1]]
                    };
                    teams.Add(line[0], newTeam);
                    _context.Teams.Add(newTeam);
                }
                else
                {
                    var team = teams[line[0]];
                    var project = projects[line[1]];
                    team.Project = project;
                    foreach (var student in team.Members)
                    {
                        student.Project = project;
                    }
                }
            }
            _context.SaveChanges();
            return RedirectToAction("AllTeams", "Teams");
        }

        public IActionResult ReadUsersData(IFormFile file)
        {
            if (file.FileName.Split('.').Last() != "tsv")
                return RedirectToAction("UploadData", "Home");
            var array = GetArray(file);
            var users = _context.Users.Include(u => u.Team).Include(u => u.Project).ToDictionary(u => u.Name);
            var teams = _context.Teams.Include(t => t.Project).ToDictionary(t => t.Name);
            foreach (var line in array)
            {
                if (line[0].Length == 0)
                    continue;
                if (!users.ContainsKey(line[0]))
                {
                    var newUser = new User
                    {
                        Name = line[0]
                    };
                    users.Add(line[0], newUser);
                    _context.Users.Add(newUser);
                }
                var user = users[line[0]];
                if (!teams.ContainsKey(line[3]))
                {
                    var newTeam = new Team
                    {
                        Name = line[3]
                    };
                    teams.Add(line[3], newTeam);
                    _context.Teams.Add(newTeam);
                }
                user.Course = line[1];
                user.Group = line[2];
                user.Team = teams[line[3]];
                user.Project = user.Team.Project;
            }
            _context.SaveChanges();
            return RedirectToAction("AllUsers", "Users");
        }

        public IActionResult DownloadExample(string type)
        {
            var info = new StringBuilder();
            var header = "";
            switch (type)
            {
                case "projects":
                    header = "Проект\tНаправление\tКуратор\tКраткое описание\tЦель\tПроблема\tРоли и компетенции\tРезультат\tЗаказчик\tКонтакты заказчика";
                    break;
                case "teams":
                    header = "Команда\tПроект";
                    break;
                case "users":
                    header = "ФИО\tКурс\tГруппа\tКоманда";
                    break;
            }
            info.AppendLine(header);
            byte[] data = Encoding.UTF8.GetBytes(info.ToString());
            info.Length = 0;
            return File(data, "text/tsv;charset=utf-8", $"Пример({type}).tsv");
        }

        private string[][] GetArray(IFormFile file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(reader.ReadLine());
            }
            var lines = result.ToString().Split("\r\n");
            var array = lines.Skip(1).Take(lines.Length - 2).Select(l => l.Split('\t')).ToArray();
            return array;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public void TempDataMessage(string key, string alert, string value)
        {
            TempData.Remove(key);
            TempData.Add(key, value);
            TempData.Add("alertType", alert);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

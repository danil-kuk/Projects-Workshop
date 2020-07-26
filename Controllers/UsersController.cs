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
    public class UsersController : Controller
    {
        private readonly EFDBContext _context;

        public UsersController(EFDBContext context)
        {
            _context = context;
        }

        public IActionResult AllUsers()
        {
            var users = _context.Users
                .Include(u => u.Team)
                .Include(u => u.Project)
                .OrderBy(u => u.Name)
                .ToList();
            return View(users);
        }

        public IActionResult UserEditor(int? id)
        {
            var model = new UserEditor
            {
                AllTeams = new SelectList(_context.Teams.Select(t => t.Name))
            };
            if (id != null)
            {
                var user = _context.Users
                    .Include(u => u.Team)
                    .Include(u => u.Project)
                    .SingleOrDefault(u => u.Id == id);
                model.UserId = user.Id;
                model.UserName = user.Name;
                model.SelectedTeam = user.Team?.Name;
                model.UserCourse = user.Course;
                model.UserGroup = user.Group;
            }
            return View(model);
        }

        public IActionResult AddUser(UserEditor model)
        {
            var team = _context.Teams.Include(t => t.Project).SingleOrDefault(t => t.Name == model.SelectedTeam);
            if (model.UserId == null)
            {
                var newUser = new User
                {
                    Name = model.UserName,
                    Team = team,
                    Project = team?.Project,
                    Course = model.UserCourse,
                    Group = model.UserGroup
                };
                _context.Users.Add(newUser);
            }
            else
            {
                var userToUpdate = _context.Users.Find(model.UserId);
                userToUpdate.Name = model.UserName;
                userToUpdate.Team = team;
                userToUpdate.Project = team?.Project;
                userToUpdate.Course = model.UserCourse;
                userToUpdate.Group = model.UserGroup;
                _context.Update(userToUpdate);
            }
            _context.SaveChanges();
            return RedirectToAction("AllUsers", "Users");
        }


        public IActionResult DeleteUser(int id)
        {
            var userToRemove = _context.Users.Include(u => u.Team).Include(u => u.Project).SingleOrDefault(u => u.Id == id);
            _context.Users.Remove(userToRemove);
            _context.SaveChanges();
            return RedirectToAction("AllUsers", "Users");
        }

        public FileResult DownloadFile()
        {
            var info = new StringBuilder();
            var header = "ФИО\tКоманда\tПроект\tНаправление\tКуратор";
            info.AppendLine(header);
            var users = _context.Users
                .Include(u => u.Project)
                .Include(u => u.Team)
                .OrderBy(u => u.Name);
            foreach (var student in users)
            {
                var line = $"{student.Name}\t{student.Team?.Name}\t{student.Project?.Name}\t{student.Project?.Theme}\t{student.Project?.Curator}";
                info.AppendLine(line);
            }
            byte[] data = Encoding.UTF8.GetBytes(info.ToString());
            info.Length = 0;
            var today = DateTime.Today.ToString("dd.MM.yy");
            return File(data, "text/tsv;charset=utf-8", $"Студенты({today}).tsv");
        }

        [HttpPost]
        public JsonResult UserNameCheck(string userName, int? UserId)
        {
            return Json(userName != null && (!_context.Users.Select(u => u.Name).Contains(userName) || UserId != null));
        }

        public void TempDataMessage(string key, string alert, string value)
        {
            TempData.Remove(key);
            TempData.Add(key, value);
            TempData.Add("alertType", alert);
        }
    }
}
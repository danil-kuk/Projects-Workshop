using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Projects_Workshop.Models.Entities;

namespace Projects_Workshop.Models.ViewModels
{
    public class UserEditor
    {
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Введите ФИО студента")]
        [Remote("UserNameCheck", "Users", HttpMethod = "Post", ErrorMessage = "Такой студент уже добавлен!", AdditionalFields = nameof(UserId))]
        public string UserName { get; set; }
        public string SelectedTeam { get; set; }
        public string SelectedProject { get; set; }
        public string UserCourse { get; set; }
        public string UserGroup { get; set; }

        public SelectList AllTeams { get; set; }
    }
}

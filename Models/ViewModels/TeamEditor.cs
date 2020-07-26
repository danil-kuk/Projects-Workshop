using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Projects_Workshop.Models.ViewModels
{
    public class TeamEditor
    {
        public int? TeamId { get; set; }
        [Required(ErrorMessage = "Введите название команды")]
        [Remote("TeamNameCheck", "Teams", HttpMethod = "Post", ErrorMessage = "Такая команда уже добавлена!", AdditionalFields = nameof(TeamId))]
        public string TeamName { get; set; }
        public string SelectedProject { get; set; }

        public SelectList AllProjects { get; set; }
    }
}

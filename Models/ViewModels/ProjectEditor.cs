using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Projects_Workshop.Models.ViewModels
{
    public class ProjectEditor
    {
        public int? ProjectId { get; set; }
        [Required(ErrorMessage = "Введите название проекта")]
        [Remote("ProjectNameCheck", "Projects", HttpMethod = "Post", ErrorMessage = "Такой проект уже добавлен!", AdditionalFields = nameof(ProjectId))]
        public string ProjectName { get; set; }
        public string Theme { get; set; }
        public string Curator { get; set; }
        [DataType(DataType.MultilineText)]
        public string ShortDescription { get; set; }
        [DataType(DataType.MultilineText)]
        public string GoalDescription { get; set; }
        [DataType(DataType.MultilineText)]
        public string ProblemDescription { get; set; }
        [DataType(DataType.MultilineText)]
        public string RolesDescription { get; set; }
        [DataType(DataType.MultilineText)]
        public string ResultDescription { get; set; }
        [DataType(DataType.MultilineText)]
        public string CustomerOrganization { get; set; }
        [DataType(DataType.MultilineText)]
        public string CustomerContacts { get; set; }
        public SelectList AllTeams { get; set; }

        public List<string> SelectedTeams { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Projects_Workshop.Models.Entities
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
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

        public virtual ICollection<Team> Teams { get; set; }
        public virtual ICollection<User> Participants { get; set; }

    }
}

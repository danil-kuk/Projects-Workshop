using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Projects_Workshop.Models.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Course { get; set; }
        public string Group { get; set; }
        public virtual Team Team { get; set; }
        public virtual Project Project { get; set; }
    }
}

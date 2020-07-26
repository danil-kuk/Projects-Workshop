using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Projects_Workshop.Models.Entities
{
    public class Team
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<User> Members { get; set; } = new HashSet<User>();
    }
}

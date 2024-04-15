using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportFIT.Models
{
    public class Permiso
    {
        [Key]
        public int IdPermiso { get; set; }
        public string NombrePermiso { get; set; }
        public virtual ICollection<Rol> Roles { get; set; }
    }
}

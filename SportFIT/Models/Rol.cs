using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportFIT.Models
{
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }
        public string NombreRol { get; set; }

        public virtual ICollection<Permiso> Permisos { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}

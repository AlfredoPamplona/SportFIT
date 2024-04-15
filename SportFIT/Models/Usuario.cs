using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportFIT.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string ApellidosUsuario { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        [ForeignKey("Rol")]
        public int IdRol { get; set; }
        public virtual Rol Rol { get; set; }

        public virtual ICollection<Pueblo> Pueblos { get; set; }
        public virtual ICollection<Reserva> Reservas { get; set; }
    }
}

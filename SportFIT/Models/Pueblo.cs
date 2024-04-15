using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportFIT.Models
{
    public class Pueblo
    {
        [Key]
        public int IdPueblo { get; set; }
        public string NombrePueblo { get; set; }
        public string CodPostal { get; set; }

        public virtual ICollection<Instalacion> Instalaciones { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}

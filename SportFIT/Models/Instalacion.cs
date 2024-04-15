using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportFIT.Models
{
    public class Instalacion
    {
        [Key]
        public int IdInstalacion { get; set; }
        public string NombreInstalacion { get; set; }
        public string Tipo { get; set; }
        public string Direccion { get; set; }

        [ForeignKey("Pueblo")]
        public int IdPueblo { get; set; }
        public virtual Pueblo Pueblo { get; set; }

        public virtual ICollection<Reserva> Reservas { get; set; }
        public virtual ICollection<Actividad> Actividades { get; set; }
    }
}

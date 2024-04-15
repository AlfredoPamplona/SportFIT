using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportFIT.Models
{
    public class Reserva
    {
        [Key]
        public int IdReserva { get; set; }
        public DateTime FechaReserva { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public int Duracion { get; set; }

        [ForeignKey("Usuario")]
        public int IdUsuario { get; set; }
        public virtual Usuario Usuario { get; set; }

        [ForeignKey("Instalacion")]
        public int IdInstalacion { get; set; }
        public virtual Instalacion Instalacion { get; set; }
    }
}

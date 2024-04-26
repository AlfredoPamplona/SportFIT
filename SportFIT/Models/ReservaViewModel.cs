using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportFIT.Models
{
    public class ReservaViewModel
    {
        public int IdReserva { get; set; }
        public string NombreUsuario { get; set; }
        public DateTime FechaReserva { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public int Duracion { get; set; }
        public string NombreActividad { get; set; }
        public string NombreInstalacion { get; set; }
    }

}

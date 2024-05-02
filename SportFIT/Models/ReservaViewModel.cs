using System;

namespace SportFIT.Models
{
    public class ReservaViewModel
    {
        public int IdReserva { get; set; }
        public string NombreUsuario { get; set; }
        public DateTime FechaReserva { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan Duracion { get; set; }
        public string NombreInstalacion { get; set; }
    }

}

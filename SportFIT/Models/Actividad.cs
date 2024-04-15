using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportFIT.Models
{
    public class Actividad
    {
        [Key]
        public int IdActividad { get; set; }
        public string NombreActividad { get; set; }

        [ForeignKey("Instalacion")]
        public int IdInstalacion { get; set; }
        public virtual Instalacion Instalacion { get; set; }
    }
}

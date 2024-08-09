using System.Net;
using System.Xml;

namespace APIcontrolAsistencia.Models
{
    public class usuario
    {
        public int DNI { get; set; }

        public string CONTRASENA { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public String Telefono { get; set; }
        public DateTime InicioDePracticas { get; set; }
        public int DepartmentID { get; set; }
        public int TipoDeUsuario { get; set; }

    }
}

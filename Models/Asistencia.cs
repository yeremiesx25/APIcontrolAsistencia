namespace APIcontrolAsistencia.Models
{
    public class Asistencia
    {

        public int AttendanceID { get; set; }
        public int ID_PRACTICANTE { get; set; }
        public DateTime FECHA { get; set; }
        public string ENTRADA { get; set; }
        public string SALIDA { get; set; }
        public string ESTADO { get; set; }
        public int Contador { get; set; }

        public string Nombres { get; set; }
        public string Apellidos { get; set; }
    }
}

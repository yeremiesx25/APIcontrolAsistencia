using APIcontrolAsistencia.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace APIcontrolAsistencia.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class MarcarAsistenciaController : ControllerBase
    {
        private readonly string cadenaSQL;

        public MarcarAsistenciaController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");//conexion con la cadena
        }

        [HttpGet]//obtener
        [Route("MsAsistencia")]//Aqui se mostrara usuario
        public IActionResult MostrarUsuario()
        {
            List<Asistencia> asistenciaLista = new List<Asistencia>();

            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {//conectando a la base de datos
                    conexion.Open();//iniciando
                    var cmd = new SqlCommand("ListaAsistencia", conexion);//llamando al store
                    cmd.CommandType = CommandType.StoredProcedure;//diciendo que es un store
                    using (var reader = cmd.ExecuteReader())
                    {//leer el store
                        while (reader.Read())
                        {
                            asistenciaLista.Add(new Asistencia()
                            {
                                    Contador = Convert.ToInt32(reader["DIAS"]),
                                    ID_PRACTICANTE = Convert.ToInt32(reader["DNI"]),//tiene que leer lo que sale en la tabla al mostrar
                                    Nombres = reader["NOMBRES"].ToString(),
                                    Apellidos = reader["APELLIDOS"].ToString(),
                                    FECHA = Convert.ToDateTime(reader["FECHA"]),
                                    ENTRADA = Convert.ToDateTime(reader["ENTRADA"]).ToString("HH:mm")
                            });
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Mostrar Lista", response = asistenciaLista });

            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = asistenciaLista });

            }
        }


        //Crear Asistencia
        [HttpPost]//MANDAR
        [Route("MrcAsistencia")]
        public IActionResult MrcAsistencia([FromBody] Asistencia objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {//conectando a la base de datos
                    conexion.Open();//iniciando
                    var cmd = new SqlCommand("SP_MarcarAsistencia", conexion);//llamando al store
                    cmd.Parameters.AddWithValue("ID_PRACTICANTE", objeto.ID_PRACTICANTE);//dando que la instancia se llama con el objeto
                    cmd.CommandType = CommandType.StoredProcedure;//diciendo que es un store
                    cmd.ExecuteNonQuery();
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Agregado Correctamente" });

            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });

            }
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using APIcontrolAsistencia.Models;

using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace APIcontrolAsistencia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly string cadenaSQL;
      
        public UsuarioController(IConfiguration config) {
            cadenaSQL = config.GetConnectionString("CadenaSQL");//conexion con la cadena
        }

        [HttpGet]//obtener
        [Route("Usuario")]
        public IActionResult Usuario() { 
            List<usuario> usuarioLista = new List<usuario>();
            try {
                using (var conexion = new SqlConnection(cadenaSQL)) {//conectando a la base de datos
                    conexion.Open();//iniciando
                    var cmd = new SqlCommand("SP_USUARIO", conexion);//llamando al store
                    cmd.CommandType = CommandType.StoredProcedure;//diciendo que es un store
                    using (var reader = cmd.ExecuteReader()) {//leer el store
                        while (reader.Read()) {
                            usuarioLista.Add(new usuario() {
                                DNI = Convert.ToInt32(reader["DNI"]),
                                CONTRASENA = reader["CONTRASENA"].ToString(),
                                Nombres = reader["Nombres"].ToString(),
                                Apellidos = reader["Apellidos"].ToString(),
                                Email = reader["Email"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                InicioDePracticas = Convert.ToDateTime(reader["InicioDePracticas"]),
                                TipoDeUsuario = Convert.ToInt32(reader["TipoDeUsuario"])
                            });
                        }
                    }
                }
                return StatusCode(StatusCodes.Status200OK, new {mensaje = "ok", response = usuarioLista});
            
            }catch(Exception error) {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = usuarioLista });

            }
        }
    }
}

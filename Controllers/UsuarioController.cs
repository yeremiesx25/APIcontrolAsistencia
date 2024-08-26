using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using APIcontrolAsistencia.Models;

using System.Data;
using System.Data.SqlClient;
using System.Net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace APIcontrolAsistencia.Controllers
{
    [EnableCors("ReglasCors")]
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly string cadenaSQL;
      
        public UsuarioController(IConfiguration config) {
            cadenaSQL = config.GetConnectionString("CadenaSQL");//conexion con la cadena
        }

        [HttpGet]//obtener
        [Route("MostrarUsuario")]//Aqui se mostrara usuario
        public IActionResult MostrarUsuario() { 
            List<usuario> usuarioLista = new List<usuario>();
            try {
                using (var conexion = new SqlConnection(cadenaSQL)) {//conectando a la base de datos
                    conexion.Open();//iniciando
                    var cmd = new SqlCommand("SP_USUARIO", conexion);//llamando al store
                    cmd.CommandType = CommandType.StoredProcedure;//diciendo que es un store
                    using (var reader = cmd.ExecuteReader()) {//leer el store
                        while (reader.Read()) {
                            usuarioLista.Add(new usuario() {
                                DNI = Convert.ToInt32(reader["DNI"]),//tiene que leer lo que sale en la tabla al mostrar
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
                return StatusCode(StatusCodes.Status200OK, new {mensaje = "Mostrar Lista", response = usuarioLista});
            
            }catch(Exception error) {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = usuarioLista });

            }
        }


        //buscar usuario
        [HttpGet]//obtener
        [Route("BuscarUsuario/{DNI:int}")]
        public IActionResult BuscarUsuario(int DNI)
        {
            List<usuario> usuarioLista = new List<usuario>();
            usuario usuarioBsq = new usuario();
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {//conectando a la base de datos
                    conexion.Open();//iniciando
                    var cmd = new SqlCommand("SP_USUARIO", conexion);//llamando al store
                    cmd.CommandType = CommandType.StoredProcedure;//diciendo que es un store
                    using (var reader = cmd.ExecuteReader())
                    {//leer el store
                        while (reader.Read())
                        {
                            usuarioLista.Add(new usuario()
                            {
                                DNI = Convert.ToInt32(reader["DNI"]),//tiene que leer lo que sale en la tabla al mostrar
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
                usuarioBsq = usuarioLista.Where(item => item.DNI == DNI).FirstOrDefault();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = usuarioBsq });

            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message, response = usuarioBsq });

            }
        }

        //Crear Usuario
        [HttpPost]//MANDAR
        [Route("CrearUsuario")]
        public IActionResult CrearUsuario([FromBody] usuario objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {//conectando a la base de datos
                    conexion.Open();//iniciando
                    var cmd = new SqlCommand("SP_CrearUsuario", conexion);//llamando al store
                    cmd.Parameters.AddWithValue("DNI", objeto.DNI);//dando que la instancia se llama con el objeto
                    cmd.Parameters.AddWithValue("CONTRASENA", objeto.CONTRASENA);
                    cmd.Parameters.AddWithValue("Nombres", objeto.Nombres);
                    cmd.Parameters.AddWithValue("Apellidos", objeto.Apellidos);
                    cmd.Parameters.AddWithValue("Email", objeto.Email);
                    cmd.Parameters.AddWithValue("Telefono", objeto.Telefono);
                    cmd.Parameters.AddWithValue("DepartmentID", objeto.DepartmentID);
                    cmd.Parameters.AddWithValue("TipoDeUsuario", objeto.TipoDeUsuario);
                    cmd.CommandType = CommandType.StoredProcedure;//diciendo que es un store
                    
                    cmd.ExecuteNonQuery();
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Agregado Correctamente"});

            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message});

            }
        }

        //EDITAR EL USUARIO
        [HttpPut]//MANDAR
        [Route("EditarUsuario")]
        public IActionResult EditarUsuario([FromBody] usuario objeto)
        {
            try
            {
                using (var conexion = new SqlConnection(cadenaSQL))
                {//conectando a la base de datos
                    conexion.Open();//iniciando
                    var cmd = new SqlCommand("SP_EditarUsuario", conexion);//llamando al store
                    cmd.Parameters.AddWithValue("DNI", objeto.DNI);
                    cmd.Parameters.AddWithValue("CONTRASENA", objeto.CONTRASENA is null ? DBNull.Value : objeto.CONTRASENA);//si manda vacio se quedara con el valor que ya tiene
                    cmd.Parameters.AddWithValue("Nombres", objeto.Nombres is null ? DBNull.Value : objeto.Nombres);
                    cmd.Parameters.AddWithValue("Apellidos", objeto.Apellidos is null ? DBNull.Value : objeto.Apellidos);
                    cmd.Parameters.AddWithValue("Email", objeto.Email is null ? DBNull.Value : objeto.Email);
                    cmd.Parameters.AddWithValue("Telefono", objeto.Telefono is null ? DBNull.Value : objeto.Telefono);
                    cmd.Parameters.AddWithValue("DepartmentID", objeto.DepartmentID == 0 ? DBNull.Value : objeto.DepartmentID);
                    cmd.Parameters.AddWithValue("TipoDeUsuario", objeto.TipoDeUsuario == 0 ? DBNull.Value : objeto.TipoDeUsuario);
                    cmd.CommandType = CommandType.StoredProcedure;//diciendo que es un store

                    cmd.ExecuteNonQuery();//lee y ejecuta el PROCEDURE
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Editado Correctamente" });

            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });

            }
        }


    }
}

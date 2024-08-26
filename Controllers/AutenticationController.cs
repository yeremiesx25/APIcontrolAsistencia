using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using APIcontrolAsistencia.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Data.SqlClient;
using System.Data;
using System.Text;

namespace APIcontrolAsistencia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticationController : ControllerBase
    {
        private readonly string cadenaSQL;
        private readonly string secretKey;
        public AutenticationController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");//conexion con la cadena
            secretKey = config.GetSection("settings").GetSection("secretkey").ToString();
        }

        [HttpPost]
        [Route("Validar")]
        public IActionResult Validar([FromBody] usuario request)
        {
            using (var conexion = new SqlConnection(cadenaSQL)) // Usar SqlConnection para SQL Server
            {
                conexion.Open();
                var command = new SqlCommand("SP_ValidarUsuario", conexion);

                command.Parameters.AddWithValue("pDNI", request.DNI); 
                command.Parameters.AddWithValue("pClave", request.CONTRASENA);
                command.CommandType = CommandType.StoredProcedure;

                using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var usuarioValido = Convert.ToInt32(reader["UsuarioValido"]);
                            if (usuarioValido > 0)
                            {
                                var KeyBytes = Encoding.ASCII.GetBytes(secretKey);
                                var claims = new ClaimsIdentity();

                                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier,request.DNI.ToString()));
                                var TokenDescriptor = new SecurityTokenDescriptor
                                {
                                    Subject = claims,
                                    Expires = DateTime.UtcNow.AddMinutes(5),
                                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(KeyBytes)
                                    ,SecurityAlgorithms.HmacSha256Signature)
                                };

                                var tokenHandler = new JwtSecurityTokenHandler();
                                var tokenConfig = tokenHandler.CreateToken(TokenDescriptor);

                                //obtener token creado
                                string tokenCreado = tokenHandler.WriteToken(tokenConfig);

                                return StatusCode(StatusCodes.Status200OK, new { token = tokenCreado });
                            }
                            else
                            {
                                return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
                            }
                        }
                        else
                        {
                            return Unauthorized(new { message = "Usuario o clave incorrectos" });
                        }
                    }
                
            }
        }
    }
}

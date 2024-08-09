using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIcontrolAsistencia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly string cadenaSQL;
      
        public UsuarioController(IConfiguration config) {
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }

        [HttpGet]

    }
}

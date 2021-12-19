using Azersun.Audit.Utilities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskManager.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CryptographerController : ControllerBase
    {
        [HttpGet]
        public string Encrypt(string data)
        {
            return Cryptographer.Encrypt(data);
        }

        [HttpGet]
        public string Decrypt(string data)
        {
            return Cryptographer.Decrypt(data);
        }
    }
}

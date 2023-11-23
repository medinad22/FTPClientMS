using FTPClientMS.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FTPClientMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FTPController : ControllerBase
    {

        private readonly IFTPClientService _ftpClientService;

        public FTPController(IFTPClientService ftpClientService)
        {
            _ftpClientService = ftpClientService;
        }

        [HttpPost(Name = "CreateFtp")]
        public async Task Post()
        {
            await _ftpClientService.Download();

        }
    }
}


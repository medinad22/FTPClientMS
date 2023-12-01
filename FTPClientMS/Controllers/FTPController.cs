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

        [HttpGet(Name = "ListFiles")]
        public async Task Get()
        {
            await _ftpClientService.ListFiles3();

        }

        [HttpPost(Name = "DownloadFiles")]
        public async Task Post()
        {
            await _ftpClientService.DownloadFiles();

        }

    }
}


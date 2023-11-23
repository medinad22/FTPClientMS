using FTPClientMS.Interfaces;
using FluentFTP;
namespace FTPClientMS.Services
{
    public class FTPClientService : IFTPClientService
    {

        public async Task Download()
        {
            var token = new CancellationToken();
            var ftpClient = new AsyncFtpClient("ftp.gnu.org", "anonymous", "", 21);

            await ftpClient.AutoConnect();

            foreach (var item in await ftpClient.GetListing("/gnu/chess/"))
            {
                switch (item.Type)
                {
                    case FtpObjectType.Directory:
                        Console.WriteLine("Directory!  " + item.FullName);
                        Console.WriteLine("Modified date:  " + await ftpClient.GetModifiedTime(item.FullName, token));

                        break;

                    case FtpObjectType.File:

                        Console.WriteLine("File!  " + item.FullName);
                        Console.WriteLine("File size:  " + await ftpClient.GetFileSize(item.FullName, -1, token));
                        Console.WriteLine("Modified date:  " + await ftpClient.GetModifiedTime(item.FullName, token));
                        Console.WriteLine("Chmod:  " + await ftpClient.GetChmod(item.FullName, token));

                        break;

                    case FtpObjectType.Link:
                        break;
                }
            };
        }

    }
}

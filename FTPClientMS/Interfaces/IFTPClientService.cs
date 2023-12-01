namespace FTPClientMS.Interfaces
{

    public interface IFTPClientService
    {
        Task DownloadFiles();
        Task ListFiles();
        Task ListFiles2();
        Task ListFiles3();
    }
}

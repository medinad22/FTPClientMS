namespace FTPClientMS.Interfaces
{

    public interface IFTPClientService
    {
        Task DownloadFiles();
        Task ListFiles();
    }
}

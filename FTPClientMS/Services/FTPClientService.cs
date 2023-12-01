using FTPClientMS.Interfaces;
using FluentFTP;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO.Compression;
using FluentFTP.Helpers;
using System.IO;

namespace FTPClientMS.Services
{
    public class FTPClientService : IFTPClientService
    {
        public async Task DownloadFiles()
        {
            var token = new CancellationToken();
            var ftpClient = new AsyncFtpClient("ftp.gnu.org", "anonymous", "", 21);

            await ftpClient.AutoConnect();

            FtpListItem[] ftpListItems = await ftpClient.GetListing("/gnu/chess/");

            FtpListItem ftpListItem = ftpListItems[0];

            Console.WriteLine(ftpListItem.Name);
            Console.WriteLine(ftpListItem.FullName);
            MemoryStream ms = new MemoryStream();
            await ftpClient.DownloadStream(ms, ftpListItem.FullName);
            ms.Position = 0;

            StreamReader sr = new StreamReader(ms);

            string fileContents = await sr.ReadToEndAsync();

            Console.WriteLine(fileContents);
            var config = new AmazonS3Config()
            {
                ServiceURL = "http://s3.localhost.localstack.cloud:4566"
            };
            var s3client = new AmazonS3Client(config);
            var objectRequest = new PutObjectRequest()
            {
                BucketName = "my-first-bucket",
                Key = ftpListItem.Name,
                InputStream = sr.BaseStream
            };

            await s3client.PutObjectAsync(objectRequest);
            sr.Close();
            ms.Close();

        }

        public async Task ListFiles()
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

        public async Task ListFiles2()
        {
            var token = new CancellationToken();
            var ftpClient = new AsyncFtpClient("127.0.0.1", "adminuser", "adminpass", 21);

            await ftpClient.AutoConnect();


            foreach (FtpListItem item in await ftpClient.GetListing("/"))
            {
                MemoryStream ms = new MemoryStream();
                await ftpClient.DownloadStream(ms, item.FullName);

                ZipArchive zipArchives = new ZipArchive(ms, ZipArchiveMode.Read, true);


                var entries = zipArchives.Entries;
                var entry = entries[0];

                Console.WriteLine(entry.Name);
                StreamReader sr = new StreamReader(ms);

                var config = new AmazonS3Config()
                {
                    ServiceURL = "http://s3.sa-east-1.localhost.localstack.cloud:4566"
                };
                var s3client = new AmazonS3Client(config);
                var objectRequest = new PutObjectRequest()
                {
                    BucketName = "my-first-bucket",
                    Key = entry.Name,
                    InputStream = sr.BaseStream

                };

                await s3client.PutObjectAsync(objectRequest);



            }


        }

        public async Task ListFiles3()
        {

            var config = new AmazonS3Config()
            {
                ServiceURL = "http://s3.sa-east-1.localhost.localstack.cloud:4566"
            };
            var s3client = new AmazonS3Client(config);

            var token = new CancellationToken();
            var ftpClient = new AsyncFtpClient("127.0.0.1", "adminuser", "adminpass", 21);

            await ftpClient.AutoConnect();


            foreach (FtpListItem item in await ftpClient.GetListing("/"))
            {
                MemoryStream ms = new MemoryStream();
                await ftpClient.DownloadStream(ms, item.FullName);

                ZipArchive zipArchives = new ZipArchive(ms, ZipArchiveMode.Read, true);

                foreach (var entry in zipArchives.Entries)
                {
                    MemoryStream ms2 = new MemoryStream();
                    var stream = entry.Open();
                    stream.CopyTo(ms2);
                    var objectRequest = new PutObjectRequest()
                    {
                        BucketName = "my-first-bucket",
                        Key = entry.Name,
                        InputStream = ms2
                    };

                    await s3client.PutObjectAsync(objectRequest);
                    ms2.Close();
                }
                ms.Close();
            }


        }
    }
}

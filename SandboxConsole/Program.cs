using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Text;

namespace SandboxConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var connString = Environment.GetEnvironmentVariable("TrashPandaConn", EnvironmentVariableTarget.Machine);

            BlobServiceClient serviceClient = new(connString);

            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient("main");

            // BlobContainerClient containerClient = serviceClient.CreateBlobContainer("main"); // create

            BlobClient client = containerClient.GetBlobClient("tideChirpCounter");

            int count = int.Parse(client.DownloadContent().Value.Content.ToString());
            Console.WriteLine($"Count before modifying: {count}");

            count++;
            // client.put
            // client.Upload(BinaryData.FromString(count.ToString()));
            client.Upload(BinaryData.FromString(count.ToString()), new BlobUploadOptions());
            // client.Upload("", new BlobUploadOptions() {  });
            Console.WriteLine($"Count after modifying: {count}");

            //Console.WriteLine(downloaded.Value.Content.ToString());

            /*
            string testString = "1234";
            byte[] testArr = Encoding.ASCII.GetBytes(testString);
            MemoryStream testStream = new(testArr);

            // Back to string from stream?:
            // var res = Encoding.UTF8.GetString(stream.GetBuffer(), 0 , (int)stream.Length)


            // TODO: make a string, convert it to a system.io.stream, and send it via client.Upload() and see what we get
            var result = client.Upload(testStream);

            var downloaded = client.DownloadContent();
            */
            // string containerName = ""


        }
    }
}

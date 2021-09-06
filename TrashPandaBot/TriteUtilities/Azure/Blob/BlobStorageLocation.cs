using Azure.Storage.Blobs;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriteUtilities.Azure.Blob
{
    public class BlobStorageLocation
    {
        private Option<BlobClient> _client = None;

        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public string BlobName { get; set; }

        public BlobStorageLocation()
        {

        }

        public BlobStorageLocation(string connectionString, string containerName, string blobName)
        {
            ConnectionString = connectionString;
            ContainerName = containerName;
            BlobName = blobName;
        }

        public BlobClient Client
        {
            get => _client.IfNone(() => GenerateNewClient());
        }

        internal BlobClient GenerateNewClient() =>
            new BlobServiceClient(ConnectionString)
                .GetBlobContainerClient(ContainerName)
                .GetBlobClient(BlobName);

        public static BlobClient GetClient(string connectionString, string containerName, string blobName) =>
            new BlobStorageLocation(connectionString, containerName, blobName).Client;
    }
}

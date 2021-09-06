using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using TriteUtilities.Extensions;
using System.Configuration;

namespace TriteUtilities.Azure.Blob
{
    public class BlobStorageService
    {
        private readonly ILogger<BlobStorageService> _logger;

        private BlobStorageOptions _options;

        private static Dictionary<string, string> _connStrings;

        public BlobStorageService(ILogger<BlobStorageService> logger, IOptions<BlobStorageOptions> options)
        {
            _logger = logger;
            _options = options.Value;

            GatherConnectionStrings();
        }

        internal static BlobClient GenerateNewClient(string connString, string containerName, string blobName) =>
            new BlobServiceClient(connString)
                .GetBlobContainerClient(containerName)
                .GetBlobClient(blobName);

        internal static Option<BlobClient> GenerateNewClient(Option<string> connString, string containerName, string blobName) =>
            connString.Map(c => GenerateNewClient(c, containerName, blobName));

        public static bool WriteBlob(string connName, string containerName, string blobName, string blobValue) =>
            GetConnString(connName)
                .Map(c => GenerateNewClient(c, containerName, blobName))
                .Map(c =>
                {
                    c.Upload(BinaryData.FromString(blobValue), new BlobUploadOptions());
                    return true;
                })
                .IfNone(() => false);

        //public static Option<string> ReadBlob(string connName, string containerName, string blobName)
        //{
        //    var connString = GetConnString(connName);
        //    var client = GenerateNewClient(connString, containerName, blobName);

        //    //from connString in GetConnString(connName)
        //    //from client in GenerateNewClient(connString, containerName, blobName)
        //    //from content in client.DownloadContent().Value.Content.ToString()
        //    //select content;
        //    // var test = GenerateNewClient("", containerName, blobName);
        //}

        public static Option<string> ReadBlob(string connName, string containerName, string blobName) =>
            GetConnString(connName)
                .Map(c => GenerateNewClient(c, containerName, blobName))
                .ValidateBlob()
                .Map(c => c.DownloadContent().Value.Content.ToString());

        // public static Option<BlobClient> GetClient
            //client.Map(c => c.Exists().Value)
            //    .Map(b => b ? client : None);

        //{
        //    if (BlobExists(connName, containerName, blobName))
        //    {

        //    }
        //}
        //GetConnString(connName)
        //    .Map(c => GenerateNewClient(c, containerName, blobName))
        //    .Map(c => c.DownloadContent().Value.Content.ToString());

        public static bool BlobExists(string connName, string containerName, string blobName) =>
            GetConnString(connName)
                .Map(c => GenerateNewClient(c, containerName, blobName))
                .Map(c => c.Exists().Value)
                .IfNone(false);

        private void GatherConnectionStrings()
        {
            bool duplicatesFound = false;

            _connStrings = new();

            foreach (string connName in _options.ConnStringNames)
            {
                if (!_connStrings.ContainsKey(connName))
                {
                    string connString = GetConnString(connName)
                        .IfNone(() => throw new ResultIsNullException($"Failed to get Azure Blob storage connection string from {connName}"));
                    _connStrings.Add(connName, connString);
                }
                else
                {
                    duplicatesFound = true;
                }
            }

            if (duplicatesFound)
            {
                _logger.LogWarning("Duplicate connection string names found in config, each entry will only be checked once.");
            }
        }

        internal static Option<string> GetConnString(string connName)
        {
            if (_connStrings is null)
            {
                _connStrings = new();
            }

            if (_connStrings.ContainsKey(connName))
            {
                return _connStrings[connName];
            }
            string connString = GetConnStringRaw(connName);
            return connString is not null
                ? Some(connString)
                : None;
        }

        private static string GetConnStringRaw(string connName)
        {
            var result = Environment.GetEnvironmentVariable(connName);

            if (result is null)
            {
                result = Environment.GetEnvironmentVariable(connName, EnvironmentVariableTarget.Machine);
            }

            if (result is null)
            {
                result = Environment.GetEnvironmentVariable(connName, EnvironmentVariableTarget.Process);
            }

            if (result is null)
            {
                result = Environment.GetEnvironmentVariable(connName, EnvironmentVariableTarget.User);
            }

            if (result is null)
            {
                result = ConfigurationManager.AppSettings[connName];
            }

            if (result is null)
            {
                throw new Exception("what the actual crap man");
            }

            return result;
        }
    }
}

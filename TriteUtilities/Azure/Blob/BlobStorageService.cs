using LanguageExt;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace TriteUtilities.Azure.Blob
{
    public class BlobStorageService
    {
        private readonly string _envVarName;

        private readonly ILogger<BlobStorageService> _logger;

        private BlobStorageOptions _options;

        private string _connString;

        private Dictionary<string, string> _connStrings;

        public BlobStorageService(ILogger<BlobStorageService> logger, IOptions<BlobStorageOptions> options)
        {
            _logger = logger;
            _options = options.Value;

            GatherConnectionStrings();
        }

        private void GatherConnectionStrings()
        {
            bool duplicatesFound = false;

            foreach (string connName in _options.ConnStringNames)
            {
                if (!_connStrings.ContainsKey(connName))
                {
                    string connString = GetConnString()
                        .IfNone(() => throw new ResultIsNullException($"Failed to get Azure Blob storage connection string from {_envVarName}"));
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

        internal Option<string> GetConnString()
        {
            string connString = Environment.GetEnvironmentVariable(_envVarName, EnvironmentVariableTarget.Machine);

            return connString is not null
                ? Some(connString)
                : None;
        }
    }
}

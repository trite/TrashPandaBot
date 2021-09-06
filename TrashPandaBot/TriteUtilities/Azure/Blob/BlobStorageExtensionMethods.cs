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
    public static class BlobStorageExtensionMethods
    {
        public static Option<BlobClient> ValidateBlob(this Option<BlobClient> client)
        {
            bool exists = client.Map(c => c.Exists().Value)
                .IfNone(false);
            return exists
                ? client
                : None;
        }
    }
}

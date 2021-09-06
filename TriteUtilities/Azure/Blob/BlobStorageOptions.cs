using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriteUtilities.Azure.Blob
{
    public class BlobStorageOptions
    {
        public static string configLocation = "triteAzureBlob";

        public List<string> ConnStringNames { get; set; }
    }
}

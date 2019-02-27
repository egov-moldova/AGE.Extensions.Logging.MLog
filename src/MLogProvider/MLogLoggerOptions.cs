using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AGE.Extensions.Logging.MLog
{
    public class MLogLoggerOptions
    {
        public Uri Url { get; set; }

        public string CertificatePath { get; set; }

        public string CertificatePassword { get; set; }

        public Dictionary<string, string> ScopeMappings { get; set; }

        public X509Certificate2 Certificate { get; set; }

        public bool IncludeScopes { get; set; } = true;

        public Func<string, LogLevel, bool> Filter { get; set; }

        public ILogger ErrorLogger { get; set; }

        public Dictionary<string, object> AdditionalFields { get; set; } = new Dictionary<string, object>();

    }
}


using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace AGE.Extensions.Logging.MLog
{
    public class MLogLoggerOptionsPostConfigure : IPostConfigureOptions<MLogLoggerOptions>
    {
        public void PostConfigure(string name, MLogLoggerOptions options)
        {
            if (options.Certificate != null) return;
            options.Certificate = new X509Certificate2(options.CertificatePath, options.CertificatePassword);
        }
    }
}


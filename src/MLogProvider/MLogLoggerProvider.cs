using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MLog.Client.Core;
using System;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;

namespace AGE.Extensions.Logging.MLog
{
    [ProviderAlias("MLog")]
    public class MLogLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly MLogLoggerOptions _options;
        private readonly MLogClient _mlogClient;
        private readonly MLogMessageProcessor _messageProcessor;
        private IExternalScopeProvider externalScopeProvider;
        private readonly ConcurrentDictionary<string, MLogLogger> _loggers = new ConcurrentDictionary<string, MLogLogger>();
        public MLogLoggerProvider(IOptions<MLogLoggerOptions> options , ILogger logger)
        {
            _options = options.Value;

            if (_options == null)
            {
                throw new ArgumentException("MLog options are not instantiated.", nameof(_options));
            }
            
            if (string.IsNullOrEmpty(_options.Url?.AbsoluteUri) )
            {
                throw new ArgumentException("MLog Url are missing.", nameof(options));
            }

            if (_options.Certificate == null)
            {
                _options.Certificate = new X509Certificate2(_options.CertificatePath, _options.CertificatePassword, X509KeyStorageFlags.MachineKeySet);
            }

            _mlogClient = new MLogClient(_options.Url, _options.Certificate, false);
            _messageProcessor = new MLogMessageProcessor(_mlogClient, logger);
        }

        public ILogger CreateLogger(string name)
        {
            return _loggers.GetOrAdd(name, newName => new MLogLogger(newName, _options, _messageProcessor)
            {
                ExternalScopeProvider = externalScopeProvider,
            });
        }

        public void Dispose()
        {
            _messageProcessor.Dispose();
            _mlogClient.Dispose();
        }

        public void SetScopeProvider(IExternalScopeProvider externalScopeProvider)
        {
            this.externalScopeProvider = externalScopeProvider;
        }
    }
}

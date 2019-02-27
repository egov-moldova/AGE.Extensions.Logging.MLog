using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MLog.Client.Core;
using System;
using System.Collections.Concurrent;

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
        public MLogLoggerProvider(IOptions<MLogLoggerOptions> options)
        {
            _options = options.Value;          
            if (string.IsNullOrEmpty(_options.Url?.AbsoluteUri) || _options.Certificate == null )
            {
                throw new ArgumentException("MLog Url or Certificate are missing.", nameof(options));
            }

            _mlogClient = new MLogClient(_options.Url, _options.Certificate, false);
            _messageProcessor = new MLogMessageProcessor(_mlogClient, _options.ErrorLogger);
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

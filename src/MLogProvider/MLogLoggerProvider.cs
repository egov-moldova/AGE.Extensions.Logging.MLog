using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MLog.Client.Core;
using System;
using System.Collections.Concurrent;

namespace AGE.Extensions.Logging.MLog
{
    [ProviderAlias("MLog")]
    public class MLogLoggerProvider : ILoggerProvider
    {
        private readonly MLogLoggerOptions _options;
        private readonly MLogClient _mlogClient;
        private readonly MLogMessageProcessor _messageProcessor;

        private readonly ConcurrentDictionary<string, MLogLogger> _loggers = new ConcurrentDictionary<string, MLogLogger>();

        public MLogLoggerProvider(IOptions<MLogLoggerOptions> options) : this(options.Value)
        {
        }

        public MLogLoggerProvider(MLogLoggerOptions options)
        {
            if (string.IsNullOrEmpty(options.Url?.AbsoluteUri) || options.Certificate == null)
            {
                throw new ArgumentException("MLog Url or Certificate is missing.", nameof(options));
            }
            _options = options;
            _mlogClient = new MLogClient(options.Url, options.Certificate, false);
            _messageProcessor = new MLogMessageProcessor(_mlogClient);
 
        }

        public ILogger CreateLogger(string name)
        {
            return _loggers.GetOrAdd(name, new MLogLogger(name, _options, _messageProcessor));
        }

        public void Dispose()
        {
            _messageProcessor.Dispose();
            _mlogClient.Dispose();
        }
    }
}

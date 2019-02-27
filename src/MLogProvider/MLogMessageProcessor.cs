using Microsoft.Extensions.Logging;
using MLog.Client.Core;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace AGE.Extensions.Logging.MLog
{
    public class MLogMessageProcessor : IDisposable
    {
        private const int _maxQueuedMessages = 1024;

        private static MLogClient _mlogClient;
        private readonly BlockingCollection<string> _messageQueue;
        private readonly Thread _processorThread;
        private ILogger _errorLogger;
        public MLogMessageProcessor(MLogClient mlogClient, ILogger errorLogger)
        {
            _mlogClient = mlogClient;
            _errorLogger = errorLogger;
            _messageQueue = new BlockingCollection<string>(_maxQueuedMessages);
            _processorThread = new Thread(StartAsync)
            {
                IsBackground = true,
                Name = "MLog logger queue processing thread"
            };
            _processorThread.Start();
        }

        public virtual void EnqueueMessage(string message)
        {
            if (!_messageQueue.TryAdd(message))
            {
                _mlogClient.RegisterEvent(message);
            }
        }

        private void StartAsync()
        {
            foreach (var message in _messageQueue.GetConsumingEnumerable())
            {
                try
                {
                    _mlogClient.RegisterEvent(message);
                }
                catch (Exception ex)
                {
                    _errorLogger?.LogCritical(ex, "Error on sending logs to MLog");
                }
            }           
        }

        public void Dispose()
        {
            _messageQueue.CompleteAdding();
            _processorThread.Join(30000);
        }
    }
}

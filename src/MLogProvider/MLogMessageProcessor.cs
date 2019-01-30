using MLog.Client.Core;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace AGE.Extensions.Logging.MLog
{
    public class MLogMessageProcessor : IDisposable
    {
        private readonly MLogClient _mlogClient;
        private readonly BlockingCollection<string> _messageQueue;
        private readonly Thread _processorThread;
        private readonly int _maxQueuedMessages = 1024;
        public MLogMessageProcessor(MLogClient mlogClient)
        {
            _mlogClient = mlogClient;
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
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    _messageQueue.Add(message);
                    return;
                }
                catch (InvalidOperationException) { }
            }

            _mlogClient.RegisterEvent(message);

        }

        private void StartAsync()
        {
            try
            {
                foreach (var message in _messageQueue.GetConsumingEnumerable())
                {
                    _mlogClient.RegisterEvent(message);
                }
            }
            catch
            {
                try
                {
                    _messageQueue.CompleteAdding();
                }
                catch { }
            }
        }

        public void Dispose()
        {
            _messageQueue.CompleteAdding();

            try
            {
                _processorThread.Join(1500);
            }
            catch (ThreadStateException) { }
        }
    }
}

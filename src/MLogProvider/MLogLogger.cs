using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using System.Collections.Generic;
using System;

namespace AGE.Extensions.Logging.MLog
{
    public class MLogLogger : ILogger
    {
        private readonly string _name;
        private readonly MLogLoggerOptions _options;
        private readonly MLogMessageProcessor _messageProcessor;
        public MLogLogger(string name, MLogLoggerOptions options, MLogMessageProcessor messageProcessor)
        {
            _name = name;
            _options = options;
            _messageProcessor = messageProcessor;
        }
        internal IExternalScopeProvider ExternalScopeProvider { get; set; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            var scopeDictionary = new Dictionary<string, object>();
            scopeDictionary.ParseCollection(_options.AdditionalFields, _options.ScopeMappings);
            PopulateScope(scopeDictionary, state);

            if (scopeDictionary.ContainsKey("event_type"))
            {
                _messageProcessor.EnqueueMessage(scopeDictionary.ToJson());
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && _options.Filter?.Invoke(_name, logLevel) != false;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return ExternalScopeProvider != null ? ExternalScopeProvider.Push(state) : NullScope.Instance;
        }

        private void PopulateScope<TState>(Dictionary<string, object> dict, TState state)
        {
            if (_options.IncludeScopes)
            {
                if (state is IReadOnlyCollection<KeyValuePair<string, object>> stateDictionary)
                {
                    dict.ParseCollection(stateDictionary, _options.ScopeMappings);
                }

                if (ExternalScopeProvider != null)
                {
                    ExternalScopeProvider.ForEachScope(
                        (activeScope, builder) =>
                        {
                            if (activeScope is IReadOnlyCollection<KeyValuePair<string, object>> activeScopeDictionary)
                            {
                                dict.ParseCollection(activeScopeDictionary, _options.ScopeMappings);
                            }
                        }, state);
                }

                if (!dict.ContainsKey("event_time"))
                {
                    dict.Add("event_time", DateTime.Now);
                }
            }
        }
    }

    public static class Extensions {

        public static void ParseCollection(this Dictionary<string, object> init, IReadOnlyCollection<KeyValuePair<string, object>> state, Dictionary<string, string> scopeMappings)
        {
            foreach (KeyValuePair<string, object> item in state)
            {
                if (scopeMappings.TryGetValue(item.Key, out var valueName))
                {
                    init.Add(valueName, item.Value);
                }
            }
        }
    }
}

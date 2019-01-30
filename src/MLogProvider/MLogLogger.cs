using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            var newState = _options.AdditionalFields
                .Concat(GetScopeAdditionalFields())
                .Concat(GetStateFields(state)).ToLookup(x => x.Key, x => x.Value);

            if (newState.Contains("event_type"))
            {
                var processedState = new Dictionary<string, object>();
                if (!newState.Contains("event_time"))
                {
                    processedState.Add("event_time", DateTime.Now);
                }
                foreach (var item in newState)
                {
                    if (item.ToArray()[0] == null)
                        continue;
                    if (item.ToArray().Count() < 2)
                        processedState.Add(item.Key, item.ToArray()[0]);
                    else
                        processedState.Add(item.Key, JArray.FromObject(item.ToArray()));
                }
                _messageProcessor.EnqueueMessage(processedState.ToJson());
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && _options.Filter?.Invoke(_name, logLevel) != false;
        }

        private static IDisposable BeginValueTupleScope<T>(ValueTuple<string, T> item)
        {
            return MLogScope.Push(new[]
            {
                new KeyValuePair<string, object>(item.Item1, item.Item2)
            });
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            switch (state)
            {
                case ValueTuple<string, string> s:
                    return BeginValueTupleScope(s);
                case ValueTuple<string, sbyte> sb:
                    return BeginValueTupleScope(sb);
                case ValueTuple<string, byte> b:
                    return BeginValueTupleScope(b);
                case ValueTuple<string, short> sh:
                    return BeginValueTupleScope(sh);
                case ValueTuple<string, ushort> us:
                    return BeginValueTupleScope(us);
                case ValueTuple<string, int> i:
                    return BeginValueTupleScope(i);
                case ValueTuple<string, uint> ui:
                    return BeginValueTupleScope(ui);
                case ValueTuple<string, long> l:
                    return BeginValueTupleScope(l);
                case ValueTuple<string, ulong> ul:
                    return BeginValueTupleScope(ul);
                case ValueTuple<string, float> f:
                    return BeginValueTupleScope(f);
                case ValueTuple<string, double> d:
                    return BeginValueTupleScope(d);
                case ValueTuple<string, decimal> dc:
                    return BeginValueTupleScope(dc);
                case ValueTuple<string, object> o:
                    return BeginValueTupleScope(o);
                case IEnumerable<KeyValuePair<string, object>> additionalFields:
                    return MLogScope.Push(additionalFields);
                default:
                    return null;
            }
        }

        private static IEnumerable<KeyValuePair<string, object>> GetStateFields<TState>(TState state)
        {
            return state is IEnumerable<KeyValuePair<string, object>> logValues
                ? logValues.Take(logValues.Count() - 1)
                : Enumerable.Empty<KeyValuePair<string, object>>();
        }

        private IEnumerable<KeyValuePair<string, object>> GetScopeAdditionalFields()
        {
            var additionalFields = Enumerable.Empty<KeyValuePair<string, object>>();

            if (!_options.IncludeScopes)
            {
                return additionalFields;
            }

            var scope = MLogScope.Current;
            if (scope != null)
            {
                var scopeFields = scope.AdditionalFields.ToDictionary(x => x.Key.ToLower(), x => x.Value);
                scopeFields.Remove("{OriginalFormat}");

                additionalFields = additionalFields.Concat(scopeFields);
            }

            return additionalFields.Reverse();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;

namespace AGE.Extensions.Logging.MLog
{
    public class MLogScope
    {
        private MLogScope(IEnumerable<KeyValuePair<string, object>> additionalFields)
        {
            AdditionalFields = additionalFields;
        }

        public MLogScope Parent { get; private set; }

        public IEnumerable<KeyValuePair<string, object>> AdditionalFields { get; }

        private static readonly AsyncLocal<MLogScope> Value = new AsyncLocal<MLogScope>();

        public static MLogScope Current
        {
            get => Value.Value;
            set => Value.Value = value;
        }

        public static IDisposable Push(IEnumerable<KeyValuePair<string, object>> additionalFields)
        {
            var parent = Current;
            Current = new MLogScope(additionalFields) { Parent = parent };
            return new DisposableScope();
        }

        private class DisposableScope : IDisposable
        {
            public void Dispose()
            {
                Current = Current.Parent;
            }
        }
    }
}

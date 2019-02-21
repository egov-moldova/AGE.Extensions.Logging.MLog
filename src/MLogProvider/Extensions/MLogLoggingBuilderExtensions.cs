using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace AGE.Extensions.Logging.MLog
{
    public static class MLogLoggingBuilderExtensions
    {  
        public static ILoggingBuilder AddMLog(this ILoggingBuilder builder, Action<MLogLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddConfiguration();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<MLogLoggerOptions>, MLogLoggerOptionsPostConfigure>());
            builder.Services.AddSingleton<ILoggerProvider, MLogLoggerProvider>();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
 
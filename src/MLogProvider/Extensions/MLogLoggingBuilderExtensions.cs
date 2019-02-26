using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;

namespace AGE.Extensions.Logging.MLog
{
    public static class MLogLoggingBuilderExtensions
    {  
        public static ILoggingBuilder AddMLog(this ILoggingBuilder builder, ILogger logger = null)
        {
            var serviceProvider = builder.Services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            builder.AddConfiguration();
            builder.Services
                .AddSingleton(loggerFactory)
                .BuildServiceProvider();

            if (logger == null)
            {
                logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<MLogMessageProcessor>();
            }
            loggerFactory.AddProvider(new MLogLoggerProvider(serviceProvider.GetService<IOptions<MLogLoggerOptions>>(), logger));
            return builder;
        }
    }
}
 
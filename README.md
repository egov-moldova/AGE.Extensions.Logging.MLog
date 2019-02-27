# AGE.Extensions.Logging.MLog

This package is intended for logging in MLog by implementing a Logger Provider.
### Getting Started

Please go through the following instructions to add Provider to your project.


### Installing

Install the following package from [NuGet](https://www.nuget.org/packages/AGE.Extensions.Logging.MLog/)

```
Install-Package AGE.Extensions.Logging.MLog	

```

Then follow the instructions from Configuration and Usage sections below.

### Configuration

Add the following configuration section to your **appsettings.json**:
```
{
	...
	"MLogConfig": 
	{
     "Url": "urlToMLogService",
     "CertificatePath": "pathToPrivateCertificate",
     "CertificatePassword": "certPassword"
	}
	...
}
```

### Usage

Add the following code snippet to your **Startup.ConfigureServices** method:
```
	services.AddLogging(builder => builder
			   .AddConfiguration(Configuration)
			   .AddMLog(options =>
			   {
				   Configuration.Bind("Logging:MLogConfig", options);
				   options.ErrorLogger = mlogErrorLogger;
			   }))
```

```

using (logger.BeginScope("{event_type}{service}", "Signature.Success", "testService"))
{
	logger.LogError("Logging with scope => {test}", "scope");
}

logger.LogError("Took place an error with event type: {event_type} with data: {test}", "ERROR", "test");
            
```

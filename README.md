# BasicLogging.NET
BasicLogging.NET is a simple .NET wrapper that makes it a bit easier to add logging to your application. Currently, it is built on top of the popular [log4net](https://logging.apache.org/log4net/) framework, but it can be extended to support other frameworks, such as [NLog](http://nlog-project.org/).

##Why use BasicLogging.NET?
BasicLogging.NET is lightweight and very simple to use. It automatically adds default context information (method, line, source file) to the log records, so you can track them in your log repositories (you don't have to, though). The library makes it easy to set up other logging context properties that may be needed (such as user, transaction, or session information). It also implements common sense checks, such as checking the log level before serializing log objects.

##Which logging frameworks does BasicLogging.NET support?
Currently, BasicLogging.NET only supports log4net, but it can be extended to support other popular frameworks, such as NLog.

##How is BasicLogging.NET similar to log4net?
BasicLogging.NET, is a simple wrapper for log4net (other frameworks may be supported in future). It has a familiar logging workflow and supports the same logging configuration options as an out-of-the-box log4net library (including custom formatters, etc).

##How is BasicLogging.NET different from log4net?
BasicLogging.NET offers a somewhat simpler logging interface with just a couple of methods and enumeration types. It also exposes additional logging context information, such as the immediate method writing a log record, source code path, and line number of the log writer. 

##Why doesn't BasicLogging.NET support log4net's format messages, like DebugFormat?
Instead of optional string formatting parameters used by log4net's format functions (such as DebugFormat, InfoFormat, etc), log writing methods exposed by BasicLogging.NET reserve optional parameters to capture caller context: name of the method making the call, source file and line number. This information can be added to the log records via the `%property{callerMemberName}`, `%property{callerLineNumber}`, and `%property{callerFilePath}` placeholders without any programming on your part (you can customize and completely turn these options off via the `CallerContext` property). For additional information, see [CallerMemberName](https://msdn.microsoft.com/en-us/library/hh551816), [CallerLineNumber](https://msdn.microsoft.com/en-us/library/hh551811), and [CallerFilePath](https://msdn.microsoft.com/en-us/library/hh551818).

##Where is the BasicLogging.NET NuGet package?
Here it is: [https://www.nuget.org/packages/BasicLogging/](https://www.nuget.org/packages/BasicLogging/).

##Where is the BasicLogging.NET documentation file?
You can find the help file at the [latest release downloads](../../releases).

##Example
The following example illustrates the basic use of BasicLogging.NET. 

Assume that we have an MVC application that, in addition to standard info (timestamp, thread ID, message, etc), must write the following to the logs: 

- transaction ID 
- endpoint 
- operation 
- method 
- client ID
- IP address (of the caller) 

*Transaction ID* will be used to identify everything that happens during the invocation of the primary controller *operation* (the topmost method, such as Post), including other called methods (name of each immediate *method* writing to the log will also be captured). In addition, a log record will include an *IP address* of the caller and a *client ID*. 

The log4net configuration in the web.config file may look like this (in the following example only log4net settings are shown): 
```xml
<configuration>
  <configSections>
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net, Version=1.2.11.0, Culture=neutral, PublicKeyToken=669e0ddf0bb1aa2a" />
  </configSections>
  ...
  <log4net>
  ...
  <appender name="RollingFileAppender" type="log4net.Appender.RollingFileAppender">
    <file value="c:\Logs\MyWebApp\MyWebApp.log" />
    <appendToFile value="true" />
    <rollingStyle value="composite" />
    <staticLogFileName value="true" />
    <maxSizeRollBackups value="100" />
    <maximumFileSize value="20MB" />
    <datePattern value=".yyyy-MM-dd" />
    <countDirection value="1" />
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="[%utcdate{ISO8601}] [%thread] [%level] [%property{transaction}] [%property{operation}] [%property{clientIp}] [%property{client}] [%logger{1}] [%property{endpoint}] [%property{callerMemberName}] %message%newline%exception" />
    </layout>
  </appender>            
  </log4net>
  ...
</configuration>
```
The following code shows how the application makes calls to write log messages. 
```csharp
...
using BasicLogging;
...
namespace BasicLoggingDemo
{
    public class UserController: ApiController
    {
        private static ILogger _log = LogManager.GetLogger(typeof(UserController));
        ...
        public void Post(UserInfo user)
        {
            // In the log4net implementation, TRACE messages are mapped to DEBUG messages.
            // In addition to the message, the log entry will includ ethe name of this method,
            // i.e. 'Post'.
            _log.Write(LogLevel.Trace, "Started.");

            // The following properties will be set for the dureation of the logical thread.
            // The will be accessible via the custom properties, e.g. [%property{transaction}].
            _log.SetContext(LogContextType.LogicalThread, "transaction", Guid.NewGuid());
            _log.SetContext(LogContextType.LogicalThread, "endpoint", this.GetType());
            _log.SetContext(LogContextType.LogicalThread, "operation", LogContext.GetMethodName());

            // Use application-specific methods to get client ID and IP address.
            _log.SetContext(LogContextType.LogicalThread, "client", AppHelper.GetClientId());
            _log.SetContext(LogContextType.LogicalThread, "clientIp", AppHelper.GetClientIpAddress());

            // The following and all subsequent log entries will include transaction ID, 
            // endpoint name, operation, client ID and IP address.
            _log.Write(LogLevel.Trace, "Context information is set.");

            try
            {
                _log.Write(LogLevel.Trace, "Calling DoSomething.");
                ...
                DoSomething(user);
            }
            catch (Exception ex)
            {
                _log.Write(LogLevel.Error, "Cannot do something.", ex);

                // Add code to handle exception.
            }

            ...
            _log.Write(LogLevel.Trace, "Ended.");
        }

        private void DoSomething(UserInfo user)
        {
            // The following log entries will include contextual info,
            // including the name of the operation ('Post'), 
            // immediate method ('DoSomething'),
            // endpoint ('BasicLoggingDemo.UserController'),
            // immediate method ('DoSomething'), transaction ID, client ID 
            // and IP address, etc.

            _log.Write(LogLevel.Trace, "Started.");

            try
            {
                ...
                DoSomethingElse();
                ...
            }
            catch(Exception ex)
            {
                _log.Write(LogLevel.Error, "Cannot do something else.", ex);

                // Add code to handle exception.

                throw;
            }

            _log.Write(LogLevel.Trace, "Ended.");                    
        }
    }
}

```

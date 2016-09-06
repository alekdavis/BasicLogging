using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

using log4net;
using log4net.Config;

namespace BasicLogging
{
	/// <summary>
	/// Implements logging using <see href="https://logging.apache.org/log4net/">log4net</see>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class must be <c>sealed</c> because
	/// some methods automatically get (and use) the 
	/// information about the caller (such as method,
	/// source file, line of code), so a derived class
	/// would produce the wrong information.
	/// </para>
	/// <para>
	/// For security reasons, the default implementation only captures 
	/// the caller file name, not the full path,
	/// as part of the caller context info.
	/// To capture full name or make other customizations of the
	/// caller context configuration,
	/// set proper <see cref="CallerContext"/>
	/// flags. 
	/// </para>
	/// </remarks>
	public sealed class Log4NetWrapper: ILogger
	{	
		/// <summary>
		/// Logger instance.
		/// </summary>
        private log4net.ILog _log = null;

		/// <summary>
		/// Caller context to be implicitly captured.
		/// </summary>
		private LogCallerContext _callerContext = 
			LogCallerContext.LineNumber | 
			LogCallerContext.MethodName | 
			LogCallerContext.SourceFileName;

		#pragma warning disable 1573
		/// <inheritdoc cref="BasicLogging.ILogger.CallerContext" select="summary|value"/> 
		#pragma warning restore 1573
		public LogCallerContext CallerContext 
		{  
			get 
			{
				return _callerContext;
			}
			
			set
			{
				_callerContext = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Log4NetWrapper"/> class.
		/// </summary>
		/// <param name="type">
		/// Logger type.
		/// </param>
        public Log4NetWrapper
		(
			Type type
		)
        {
            _log = log4net.LogManager.GetLogger(type);
            XmlConfigurator.Configure();
        }

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="Log4NetWrapper"/> class.
		/// </summary>
		/// <param name="name">
		/// Name of the logger type.
		/// </param>
        public Log4NetWrapper
		(
			string name
		)
        {
            _log = log4net.LogManager.GetLogger(name);
            XmlConfigurator.Configure();
        }

		/// <summary>
		/// Checks if a log record of the specified log level must
		/// not be written to the log.
		/// </summary>
		/// <param name="logLevel">
		/// Log level of the log record.
		/// </param>
		/// <returns>
		/// <c>True</c> if the log record must not be written to the log;
		/// otherwise, false.
		/// </returns>
		private bool IgnoreLogLevel
		(
			LogLevel logLevel
		)
		{
			if ((logLevel == LogLevel.Trace && !_log.IsDebugEnabled) ||
				(logLevel == LogLevel.Debug && !_log.IsDebugEnabled) ||
				(logLevel == LogLevel.Info && !_log.IsInfoEnabled) ||
				(logLevel == LogLevel.Warn && !_log.IsWarnEnabled) ||
				(logLevel == LogLevel.Error && !_log.IsErrorEnabled) ||
				(logLevel == LogLevel.Fatal && !_log.IsFatalEnabled))
				return true;

			return false;
		}

		/// <summary>
		/// Sets the properties of the caller context.
		/// </summary>
		/// <param name="callerMemberName">
		/// Name of the caller method.
		/// Omit to use the default.
		/// </param>
		/// <param name="callerFilePath">
		/// File path of the caller.
		/// Omit to use the default.
		/// </param>
		/// <param name="callerLineNumber">
		/// Line number of the statement making the call.
		/// Omit to use the default.
		/// </param>
		/// <remarks>
		/// <para>
		/// To add custom properties to a log record, use a 
		/// cref="overloads:BasicLogging.Log4NetWrapper.SetContext"/> method before making this call.
		/// </para>
		/// </remarks>
		/// <seealso cref="BasicLogging.ILogger.Write(LogLevel,string,string,string,int)"/>
		private void SetCallerContext
		(
			string	callerMemberName, 
			string	callerFilePath, 
			string	callerLineNumber
		)
		{
			if (_callerContext.HasFlag(LogCallerContext.MethodName))
				log4net.ThreadContext.Properties["callerMemberName"] = callerMemberName;

			if (_callerContext.HasFlag(LogCallerContext.LineNumber))
				log4net.ThreadContext.Properties["callerLineNumber"] = callerLineNumber;

			if (_callerContext.HasFlag(LogCallerContext.SourceFilePath))
				log4net.ThreadContext.Properties["callerFilePath"] = callerFilePath;
			else if (_callerContext.HasFlag(LogCallerContext.SourceFileName))
			{
				log4net.ThreadContext.Properties["callerFilePath"] = 
					String.IsNullOrEmpty(callerFilePath) ? 
						callerFilePath : 
						Path.GetFileName(callerFilePath);
			}
		}

		#pragma warning disable 1573
		/// <inheritdoc cref="BasicLogging.ILogger.SetContext(LogContextType,string,string)" select="overloads|summary|param|remarks"/> 
		#pragma warning restore 1573
		public void SetContext
		(
			LogContextType	contextType,
			string			name,
			object			value
		)
		{
			SetContext(contextType, name, value.ToString());
		}

		#pragma warning disable 1573
		/// <inheritdoc cref="BasicLogging.ILogger.SetContext(LogContextType,string,object)" select="summary|param|remarks"/> 
		#pragma warning restore 1573
		public void SetContext
		(
			LogContextType	contextType,
			string			name,
			string			value
		)
		{
			switch (contextType)
			{
				case LogContextType.Global:
					log4net.GlobalContext.Properties[name] = value;
					break;

				case LogContextType.LogicalThread:
					log4net.LogicalThreadContext.Properties[name] = value;
					break;

				case LogContextType.Thread:
					log4net.ThreadContext.Properties[name] = value;
					break;
			}
		}

		#pragma warning disable 1573
		/// <inheritdoc cref="BasicLogging.ILogger.SetContext{TKey,TValue}(LogContextType,KeyValuePair{TKey,TValue})" select="summary|typeparam|param|remarks"/> 
		public void SetContext<TKey, TValue>
		(
			LogContextType	contextType,
			KeyValuePair<TKey, TValue> property
		)
		{
			SetContext(contextType, property.Key.ToString(), property.Value.ToString());
		}
		#pragma warning restore 1573

		#pragma warning disable 1573
		/// <inheritdoc cref="BasicLogging.ILogger.SetContext(LogContextType,Dictionary{string,string})" select="summary|param|remarks"/> 
		public void SetContext
		(
			LogContextType contextType,
			Dictionary<string, string> properties
		)
		{
			foreach (KeyValuePair<string, string> property in properties)
			{
				SetContext(contextType, property.Key, property.Value);
			}
		}
		#pragma warning restore 1573

		#pragma warning disable 1573
		/// <inheritdoc cref="BasicLogging.ILogger.SetContext(LogContextType,Dictionary{string,object})" select="overloads|summary|param|remarks"/> 
		#pragma warning restore 1573
		public void SetContext
		(
			LogContextType contextType,
			Dictionary<string, object> properties
		)
		{
			foreach (KeyValuePair<string, object> property in properties)
			{
				SetContext<string, object>(contextType, property);
			}
		}
		#pragma warning disable 1573
		/// <inheritdoc cref="BasicLogging.ILogger.SetContext(LogContextType,NameValueCollection)" select="overloads|summary|param|remarks"/> 
		#pragma warning restore 1573
		public void SetContext
		(
			LogContextType contextType,
			NameValueCollection properties
		)
		{
			foreach (string key in properties.Keys)
			{
				SetContext<string, object>(contextType, 
					new KeyValuePair<string, object>(key, properties[key]));
			}
		}

		#pragma warning disable 1573
		/// <inheritdoc cref="BasicLogging.ILogger.Write(LogLevel,string,string,string,int)" select="overloads|summary|param|remarks"/> 
		#pragma warning restore 1573
		public void Write
		(
			LogLevel	logLevel, 
			string		message,
			[CallerMemberName]	string callerMemberName = "", 
			[CallerFilePath]	string callerFilePath	= "", 
			[CallerLineNumber]	int callerLineNumber	= 0
		)
        {
            if (IgnoreLogLevel(logLevel))
               return;

			SetCallerContext(callerMemberName, callerFilePath, callerLineNumber.ToString());

            if (logLevel == LogLevel.Trace)
				_log.Debug(message);
            else if (logLevel == LogLevel.Debug)
				_log.Debug(message);
            else if (logLevel == LogLevel.Info)
                _log.Info(message);
			else if (logLevel == LogLevel.Warn)
                _log.Warn(message);
			else if (logLevel == LogLevel.Error)
                _log.Error(message);
            else if (logLevel == LogLevel.Fatal)
                _log.Fatal(message);

			SetCallerContext(null, null, null);
		}

		#pragma warning disable 1573
		/// <inheritdoc cref="BasicLogging.ILogger.Write(LogLevel,object,string,string,int)" select="summary|param|remarks"/> 
		#pragma warning restore 1573
        public void Write
		(
			LogLevel	logLevel, 
			object		message, 
			[CallerMemberName]	string	callerMemberName	= "", 
			[CallerFilePath]	string	callerFilePath		= "", 
			[CallerLineNumber]	int		callerLineNumber	= 0)
        {
            if (IgnoreLogLevel(logLevel))
               return;

			Write(logLevel, message.ToString(),
				callerMemberName, callerFilePath, callerLineNumber);
		}

		#pragma warning disable 1573
		/// <inheritdoc cref="BasicLogging.ILogger.Write(LogLevel,Exception,string,string,int)" select="summary|param|remarks"/> 
		#pragma warning restore 1573
        public void Write
		(
			LogLevel	logLevel, 
			Exception	ex, 
			[CallerMemberName]	string	callerMemberName	= "", 
			[CallerFilePath]	string	callerFilePath		= "", 
			[CallerLineNumber]	int		callerLineNumber	= 0
		)
        {
            if (IgnoreLogLevel(logLevel))
               return;

			SetCallerContext(callerMemberName, callerFilePath, callerLineNumber.ToString());

            if (logLevel == LogLevel.Trace)
                _log.Debug(ex);
            else if (logLevel == LogLevel.Debug)
                _log.Debug(ex);
            else if (logLevel == LogLevel.Info)
                _log.Info(ex);
			else if (logLevel == LogLevel.Warn)
                _log.Warn(ex);
			else if (logLevel == LogLevel.Error)
                _log.Error(ex);
             else if (logLevel == LogLevel.Fatal)
                _log.Fatal(ex);

			SetCallerContext(null, null, null);
		}

		#pragma warning disable 1573
		/// <inheritdoc cref="BasicLogging.ILogger.Write(LogLevel,string,Exception,string,string,int)" select="summary|param|remarks"/> 
		#pragma warning restore 1573
		public void Write
		(
			LogLevel	logLevel, 
			string		message, 
			Exception	ex, 
			[CallerMemberName]	string	callerMemberName	= "", 
			[CallerFilePath]	string	callerFilePath		= "", 
			[CallerLineNumber]	int		callerLineNumber	= 0
		)
		{
            if (IgnoreLogLevel(logLevel))
               return;

			SetCallerContext(callerMemberName, callerFilePath, callerLineNumber.ToString());

			if (logLevel == LogLevel.Trace)
				_log.Debug(message, ex);
			else if (logLevel == LogLevel.Debug)
				_log.Debug(message, ex);
			else if (logLevel == LogLevel.Info)
				_log.Info(message, ex);
			else if (logLevel == LogLevel.Warn)
				_log.Warn(message, ex);
			else if (logLevel == LogLevel.Error)
				_log.Error(message, ex);
			else if (logLevel == LogLevel.Fatal)
				_log.Fatal(message, ex);

			SetCallerContext(null, null, null);
		}

		#pragma warning disable 1573
		/// <inheritdoc cref="BasicLogging.ILogger.Write(LogLevel,object,Exception,string,string,int)" select="summary|param|remarks"/> 
		#pragma warning restore 1573
		public void Write
		(
			LogLevel	logLevel, 
			object		message, 
			Exception	ex, 
			[CallerMemberName]	string	callerMemberName	= "", 
			[CallerFilePath]	string	callerFilePath		= "", 
			[CallerLineNumber]	int		callerLineNumber	= 0
		)
		{
            if (IgnoreLogLevel(logLevel))
               return;

			Write(logLevel, message.ToString(), ex,
				callerMemberName, callerFilePath, callerLineNumber);
		}
	}
}

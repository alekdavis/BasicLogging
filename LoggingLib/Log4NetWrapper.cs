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
	/// set proper <see cref="BasicLogging.ILogger.CallerContext"/>
	/// flags. 
	/// </para>
	/// </remarks>
	public sealed class Log4NetWrapper: LogWrapper
	{	
		/// <summary>
		/// Logger instance.
		/// </summary>
        private log4net.ILog _log = null;

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
		protected override bool IgnoreLogLevel
		(
			LogLevel logLevel
		)
		{
			if ((logLevel == LogLevel.Trace && !_log.IsDebugEnabled) ||
				(logLevel == LogLevel.Debug && !_log.IsDebugEnabled) ||
				(logLevel == LogLevel.Info  && !_log.IsInfoEnabled)  ||
				(logLevel == LogLevel.Warn  && !_log.IsWarnEnabled)  ||
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
		/// <inheritdoc /> 
		#pragma warning restore 1573
		public override bool IsEnabled
		(
			LogLevel logLevel
		)
		{
			if (_log == null)
				return false;

			switch (logLevel)
			{
				case LogLevel.Debug:
				case LogLevel.Trace:
					return _log.IsDebugEnabled;

				case LogLevel.Error:
					return _log.IsErrorEnabled;

				case LogLevel.Fatal:
					return _log.IsFatalEnabled;

				case LogLevel.Info:
					return _log.IsInfoEnabled;

				case LogLevel.Warn:
					return _log.IsWarnEnabled;
			}

			return false;
		}

		#pragma warning disable 1573
		/// <inheritdoc /> 
		#pragma warning restore 1573
		public override void SetContext
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
		/// <inheritdoc /> 
		/// <overloads>
		/// <inheritdoc /> 
		/// </overloads>
		#pragma warning restore 1573
		public override void Write
		(
			LogLevel	logLevel, 
			string		message,
			[CallerMemberName]
			string		callerMemberName	= "", 
			[CallerFilePath]
			string		callerFilePath		= "", 
			[CallerLineNumber]
			int			callerLineNumber	= 0
		)
        {
            if (IgnoreLogLevel(logLevel))
               return;

			if (message == null)
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
		/// <inheritdoc /> 
		#pragma warning restore 1573
        public override void Write
		(
			LogLevel	logLevel, 
			object		message, 
			[CallerMemberName]
			string		callerMemberName	= "", 
			[CallerFilePath]
			string		callerFilePath		= "", 
			[CallerLineNumber]
			int			callerLineNumber	= 0
		)
        {
            if (IgnoreLogLevel(logLevel))
               return;

			if (message == null)
				return;

			Write(logLevel, message.ToString(),
				callerMemberName, callerFilePath, callerLineNumber);
		}

		#pragma warning disable 1573
		/// <inheritdoc /> 
		#pragma warning restore 1573
        public override void Write
		(
			LogLevel	logLevel, 
			Exception	ex, 
			[CallerMemberName]
			string		callerMemberName	= "", 
			[CallerFilePath]
			string		callerFilePath		= "", 
			[CallerLineNumber]
			int			callerLineNumber	= 0
		)
        {
            if (IgnoreLogLevel(logLevel))
               return;

			if (ex == null)
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
		/// <inheritdoc /> 
		#pragma warning restore 1573
		public override void Write
		(
			LogLevel	logLevel, 
			string		message, 
			Exception	ex, 
			[CallerMemberName]
			string		callerMemberName	= "", 
			[CallerFilePath]
			string		callerFilePath		= "", 
			[CallerLineNumber]
			int			callerLineNumber	= 0
		)
		{
            if (IgnoreLogLevel(logLevel))
               return;

			if (message == null && ex == null)
				return;

			SetCallerContext(callerMemberName, callerFilePath, callerLineNumber.ToString());

			if (ex == null)
			{
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
			}
			else
			{
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
			}

			SetCallerContext(null, null, null);
		}

		#pragma warning disable 1573
		/// <inheritdoc /> 
		#pragma warning restore 1573
		public override void Write
		(
			LogLevel	logLevel, 
			object		message, 
			Exception	ex, 
			[CallerMemberName]	
			string		callerMemberName	= "", 
			[CallerFilePath]	
			string		callerFilePath		= "", 
			[CallerLineNumber]	
			int			callerLineNumber	= 0
		)
		{
            if (IgnoreLogLevel(logLevel))
               return;

			if (message == null && ex == null)
				return;

			Write(logLevel, message.ToString(), ex,
				callerMemberName, callerFilePath, callerLineNumber);
		}
	}
}

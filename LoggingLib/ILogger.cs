using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace BasicLogging
{
	/// <summary>
	/// Defines common log methods.
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Gets or sets the details about the caller context 
		/// (such as calling method or line number)
		/// that must be captured in the logs.
		/// </summary>
		/// <value>
		/// Details about the caller context that must be captured.
		/// </value>
		/// <remarks>
		/// <para>
		/// While this setting dictates what information about 
		/// the caller must be made available to the log formatter,
		/// whether this information is included in the log
		/// depends on the log configuration.
		/// </para>
		/// <para>
		/// For security reasons, it is recommended to only
		/// capture the source file name (not the full path)
		/// in the default logger configuration.
		/// </para>
		/// </remarks>
		LogCallerContext CallerContext { get; set; }

		/// <summary>
		/// Determines whether the specified log level is enabled.
		/// </summary>
		/// <param name="logLevel">
		/// The log level.
		/// </param>
		/// <returns>
		///   <c>true</c> if the specified log level is enabled; otherwise, <c>false</c>.
		/// </returns>
		bool IsEnabled
		(
			LogLevel logLevel
		);

		/// <overloads>
		/// Writes a record to the log.
		/// </overloads>
		/// <summary>
		/// Writes a text message to the log.
		/// </summary>
		/// <param name="logLevel">
		/// Log level of the log record.
		/// </param>
		/// <param name="message">
		/// Log message.
		/// </param>
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
		/// <see cref="O:BasicLogging.ILogger.SetContext"/> method before making this call.
		/// </para>
		/// <para>
		/// When not specified <paramref name="callerMemberName"/>,
		/// <paramref name="callerFilePath"/>,
		/// <paramref name="callerLineNumber"/> values will
		/// reflect the caller information.
		/// Generally, you should not override those.
		/// When using log4net, you can reference them in the log template
		/// via the <c>%property{name}</c> placeholder,
		/// where <c>name</c> is either <c>callerMemberName</c>,
		/// <c>callerFilePath</c>, or <c>callerLineNumber</c>.
		/// For more information about these properties, 
		/// see
		/// <see cref="System.Runtime.CompilerServices.CallerMemberNameAttribute"/>,
		/// <see cref="System.Runtime.CompilerServices.CallerFilePathAttribute"/>, and
		/// <see cref="System.Runtime.CompilerServices.CallerLineNumberAttribute"/>
		/// </para>
		/// </remarks>
		void Write
		(
			LogLevel	logLevel, 
			string		message,
			[CallerMemberName]	string	callerMemberName	= "", 
			[CallerFilePath]	string	callerFilePath		= "", 
			[CallerLineNumber]	int		callerLineNumber	= 0
		);

		#pragma warning disable 1573 
		/// <inheritdoc cref="Write(LogLevel,string,string,string,int)" select="param|remarks"/> 
		/// <summary>
		/// Converts the specified <paramref name="message"/> to a string
		/// and writes it to the log.
		/// </summary>
		/// <param name="message">
		/// Object to be converted to a log message.
		/// </param>
		void Write
		(
			LogLevel	logLevel, 
			object		message, 
			[CallerMemberName]	string	callerMemberName	= "", 
			[CallerFilePath]	string	callerFilePath		= "", 
			[CallerLineNumber]	int		callerLineNumber	= 0
		);
		#pragma warning restore 1573
        
		#pragma warning disable 1573 
		/// <inheritdoc cref="Write(LogLevel,string,string,string,int)" select="param|remarks"/> 
		/// <summary>
		/// Writes an exception to the log.
		/// </summary>
		/// <param name="ex">
		/// Error information.
		/// </param>
		void Write
		(
			LogLevel	logLevel, 
			Exception	ex, 
			[CallerMemberName]	string	callerMemberName	= "", 
			[CallerFilePath]	string	callerFilePath		= "", 
			[CallerLineNumber]	int		callerLineNumber	= 0
		);
		#pragma warning restore 1573
		
		#pragma warning disable 1573 
		/// <inheritdoc cref="Write(LogLevel,string,string,string,int)" select="param|remarks"/> 
		/// <summary>
		/// Writes a text message and an exception to the log.
		/// </summary>
		/// <param name="ex">
		/// Error information.
		/// </param>
		void Write
		(
			LogLevel	logLevel, 
			string		message, 
			Exception	ex, 
			[CallerMemberName]	string	callerMemberName	= "", 
			[CallerFilePath]	string	callerFilePath		= "", 
			[CallerLineNumber]	int		callerLineNumber	= 0
		);
		#pragma warning restore 1573
		
		#pragma warning disable 1573 
		/// <inheritdoc cref="Write(LogLevel,string,Exception,string,string,int)" select="param|remarks"/> 
		/// <summary>
		/// Converts the specified <paramref name="message"/> to a string
		/// and writes it along with an exception to the log.
		/// </summary>
		void Write
		(
			LogLevel	logLevel, 
			object		message, 
			Exception	ex, 
			[CallerMemberName]	string	callerMemberName	= "", 
			[CallerFilePath]	string	callerFilePath		= "", 
			[CallerLineNumber]	int		callerLineNumber	= 0
		);
		#pragma warning restore 1573

		/// <overloads>
		/// Sets values of custom properties that can be logged implicitly.
		/// </overloads>
		/// <summary>
		/// Sets the string value of a custom property that can be logged implicitly.
		/// </summary>
		/// <param name="contextType">
		/// Property accessibility scope.
		/// </param>
		/// <param name="name">
		/// Property name.
		/// </param>
		/// <param name="value">
		/// Property value.
		/// </param>
		/// <remarks>
		/// Custom properties can be referenced in the log templates using
		/// standard logger syntax. For example, for log4net loggers,
		/// they can be referenced as <c>%property{name}</c>, 
		/// where <c>name</c> is the name of the property.
		/// </remarks>
		void SetContext
		(
			LogContextType	contextType,
			string			name,
			string			value
		);

		#pragma warning disable 1573 
		/// <inheritdoc cref="SetContext(LogContextType,string,string)" select="param|remarks"/> 
		/// <summary>
		/// Sets the value of a custom string property that can be logged implicitly.
		/// </summary>
		void SetContext
		(
			LogContextType	contextType,
			string			name,
			object			value
		);
		#pragma warning restore 1573

		#pragma warning disable 1573 
		/// <inheritdoc cref="SetContext(LogContextType,string,string)" select="param|remarks"/> 
		/// <summary>
		/// Sets the values of the custom string properties that can be logged implicitly
		/// using a generic key-value pair array.
		/// </summary>
		/// <typeparam name="TKey">
		/// Type of the property name.
		/// </typeparam>
		/// <typeparam name="TValue">
		/// Type of the property value.
		/// </typeparam>
		/// <param name="property">
		/// Property name value pair.
		/// </param>
		void SetContext<TKey, TValue>
		(
			LogContextType contextType,
			KeyValuePair<TKey, TValue> property
		);
		#pragma warning restore 1573

		#pragma warning disable 1573 
		/// <inheritdoc cref="SetContext(LogContextType,string,string)" select="param|remarks"/>
		/// <summary>
		/// Sets the values of the custom string properties that can be logged implicitly
		/// using a string dictionary.
		/// </summary>
		/// <param name="properties">
		/// Collection of name/value pairs holding custom properties.
		/// </param>
		void SetContext
		(
			LogContextType contextType,
			Dictionary<string, string> properties
		);
		#pragma warning restore 1573

		#pragma warning disable 1573 
		/// <inheritdoc cref="SetContext(LogContextType,Dictionary{string,string})" select="param|remarks"/>
		/// <summary>
		/// Sets the values of the custom string properties that can be logged implicitly
		/// using an object dictionary.
		/// </summary>
		void SetContext
		(
			LogContextType contextType,
			Dictionary<string, object> properties
		);
		#pragma warning restore 1573

		#pragma warning disable 1573 
		/// <inheritdoc cref="SetContext(LogContextType,Dictionary{string,string})" select="param|remarks"/>
		/// <summary>
		/// Sets the values of the custom string properties that can be logged implicitly
		/// using a name-value collection.
		/// </summary>
		void SetContext
		(
			LogContextType contextType,
			NameValueCollection properties
		);
		#pragma warning restore 1573
	}
}

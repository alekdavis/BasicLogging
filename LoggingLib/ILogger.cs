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
		/// <overloads>
		/// Writes a record to a log.
		/// </overloads>
		/// <summary>
		/// Writes a record to a log.
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
		/// <inheritdoc cref="Write(LogLevel,string,string,string,int)" select="summary|param|remarks"/> 
		#pragma warning restore 1573
		void Write
		(
			LogLevel	logLevel, 
			object		message, 
			[CallerMemberName]	string	callerMemberName	= "", 
			[CallerFilePath]	string	callerFilePath		= "", 
			[CallerLineNumber]	int		callerLineNumber	= 0
		);
        
		#pragma warning disable 1573 
		/// <inheritdoc cref="Write(LogLevel,string,string,string,int)" select="summary|param|remarks"/> 
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
		/// <inheritdoc cref="Write(LogLevel,string,string,string,int)" select="summary|param|remarks"/> 
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
		/// <inheritdoc cref="Write(LogLevel,string,Exception,string,string,int)" select="summary|param|remarks"/> 
		/// <param name="ex">
		/// Error information.
		/// </param>
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
		/// Sets values of custom properties that can be logged implicitly.
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
		/// <inheritdoc cref="SetContext(LogContextType,string,string)" select="summary|param|remarks"/> 
		#pragma warning restore 1573
		void SetContext
		(
			LogContextType	contextType,
			string			name,
			object			value
		);

		#pragma warning disable 1573 
		/// <inheritdoc cref="SetContext(LogContextType,string,string)" select="summary|param|remarks"/> 
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
		/// <inheritdoc cref="SetContext(LogContextType,string,string)" select="summary|param|remarks"/>
		/// <param name="contextType">
		/// Property accessibility scope.
		/// </param>
		/// <param name="properties">
		/// Collection of name/value pairs holding custom properties.
		/// </param>
		#pragma warning restore 1573
		void SetContext
		(
			LogContextType contextType,
			Dictionary<string, string> properties
		);

		#pragma warning disable 1573 
		/// <inheritdoc cref="SetContext(LogContextType,Dictionary{string,string})" select="summary|param|remarks"/>
		#pragma warning restore 1573
		void SetContext
		(
			LogContextType contextType,
			Dictionary<string, object> properties
		);

		#pragma warning disable 1573 
		/// <inheritdoc cref="SetContext(LogContextType,Dictionary{string,string})" select="summary|param|remarks"/>
		#pragma warning restore 1573
		void SetContext
		(
			LogContextType contextType,
			NameValueCollection properties
		);
	}
}

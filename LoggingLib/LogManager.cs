using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogging
{
	/// <summary>
	/// Implements a log factory generating a logger instance.
	/// </summary>
	public static class LogManager
	{
		// Internal error messages.
		private static readonly string _ERRMSG_UNSUPPORTED_PROVIDER =
			"Logging provider '{0}' is currently not supported.";

		/// <summary>
		/// Logging provider.
		/// </summary>
		/// <remarks>
		/// Only <c>Log4Net</c> is supported at this time.
		/// </remarks>
		public static LogProvider Provider = LogProvider.Log4Net;

		/// <overloads>
		/// Generates a logger instance.
		/// </overloads>
		/// <summary>
		/// Returns the logger of the specified type.
		/// </summary>
		/// <param name="name">
		/// Logger name.
		/// When not specified, the full name of the caller class will be
		/// used (it's a bit slow and therefore not recommended).
		/// </param>
		/// <returns>
		/// Logger instance.
		/// </returns>
		/// <remarks>
		/// Currently, only an instance of
		/// <see cref="BasicLogging.Log4NetWrapper"/>
		/// can be returned.
		/// </remarks>
		public static ILogger GetLogger
		(
			string name = null
		)
        {
			VerifyProvider();

			if (String.IsNullOrEmpty(name))
				return GetLogger((new StackFrame(1)).GetMethod().DeclaringType);

            return new Log4NetWrapper(name);
        }

		#pragma warning disable 1573
		/// <inheritdoc cref="GetLogger(string)" select="summary|param|returns|remarks"/> 
		/// <param name="type">
		/// Logger type.
		/// </param>		
		#pragma warning restore 1573
        public static ILogger GetLogger
		(
			Type type
		)
        {
			VerifyProvider();

			return new Log4NetWrapper(type);
        }

		/// <summary>
		/// Checks if the logging provider is supported.
		/// </summary>
		/// <exception cref="System.Exception">
		/// The specified provider is not supported.
		/// </exception>
		private static void VerifyProvider()
		{
			if (Provider != LogProvider.Log4Net)
				throw new Exception(
					String.Format(_ERRMSG_UNSUPPORTED_PROVIDER, Provider));
		}
	}
}

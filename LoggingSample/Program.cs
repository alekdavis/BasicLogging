using System;
using System.Threading.Tasks;

using BasicLogging;

namespace TestBasicLogging
{
	/// <summary>
	/// Writes different entries to a log file using the BasicLogging library
	/// with a log4net provider.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The format of the log entries is defined in the app.config file.
	/// </para>
	/// <para>
	/// The log file will be written to the working application directory.
	/// The name of the log file--as specified in the app.config file--
	/// is LoggingSample.log.
	/// </para>
	/// <para>
	/// Notice that all TRACE entries will be converted to DEBUG
	/// (because log4net does not support the TRACE log level).
	/// </para>
	/// </remarks>
	class Program
	{
		// Generate a static logger object.
		private static ILogger _log = LogManager.GetLogger(typeof(Program));

		/// <summary>
		/// Demonstrates the logging operations.
		/// </summary>
		/// <param name="args">
		/// Command-line arguments.
		/// </param>
		static void Main(string[] args)
		{
			// _log.Write(LogLevel.Trace, "Started.");
			_log.Trace("Started.");
			
			// Define a transaction ID for the duration of the call.
			_log.SetContext(LogContextType.LogicalThread, "transaction", Guid.NewGuid());

			// Test simple messages.
			DoThis();

			// Test exception logging.
			DoThat();

			// Test multi-threaded logging.
			DoTheOther();

			// Test the context getter methods.
			TestContextGet("transaction");

			// _log.Write(LogLevel.Trace, "Ended.");
			_log.Trace("Ended.");
		}

		/// <summary>
		/// Logs simple messages.
		/// </summary>
		static void DoThis()
		{
			// _log.Write(LogLevel.Trace, "Started.");
			_log.Trace("Started.");

			// _log.Write(LogLevel.Info, "Doing this.");
			_log.Info("Doing this.");

			// _log.Write(LogLevel.Trace, "Ended.");
			_log.Trace("Ended.");
		}

		/// <summary>
		/// Logs exception information.
		/// </summary>
		static void DoThat()
		{
			// _log.Write(LogLevel.Trace, "Started.");
			_log.Trace("Started.");

			// _log.Write(LogLevel.Info, "Doing that.");
			_log.Info("Doing that.");

			try
			{
				// _log.Write(LogLevel.Warn, "Generating DivideByZeroException.");
				_log.Warn("Generating DivideByZeroException.");
				throw new DivideByZeroException();
			}
			catch(Exception ex)
			{
				// _log.Write(LogLevel.Error, "Caught DivideByZeroException.", ex);
				_log.Error("Caught DivideByZeroException.", ex);
			}

			try
			{
				// _log.Write(LogLevel.Warn, "Generating StackOverflowException.");
				_log.Warn("Generating StackOverflowException.");
				
				throw new StackOverflowException();

			}
			catch(Exception ex)
			{
				// _log.Write(LogLevel.Error, "Caught StackOverflowException.");
				_log.Error("Caught StackOverflowException.");

				// _log.Write(LogLevel.Error, ex);
				_log.Error(ex);
			}

			try
			{
				// _log.Write(LogLevel.Warn, "Generating another exception.");
				_log.Warn("Generating multi-level exception.");

				DoFail();
			}
			catch(Exception ex)
			{
				// _log.Write(LogLevel.Error, "Caught another exception.");
				_log.Error("Caught multi-level exception.");

				// _log.Write(LogLevel.Fatal, ex);
				_log.Fatal(ex);
			}

			// _log.Write(LogLevel.Trace, "Ended.");
			_log.Trace("Ended.");
		}

		/// <summary>
		/// Logs messages from multiple threads.
		/// </summary>
		static void DoTheOther()
		{
			// _log.Write(LogLevel.Trace, "Started.");
			_log.Trace("Started.");

			for (int i=0; i<10; i++)
			{
				string message = "Running task " + i.ToString() + ".";

				Task.Run(()=>{
					// _log.Write(LogLevel.Info, message);
					_log.Info(message);
				});
			}

			// _log.Write(LogLevel.Trace, "Ended.");
			_log.Trace("Ended.");
		}

		/// <summary>
		/// Calls a method that generates an exception.
		/// </summary>
		static void DoFail()
		{
			// _log.Write(LogLevel.Trace, "Started.");
			_log.Trace("Started.");

			DoFailForSure();

			// _log.Write(LogLevel.Trace, "Ended.");
			_log.Trace("Ended.");
		}

		/// <summary>
		/// Generates an exception with an inner exception.
		/// </summary>
		static void DoFailForSure()
		{
			// _log.Write(LogLevel.Trace, "Started.");
			_log.Trace("Started.");

			throw new InvalidOperationException(
				"Invalid operation occurred.",
				new ApplicationException("Custom inner exception."));
		}

		/// <summary>
		/// Tests the context get methods.
		/// </summary>
		/// <param name="name">
		/// Name of the context property.
		/// </param>
		static void TestContextGet
		(
			string name
		)
		{
			string guid;
			
			guid = _log.GetContext(LogContextType.LogicalThread, name);

			_log.Info("Property '" + name + "' from logical thread: " + guid);
			
			guid = _log.GetContext(LogContextType.Thread, name);

			_log.Info("Property '" + name + "' from physical thread: " + guid);
			
			guid = _log.GetContext(LogContextType.Global, name);

			_log.Info("Property '" + name + "' from global context: " + guid);
			
			guid = _log.GetContext(name);

			_log.Info("Property '" + name + "' from unspecified context: " + guid);
		}
	}
}
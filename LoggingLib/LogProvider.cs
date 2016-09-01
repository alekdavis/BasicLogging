using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogging
{
	/// <summary>
	/// Defines supported logging providers.
	/// </summary>
	/// <remarks>
	/// Only <c>log4net</c> is currently supported.
	/// </remarks>
	public enum LogProvider
	{
		/// <summary>
		/// log4net
		/// </summary>
		Log4Net
	}
}

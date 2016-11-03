using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

#if SSHARP
namespace SSCore.Diagnostics
	{
   public enum OutputMode
	   {
	   Debugger,
		Console,
		ConsoleIfNotDebugging,
		None
	   }
	}
#endif
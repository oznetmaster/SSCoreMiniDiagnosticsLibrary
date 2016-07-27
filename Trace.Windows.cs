// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if SSHARP
using System;
using Environment = Crestron.SimplSharp.CrestronEnvironment;
#else
using System.Security;
using System.Threading;
#endif

#if SSHARP
using Crestron.SimplSharp;
namespace SSCore.Diagnostics
#else
namespace System.Diagnostics
#endif
	{
   public static partial class Trace
		{
#if SSHARP
	   public enum OutputMode
		   {
		   Debugger,
			Console,
			ConsoleIfNotDebugging,
			None
		   }

	   public static OutputMode Mode { get; set; }

	   static Trace ()
		   {
			Mode = OutputMode.Debugger;
		   }
#endif

	   internal static readonly IDebugLogger s_logger = new WindowsDebugLogger ();

		internal sealed class WindowsDebugLogger : IDebugLogger
			{
#if !NETCF
         [SecuritySafeCritical]
#endif
			public void ShowAssertDialog (string stackTrace, string message, string detailMessage)
				{
				string fullMessage = message + Environment.NewLine + detailMessage + Environment.NewLine + stackTrace;

				Debug.WriteLine (fullMessage);
				if (Debugger.IsAttached)
					{
					Debugger.Break ();
					}
				else
					{
#if SSHARP
#else
                    Environment.FailFast(fullMessage);
#endif
					}
				}

			private static readonly object s_ForLock = new Object ();

			public void WriteCore (string message)
				{
#if SSHARP
				if (Mode == OutputMode.None)
					return;
#endif
				// really huge messages mess up both VS and dbmon, so we chop it up into 
				// reasonable chunks if it's too big. This is the number of characters 
				// that OutputDebugstring chunks at.
				const int WriteChunkLength = 512;

				// We don't want output from multiple threads to be interleaved.
				lock (s_ForLock)
					{
					if (message == null || message.Length <= WriteChunkLength)
						{
						WriteToDebugger (message);
						}
					else
						{
						int offset;
						for (offset = 0; offset < message.Length - WriteChunkLength; offset += WriteChunkLength)
							{
							WriteToDebugger (message.Substring (offset, WriteChunkLength));
							}
						WriteToDebugger (message.Substring (offset));
						}
					}
				}

#if !NETCF
         [System.Security.SecuritySafeCritical]
#endif
			private static void WriteToDebugger (string message)
				{
#if !SSHARP
                if (Debugger.IsLogging())
                {
                    Debugger.Log(0, null, message);
                }
                else
#endif
					{
#if SSHARP
					if (Mode == OutputMode.Console || (Mode == OutputMode.ConsoleIfNotDebugging && !Debugger.IsAttached))
						CrestronConsole.Print (message ?? string.Empty);
					else 
						Debugger.Write (message ?? string.Empty);
#else
               Interop.mincore.OutputDebugString(message ?? string.Empty);
#endif
					}
				}
			}
		}
	}

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if SSHARP
using System;
using Environment = Crestron.SimplSharp.CrestronEnvironment;
using Crestron.SimplSharp.Reflection;
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
		public static OutputMode Mode { get; set; }

		static Trace ()
			{
			Mode = OutputMode.Debugger;
			}

		public delegate string FormatMessageDelegate (string message, bool chunked);

		private static FormatMessageDelegate formatMessage;

		public static FormatMessageDelegate FormatMessage
			{
			get { return formatMessage ?? ((s, b) => s); }
			set { formatMessage = value; }
			}
#endif

		internal static readonly IDebugLogger s_logger = new WindowsDebugLogger ();

		internal sealed class WindowsDebugLogger : IDebugLogger
			{
#if SSHARP
			private static readonly CType _ctypeThread;
			private static readonly PropertyInfo _propCurrentThread;
			private static readonly PropertyInfo _propName;
			private delegate object DelGetCurrentThread ();
			private static DelGetCurrentThread _delGetCurrentThread;

			static WindowsDebugLogger ()
				{
				try
					{
					_ctypeThread = Type.GetType ("Crestron.SimplSharpPro.CrestronThread.Thread, SimplSharpPro");
					}
				catch
					{
					}

				if (_ctypeThread == null)
					return;

				_propCurrentThread = _ctypeThread.GetProperty ("CurrentThread");
				_propName = _ctypeThread.GetProperty ("Name");
				}

			private static bool IsCommandThread
				{
				get
					{
					if (_ctypeThread == null)
						return false;

					if (_delGetCurrentThread == null)
						_delGetCurrentThread = (DelGetCurrentThread)CDelegate.CreateDelegate (typeof (DelGetCurrentThread), null, _propCurrentThread.GetGetMethod ());

					return (string)_propName.GetValue (_delGetCurrentThread (), null) == "SimplSharpProCommandProcessorThread";
					}
				}
#endif

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
							WriteToDebugger (message.Substring (offset, WriteChunkLength), offset != 0);
							}
						WriteToDebugger (message.Substring (offset), offset != 0);
						}
					}
				}

#if !NETCF
         [System.Security.SecuritySafeCritical]
#endif

			private static void WriteToDebugger (string message)
				{
				WriteToDebugger (message, false);
				}

			private static void WriteToDebugger (string message, bool chunk)
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
					var msg = FormatMessage (message ?? string.Empty, chunk);

					if (Mode == OutputMode.Console || (Mode == OutputMode.ConsoleIfNotDebugging && !Debugger.IsAttached))
						if (IsCommandThread)
							CrestronConsole.ConsoleCommandResponse (msg);
						else
							CrestronConsole.Print (msg);
					else
						Debugger.Write (msg);
#else
               Interop.mincore.OutputDebugString(message ?? string.Empty);
#endif
					}
				}
			}
		}
	}
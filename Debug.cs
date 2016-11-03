// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#define DEBUG // Do not remove this, it is needed to retain calls to these conditional methods in release builds

using System;
#if SSHARP
using System.Linq;
using Environment = Crestron.SimplSharp.CrestronEnvironment;
#else
#endif

#if SSHARP
namespace SSCore.Diagnostics
#else
namespace System.Diagnostics
#endif
	{
	/// <summary>
	/// Provides a set of properties and methods for debugging code.
	/// </summary>
	public static class Debug
		{
#if SSHARP
		public static OutputMode Mode
			{
			get { return Trace.Mode; }
			set { Trace.Mode = value; }
			}

		static Debug ()
		   {
			Mode = OutputMode.Debugger;
		   }
#endif

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void Assert (bool condition)
			{
			Assert (condition, string.Empty, string.Empty);
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void Assert (bool condition, string message)
			{
			Assert (condition, message, string.Empty);
			}

		[System.Diagnostics.Conditional ("DEBUG")]
#if !NETCF
        [System.Security.SecuritySafeCritical]
#endif
		public static void Assert (bool condition, string message, string detailMessage)
			{
			if (!condition)
				{
				string stackTrace = null;

#if NETCF
				try
					{
					int j = 0;
					int i = 123 / j;
					}
				catch (DivideByZeroException dex)
					{
					stackTrace = String.Join ("\r\n", dex.StackTrace.Replace ("\r\n", "\n").Split ('\n').Skip (1).ToArray ());
					}
#else
                try
                {
                    stackTrace = Environment.StackTrace;
                }
                catch
                {
                    stackTrace = "";
                }
#endif

				WriteLine (FormatAssert (stackTrace, message, detailMessage));
				Trace.s_logger.ShowAssertDialog (stackTrace, message, detailMessage);
				}
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void Fail (string message)
			{
			Assert (false, message, string.Empty);
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void Fail (string message, string detailMessage)
			{
			Assert (false, message, detailMessage);
			}

		private static string FormatAssert (string stackTrace, string message, string detailMessage)
			{
			return "---- DEBUG ASSERTION FAILED ----" + Environment.NewLine
						  + "---- Assert Short Message ----" + Environment.NewLine
						  + message + Environment.NewLine
						  + "---- Assert Long Message ----" + Environment.NewLine
						  + detailMessage + Environment.NewLine
						  + stackTrace;
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void Assert (bool condition, string message, string detailMessageFormat, params object[] args)
			{
			Assert (condition, message, string.Format (detailMessageFormat, args));
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void WriteLine (string message)
			{
			Write (message + Environment.NewLine);
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void Write (string message)
			{
			Trace.s_logger.WriteCore (message ?? string.Empty);
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void WriteLine (object value)
			{
			WriteLine ((value == null) ? string.Empty : value.ToString ());
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void WriteLine (object value, string category)
			{
			WriteLine ((value == null) ? string.Empty : value.ToString (), category);
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void WriteLine (string format, params object[] args)
			{
			WriteLine (string.Format (null, format, args));
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void WriteLine (string message, string category)
			{
			if (category == null)
				{
				WriteLine (message);
				}
			else
				{
				WriteLine (category + ":" + ((message == null) ? string.Empty : message));
				}
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void Write (object value)
			{
			Write ((value == null) ? string.Empty : value.ToString ());
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void Write (string message, string category)
			{
			if (category == null)
				{
				Write (message);
				}
			else
				{
				Write (category + ":" + ((message == null) ? string.Empty : message));
				}
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void Write (object value, string category)
			{
			Write ((value == null) ? string.Empty : value.ToString (), category);
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void WriteIf (bool condition, string message)
			{
			if (condition)
				{
				Write (message);
				}
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void WriteIf (bool condition, object value)
			{
			if (condition)
				{
				Write (value);
				}
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void WriteIf (bool condition, string message, string category)
			{
			if (condition)
				{
				Write (message, category);
				}
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void WriteIf (bool condition, object value, string category)
			{
			if (condition)
				{
				Write (value, category);
				}
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void WriteLineIf (bool condition, object value)
			{
			if (condition)
				{
				WriteLine (value);
				}
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void WriteLineIf (bool condition, object value, string category)
			{
			if (condition)
				{
				WriteLine (value, category);
				}
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void WriteLineIf (bool condition, string value)
			{
			if (condition)
				{
				WriteLine (value);
				}
			}

		[System.Diagnostics.Conditional ("DEBUG")]
		public static void WriteLineIf (bool condition, string value, string category)
			{
			if (condition)
				{
				WriteLine (value, category);
				}
			}

		}
	}

#if SSHARP
namespace SSCore.Diagnostics
#else
namespace System.Diagnostics
#endif
	{
	internal interface IDebugLogger
		{
		void ShowAssertDialog (string stackTrace, string message, string detailMessage);
		void WriteCore (string message);
		}
	}
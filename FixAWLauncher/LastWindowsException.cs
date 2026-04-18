using System.ComponentModel;
using System.Runtime.InteropServices;

namespace FixAWLauncher;

internal class LastWindowsException : Exception
{
	internal LastWindowsException() : base(GetLastWin32ErrorMessage())
	{
	}

	internal static int ThrowIfZero(int result)
	{
		if (result < 1)
			throw new LastWindowsException();

		return result;
	}

	internal static void ThrowIfFalse(bool okResult)
	{
		if (!okResult)
			throw new LastWindowsException();
	}

	private static string GetLastWin32ErrorMessage()
	{
		int errorCode = Marshal.GetLastWin32Error();
		string message = new Win32Exception(errorCode).Message;
		return $"Win32 Error {errorCode} : {message}";
	}
}

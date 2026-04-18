using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FixAWLauncher;

internal class LauncherWindow(Process process, IntPtr hWnd)
{
	internal IntPtr Handle => hWnd;

	private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

	[DllImport("user32.dll")]
	private static extern bool IsWindowVisible(IntPtr hWnd);

	[DllImport("user32.dll")]
	private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

	[StructLayout(LayoutKind.Sequential)]
	private struct RECT
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}

	private static readonly IntPtr HWND_TOP = IntPtr.Zero;
	private const uint SWP_NOSIZE = 0x0001;
	private const uint SWP_NOZORDER = 0x0004;

	internal static async Task<LauncherWindow?> FindIt(Process process)
	{
		IntPtr hWnd = await FindWindowHandle(process);
		if (hWnd == IntPtr.Zero)
			return null;

		return new LauncherWindow(process, hWnd);

		static async Task<IntPtr> FindWindowHandle(Process process)
		{
			// Wait a short time for the main/top-level window handle to be available
			IntPtr launcherHWnd = IntPtr.Zero;
			for (int i = 0; i < 50; i++)
			{
				process.Refresh();
				launcherHWnd = CheckEachWindow(process.Id);

				if (launcherHWnd != IntPtr.Zero)
					break;

				await Task.Delay(100);
			}

			return launcherHWnd;
		}

		// Helper: enumerate top-level windows and find one that belongs to the given process id
		static IntPtr CheckEachWindow(int pid)
		{
			IntPtr lancherHWnd = IntPtr.Zero;
			EnumWindowsProc checkWindowFunction = (nint hWnd, nint lParam) =>
			{
				if (!IsWindowVisible(hWnd))
					return true; // continue

				var result = GetWindowThreadProcessId(hWnd, out uint windowPid);
				if (result == 0) // Failed to get process id for this window, skip it
					return true; // continue

				if ((int)windowPid == pid)
				{
					lancherHWnd = hWnd;
					return false; // stop enumeration
				}

				return true; // continue
			};

			EnumWindows(checkWindowFunction, IntPtr.Zero);

			return lancherHWnd;
		}
	}

	internal async Task FixPosition(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested && !process.HasExited)
		{
			LastWindowsException.ThrowIfFalse(GetWindowRect(hWnd, out RECT rect));

			// Set window position relative to the top of the screen, but keep its current horizontal position and size
			LastWindowsException.ThrowIfFalse(SetWindowPos(hWnd, HWND_TOP, rect.Left, 100, 0, 0, SWP_NOSIZE | SWP_NOZORDER));

			await Task.Delay(500, cancellationToken);
		}
	}
}

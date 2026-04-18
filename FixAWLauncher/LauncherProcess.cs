using System.Diagnostics;

namespace FixAWLauncher;

internal class LauncherProcess(Process process)
{
	const string lancherName = "ArmoredWarfareLauncher"; // without .exe

	internal int Id => process.Id;

	internal static LauncherProcess? FindIt()
	{
		var processes = Process.GetProcessesByName(lancherName);
		var process = processes.Length == 0 ? LaunchIt() : processes[0];

		return process != null ? new LauncherProcess(process) : null;
	}

	private static Process? LaunchIt()
	{
		var files = Directory.GetFiles(Directory.GetCurrentDirectory(), $"{lancherName}.exe");
		if (files.Length == 0)
			return null;

		return Process.Start(files[0]);
	}

	internal Task<LauncherWindow?> GetWindow()
	{
		return LauncherWindow.FindIt(process);
	}
}

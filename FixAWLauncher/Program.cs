namespace FixAWLauncher;

internal class Program
{

	static async Task Main(string[] args)
	{
		Console.WriteLine("Searching for the launcher process");
		try
		{
			var process = LauncherProcess.FindIt();
			if (process is null)
			{
				Console.WriteLine("No processes found.");
				return;
			}

			Console.WriteLine($"Process Id {process.Id}");

			var window = await process.GetWindow();

			if (window is null)
			{
				Console.WriteLine("Could not obtain main window handle.");
				return;
			}

			Console.WriteLine($"Window handle: {window.Handle}");

			Console.WriteLine("Fixing window position. Press Ctrl+C to stop.");
			var cancellationTokenSource = new CancellationTokenSource();
			Console.CancelKeyPress += (s, e) => { cancellationTokenSource.Cancel(); e.Cancel = true; };

			await window.FixPosition(cancellationTokenSource.Token);
		}
		catch (LastWindowsException ex)
		{
			Console.WriteLine($"Exception: {ex.Message}");
		}

		Console.WriteLine("Finished.");
	}
}

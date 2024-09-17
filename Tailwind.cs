namespace BlazorTailwind;

using System.Diagnostics;

public static class Tailwind
{
    private static Process? _sTailwindProcess;

    private static string? GetRootDirectory()
    {
        var baseDirectory = AppContext.BaseDirectory;
        var directoryInfo = Directory.GetParent(baseDirectory);

        while (directoryInfo != null && directoryInfo.GetFiles("*.sln").Length == 0)
        {
            directoryInfo = directoryInfo.Parent;
        }

        return directoryInfo?.FullName;
    }

    public async static Task DevAsync(string cssInput, string cssOutput)
    {
        var rootDirectory = GetRootDirectory();

        if (rootDirectory == null)
        {
            Console.WriteLine("Error: Could not find the root directory.");

            return;
        }

        var tailwindExePath = Path.Combine(rootDirectory, @".tailwind\3.4.10\tailwindcss-windows-x64.exe");
        var tailwindCssInput = Path.Combine(rootDirectory, cssInput);
        var tailwindConfig = Path.Combine(rootDirectory, "tailwind.config.js");
        var tailwindCssOutput = Path.Combine(rootDirectory, cssOutput);

        Console.WriteLine($"TailwindConfig: {tailwindConfig}");
        Console.WriteLine($"TailwindExePath: {tailwindExePath}");
        Console.WriteLine($"TailwindCssInput: {tailwindCssInput}");
        Console.WriteLine($"TailwindCssOutput: {tailwindCssOutput}");

        if (!File.Exists(tailwindExePath))
        {
            Console.WriteLine($"Error: The file '{tailwindExePath}' does not exist.");

            return;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = tailwindExePath,
            Arguments = $" -c {tailwindConfig} -i {tailwindCssInput} -o {tailwindCssOutput} --watch",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using (_sTailwindProcess = new Process
                   {
                       StartInfo = startInfo,
                   })
            {
                _sTailwindProcess.Start();
                Console.WriteLine("Tailwind process started.");

                var outputTask = _sTailwindProcess.StandardOutput.ReadToEndAsync();
                var errorTask = _sTailwindProcess.StandardError.ReadToEndAsync();

                await Task.WhenAll(outputTask, errorTask);

                var output = await outputTask;
                var error = await errorTask;

                if (!string.IsNullOrEmpty(output))
                {
                    Console.WriteLine("Output: " + output);
                }

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine("Error: " + error);
                }

                await _sTailwindProcess.WaitForExitAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }
        finally
        {
            _sTailwindProcess = null;
        }
    }

    public static void StopTailwindProcess()
    {
        if (_sTailwindProcess is not { HasExited: false })
        {
            return;
        }

        _sTailwindProcess.Kill();
        _sTailwindProcess.Dispose();
        _sTailwindProcess = null;
    }
}

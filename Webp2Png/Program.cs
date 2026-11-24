using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Microsoft.Win32;
using SixLabors.ImageSharp;

namespace Webp2Png;

internal static class Program
{
    private const string AppName = "Webp2Png";
    private const string WebpExtension = ".webp";
    private const string PngExtension = ".png";
    private const string ShellKeyPath = $@"*\shell\{AppName}";

    public static async Task Main(string[] args)
    {
        var app = ConsoleApp.Create();

        app.Add("", WebpToPngAsync);
        app.Add("install", Install);
        app.Add("uninstall", Uninstall);

        await app.RunAsync(args).ConfigureAwait(false);
    }

    /// <summary>
    /// Converts a WebP image file to a PNG image file next to it and deletes the WebP file on success.
    /// </summary>
    /// <param name="path">Path to the WebP image to convert.</param>
    /// <param name="cancellationToken">Cancels the conversion (Ctrl+C).</param>
    /// <returns>0 on success, -1 if the file does not exist or the conversion fails.</returns>
    private static async Task<int> WebpToPngAsync([Argument] string path, CancellationToken cancellationToken)
    {
        path = Path.GetFullPath(path);
        if (!File.Exists(path))
        {
            ConsoleApp.LogError($"There is no file at '{path}'.");
            return -1;
        }

        try
        {
            var outPath = Path.ChangeExtension(path, PngExtension);
            using (var image = await Image.LoadAsync(path, cancellationToken).ConfigureAwait(false))
            {
                await image.SaveAsPngAsync(outPath, cancellationToken).ConfigureAwait(false);
            }

            File.Delete(path);
            return 0;
        }
        catch (Exception e) when (e is not OperationCanceledException)
        {
            ConsoleApp.LogError(e.Message);
            return -1;
        }
    }

    /// <summary>
    /// Removes the Explorer context menu entry for WebP files.
    /// </summary>
    /// <returns>0 on success, -1 if an error occurs.</returns>
    private static int Uninstall()
    {
        try
        {
            Registry.ClassesRoot.DeleteSubKeyTree(ShellKeyPath, throwOnMissingSubKey: false);
            return 0;
        }
        catch (Exception e)
        {
            ConsoleApp.LogError(e.Message);
            return -1;
        }
    }

    /// <summary>
    /// Adds an Explorer context menu entry "Convert to Png" for WebP files.
    /// </summary>
    /// <returns>0 on success, -1 if an error occurs.</returns>
    private static int Install()
    {
        try
        {
            var pathToExe = Environment.ProcessPath;
            if (string.IsNullOrWhiteSpace(pathToExe))
            {
                ConsoleApp.LogError("Can't determine own path!");
                return -1;
            }

            pathToExe = Path.GetFullPath(pathToExe);

            using (var key = Registry.ClassesRoot.CreateSubKey(ShellKeyPath))
            {
                key.SetValue("", "Convert to Png");
                key.SetValue("AppliesTo", WebpExtension);
                key.SetValue("icon", $"{pathToExe},0");
            }

            using (var commandKey = Registry.ClassesRoot.CreateSubKey($@"{ShellKeyPath}\command"))
            {
                // Quote the exe path: paths with spaces (e.g. "Program Files") otherwise break the verb.
                commandKey.SetValue("", $"\"{pathToExe}\" \"%1\"");
            }

            return 0;
        }
        catch (Exception e)
        {
            ConsoleApp.LogError(e.Message);
            return -1;
        }
    }
}

using System;
using System.IO;
using ConsoleAppFramework;
using Microsoft.Win32;
using SixLabors.ImageSharp;

namespace Webp2Png;

internal static class Program
{
    private const string AppName = "Webp2Png";
    private const string WebpExtension = ".webp";
    private const string PngExtension = ".png";
    
    public static void Main(string[] args)
    {
        var app = ConsoleApp.Create();
        
        app.Add("", WebpToPng);
        app.Add("install", Install);
        app.Add("uninstall", Uninstall);
        
        app.Run(args);
    }

    /// <summary>
    /// Converts a WebP image file to a PNG image file while preserving the image content.
    /// The WebP file is deleted upon successful conversion.
    /// </summary>
    /// <param name="path">
    /// The file path to the WebP image that will be converted to PNG format.
    /// Must be a valid and existing file path.
    /// </param>
    /// <return>
    /// Returns 0 if the conversion is successful.
    /// Returns -1 if the file does not exist or an error occurs during the conversion process.
    /// </return>
    private static int WebpToPng([Argument] string path)
    {
        path = Path.GetFullPath(path);
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"There is no file at '{path}'.");
            return -1;
        }

        try
        {
            var outPath = Path.ChangeExtension(path, PngExtension);
            using (var image = Image.Load(path))
            {
                image.SaveAsPng(outPath);
            }

            File.Delete(path);
            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return -1;
        }
    }

    /// <summary>
    /// Removes the application's registry entries to unregister any associated shell extensions.
    /// </summary>
    /// <return>
    /// Returns 0 if the uninstallation is successful, or -1 if an error occurs.
    /// </return>
    private static int Uninstall()
    {
        try
        {
            Registry.ClassesRoot.DeleteSubKeyTree($@"*\shell\{AppName}");
            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return -1;
        }
    }

    /// <summary>
    /// Adds registry entries to integrate the application as a shell extension.
    /// </summary>
    /// <returns>
    /// Returns 0 if the installation is successful, or -1 if an error occurs.
    /// </returns>
    private static int Install()
    {
        try
        {
            var pathToExe = Environment.ProcessPath;
            if (string.IsNullOrWhiteSpace(pathToExe))
            {
                Console.Error.WriteLine("Can't determine own path!");
                return -1;
            }

            pathToExe = Path.GetFullPath(pathToExe);

            using (var key = Registry.ClassesRoot.CreateSubKey($@"*\shell\{AppName}"))
            {
                key.SetValue("", "Convert to Png");
                key.SetValue("AppliesTo", WebpExtension);
                key.SetValue("icon", pathToExe + ",0");
            }

            using (var commandKey = Registry.ClassesRoot.CreateSubKey($@"*\shell\{AppName}\command"))
            {
                commandKey.SetValue("", $@"{pathToExe} ""%1""");
            }

            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return -1;
        }
    }
}
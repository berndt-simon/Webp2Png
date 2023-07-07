using System;
using System.IO;
using Microsoft.Win32;
using SixLabors.ImageSharp;

namespace Webp2Png;

internal static class Program
{
    private const string appName = "Webp2Png";
    private const string webpExtension = ".webp";
    private const string pngExtension = ".png";


    public static void Main(string[] args)
    {
        if (args.Length is not 1)
        {
            Console.Error.WriteLine("Expecting exactly one argument.");
            Console.Error.WriteLine("Either '-install', '-uninstall', or a path to a webp file.");
            Environment.Exit(-1);
        }

        if (args[0] == "-install")
        {
            Environment.Exit(Install());
        }

        if (args[0] == "-uninstall")
        {
            Environment.Exit(Uninstall());
        }

        
        Environment.Exit(WebpToPng(args[0]));
    }

    private static int WebpToPng(string path)
    {
        path = Path.GetFullPath(path);
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"There is no file at '{path}'.");
            return -1;
        }
        
        try
        {
            var outPath = Path.ChangeExtension(path, pngExtension);
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

    private static int Uninstall()
    {
        try
        {
            Registry.ClassesRoot.DeleteSubKeyTree($@"*\shell\{appName}");
            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            return -1;
        }
    }

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

            using (var key = Registry.ClassesRoot.CreateSubKey($@"*\shell\{appName}"))
            {
                key.SetValue("", "Convert to Png");
                key.SetValue("AppliesTo", webpExtension);
                key.SetValue("icon", pathToExe + ",0");
            }

            using (var commandKey = Registry.ClassesRoot.CreateSubKey($@"*\shell\{appName}\command"))
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
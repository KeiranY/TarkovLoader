
using System.IO;
using UnityEngine;
//using Aki.Common;
using System.Reflection;
using System.Diagnostics;
using System;
using TarkovLoader;

public class SPTLoader
{
    private static void Main()
    {
        var Log = new NLogLog();

        string folder = "";
        // Search mods folder for module.dlls
        foreach (var m in Directory.GetFiles(@"user\mods\", "module.dll", SearchOption.AllDirectories))
        {
            try
            {

                var name = AssemblyName.GetAssemblyName(m);
                Log.Info(name.ToString());
                // Check if the assembly name is our own
                if (name.Name == "TarkovLoader")
                {
                    // Get mod folder name
                    folder = m.Split('\\')[2];
                    break;
                }
            }
            catch { }
        }
        if (folder == "")
        {
            Log.Error("Unable to find own module.dll under 'user\\mods\\'. Stopping loader.");
            return;
        }

        Log.Info("Hello from modloader");
        var loader = new BaseLoader(new GameObject("kcy-loader"), Log, $@"user\mods\{ folder }");

        loader.LoadMods();
    }
}

//public class SPTLog : BaseLog
//{
//    public SPTLog()
//    {
//        Console.WriteLine("test");
//        var log = Assembly.LoadFrom(@"EscapeFromTarkov_Data\Managed\Aki.Common.dll").GetType("Aki.Common.Log");
//        Debug = new Action<string>((msg) => log.GetMethod("Write").Invoke(null, new System.Object[] { msg }));
//        Info = new Action<string>((msg) => log.GetMethod("Info").Invoke(null, new System.Object[] { msg }));
//        Warn = new Action<string>((msg) => log.GetMethod("Warning").Invoke(null, new System.Object[] { msg }));
//        Error = new Action<string>((msg) => log.GetMethod("Error").Invoke(null, new System.Object[] { msg }));
//    }
//}


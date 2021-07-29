using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq;

namespace TarkovLoader
{
    public abstract class BaseMod : MonoBehaviour
    {
        //TODO: public BaseLoader loader;
        public Dictionary<string, string> options;
        abstract public Dictionary<string, string> DefaultOptions { get; }
        public BaseLoader loader;

        public virtual void Init(Dictionary<string, string> options, BaseLoader loader)
        {
            if (options != null && options.Count != 0)
            {
                this.options = options;
            }
            else
            {
                this.options = DefaultOptions;
            }
            this.loader = loader;
        }

    }

    public class BaseLoader
    {
        public GameObject mainObject;
        public BaseLog log;
        public string modPath;

        public BaseLoader(GameObject mainObject, BaseLog log, string modPath)
        {
            GameObject.DontDestroyOnLoad(mainObject);
            this.mainObject = mainObject;
            this.log = log;
            this.modPath = modPath;
        }

        public List<BaseMod> mods = new List<BaseMod>();
            

        public void LoadMods()
        {
            log.Info($"[loader] Loading mods from: {modPath}");

            foreach (var modDll in Directory.GetFiles(modPath, "*.dll", SearchOption.AllDirectories))
            {
                if (modPath.EndsWith("module.dll")) continue;
                LoadMod(modDll);
            }
        }

        void LoadMod(string dllPath)
        {
            string name = Path.GetFileNameWithoutExtension(dllPath);
            log.Info($"[loader][{name}] Loading from {dllPath}");
            string confPath = Path.ChangeExtension(dllPath, "json");
            // Load options
            Dictionary<string, string> options = null;
            if (File.Exists(confPath))
            {
                try
                {
                    options = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(confPath));
                }
                catch (Exception e)
                {
                    log.Error($"[loader][{name}] Error occured while loading conf file {confPath}");
                    log.Error(e.Message + '\n' + e.StackTrace);
                }
            }
            // Load dll
            try
            {
                var assembly = Assembly.LoadFrom(dllPath);
                foreach (Type t in assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BaseMod)))) {
                    // Create mod
                    try
                    {
                        BaseMod mod = (BaseMod)mainObject.AddComponent(t);
                        mod.Init(options, this);
                        mods.Add(mod);
                        log.Info($"[loader][{name}] Loaded mod type {t.Name}");
                    }
                    catch (Exception e)
                    {
                        log.Error($"[loader][{name}] Failed to load mod type {t.Name}");
                        log.Error(e.Message + '\n' + e.StackTrace);
                    }
                }
            } 
            catch (Exception e)
            {
                log.Error($"[loader][{name}] Failed to load mods");
                log.Error(e.Message + '\n' + e.StackTrace);
            }
        }
        

    }


    abstract public class BaseLog
    {
        public Action<string> Debug;
        public Action<string> Info;
        public Action<string> Warn;
        public Action<string> Error;
    }

    public class NLogLog : BaseLog
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("application");

        public NLogLog()
        {
            Debug = new Action<string>((msg) => Logger.Debug(msg));
            Info = new Action<string>((msg) => Logger.Info(msg));
            Warn = new Action<string>((msg) => Logger.Warn(msg));
            Error = new Action<string>((msg) => Logger.Error(msg));
        }
    }
}

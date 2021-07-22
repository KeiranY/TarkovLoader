using System.Collections.Generic;
using TarkovLoader;
using UnityEngine;

namespace Nightvision
{
    public class Nightvision : BaseMod
    {
        public override Dictionary<string, string> DefaultOptions => new Dictionary<string, string>(){ 
            { "key", "k" } 
        };

        void Update()
        {
            if (Input.GetKeyDown(options["key"]))
            {
                var component = Camera.main.GetComponent<BSG.CameraEffects.NightVision>();
                loader.log.Info($"[nightvision] Toggling { (component.On ? "Off" : "On")}");
                component?.StartSwitch(!component.On);
            }
        }
    }
}

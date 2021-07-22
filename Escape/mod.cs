using System;
using System.Collections.Generic;
using TarkovLoader;
using UnityEngine;
using EFT;
using EFT.Interactive;

namespace Escape
{
    public class Escape : BaseMod
    {
        public override Dictionary<string, string> DefaultOptions => new Dictionary<string, string>(){
            { "key", "l" }
        };

        void Update()
        {
            if (Utils.InGame && Input.GetKeyDown(options["key"]))
            {
                Player localPlayer = Utils.LocalPlayer;
                if (localPlayer != null)
                {
                    // Get an extract
                    // TODO: Don't loop, actually fix whats blocking the extract
                    foreach (var ex in Utils.GameWorld.ExfiltrationController.ExfiltrationPoints)
                    {
                        // Remove requirements
                        ex.Requirements = new ExfiltrationRequirement[0];
                        // Set it to instant
                        ex.Settings.ExfiltrationTime = 0.0f;
                        // Extract
                        ex.Proceed(localPlayer);
                    }
                }
            }
        }
    }
}

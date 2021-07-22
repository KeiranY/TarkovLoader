using System;
using System.Collections.Generic;
using TarkovLoader;
using UnityEngine;

namespace Freecam
{
    public class Freecam : BaseMod
    {

        GameObject camObj = null;
        Camera backupCam;
        GameObject _input = null;
        GameObject input => GameObject.Find("___Input");

        public override Dictionary<string, string> DefaultOptions => new Dictionary<string, string>(){
            { "key", "m" }
        };

        void Update()
        {
            if (Utils.InGame && Input.GetKeyDown(options["key"]))
            {
                loader.log.Debug("[Freecam] Toggle");
                if (camObj == null)
                {
                    // Backup OG cam
                    backupCam = Camera.main;
                    // Create new cam object, add flymode and set as main
                    camObj = Instantiate(backupCam.gameObject);
                    camObj.AddComponent<ExtendedFlycam>();
                    camObj.GetComponent<Camera>().tag = "MainCamera";
                    ((Behaviour)camObj.GetComponent("Cinemachine.CinemachineBrain")).enabled = false;
                    // Disable OG cam
                    backupCam.enabled = false;
                    // Disable game input
                    input?.SetActive(false);
                }
                else
                {
                    // Enable OG cam
                    backupCam.enabled = true;
                    // Enable game input
                    input?.SetActive(true);
                    // Destroy cloned cam object
                    GameObject.Destroy(camObj);
                    camObj = null;
                }
            }
        }
    }
}

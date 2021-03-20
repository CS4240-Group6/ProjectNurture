using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand.Demo{
    public class OVRTeleporterLink : MonoBehaviour{
        public Teleporter teleport;
        public OVRInput.Controller controller;
        public OVRInput.Button teleportButton;
        bool aiming = false;

        public void FixedUpdate() {
            if(!aiming && OVRInput.Get(teleportButton, controller)) {
                aiming = true;
                teleport.StartTeleport();
            }

            if(aiming && !OVRInput.Get(teleportButton, controller)) {
                aiming = false;
                teleport.Teleport();
            }
        }
    }
}

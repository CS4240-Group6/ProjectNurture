using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

namespace Autohand.Demo{
    public class OVRHandControllerLink : MonoBehaviour{
        public Hand hand;
        public OVRInput.Controller controller;
        public OVRInput.Axis1D grabAxis;
        public OVRInput.Button grabButton;
        public OVRInput.Button squeezeButton;
        
        bool grabbing = false;
        bool squeezing = false;

        public void Update() {
            if(!grabbing && OVRInput.GetDown(grabButton, controller)) {
                grabbing = true;
                hand.Grab();
            }
            if(grabbing && OVRInput.GetUp(grabButton, controller)) {
                grabbing = false;
                hand.Release();
            }

            if(!squeezing && OVRInput.GetDown(squeezeButton, controller)) {
                squeezing = true;
                hand.Squeeze();
            }
            if(squeezing && OVRInput.GetUp(squeezeButton, controller)) {
                squeezing = false;
                hand.Unsqueeze();
            }

            hand.SetGrip(OVRInput.Get(grabAxis, controller));
        }

        public float GetAxis(OVRInput.Axis1D axis) {
            return OVRInput.Get(axis, controller);
        }

        public Vector2 GetAxis2D(OVRInput.Axis2D axis) {
            return OVRInput.Get(axis, controller);
        }
        
        public bool ButtonPressed(OVRInput.Button button) {
            return OVRInput.Get(button, controller);
        }
        
        public bool ButtonPressed(OVRInput.Touch button) {
            return OVRInput.Get(button, controller);
        }
        
        public bool ButtonTouched(OVRInput.Touch button) {
            return OVRInput.Get(button, controller);
        }
    }
}

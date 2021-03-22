using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand.Demo{
    public class OVRAutoHandFingerBender : MonoBehaviour{
        public OVRHandControllerLink controller;
        public OVRInput.Touch touch;
        public OVRInput.Button button;
        
        [HideInInspector]
        public float[] bendOffsets;

        bool pressed;
        
        void Update(){
            if(!pressed && Pressed()) {
                pressed = true;
                for(int i = 0; i < controller.hand.fingers.Length; i++) {
                    controller.hand.fingers[i].bendOffset += bendOffsets[i];
                }
            }
            else if(pressed && !Pressed()) {
                pressed = false;
                for(int i = 0; i < controller.hand.fingers.Length; i++) {
                    controller.hand.fingers[i].bendOffset -= bendOffsets[i];
                }
            }
        }

        bool Pressed() {
            return controller.ButtonPressed(button) || controller.ButtonTouched(touch);
        }
        
        private void OnDrawGizmosSelected() {
            if(controller == null && GetComponent<OVRHandControllerLink>()){
                controller = GetComponent<OVRHandControllerLink>();
                bendOffsets = new float[controller.hand.fingers.Length];
            }
        }
    }
}

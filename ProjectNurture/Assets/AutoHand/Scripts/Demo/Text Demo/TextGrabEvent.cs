using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

namespace Autohand.Demo{
    public class TextGrabEvent : MonoBehaviour{
        public TextChanger changer;
        public Grabbable grab;
        public float changeTime = 0.2f;
        public float fadeTime = 5;
        [TextArea]
        public string message;

        private void Start() {
            if(grab == null && GetComponent<Grabbable>() != null)
                grab = GetComponent<Grabbable>();

            if(grab == null || changer == null)
                Destroy(this);

            grab.OnGrabEvent += OnGrab;
            grab.OnJointBreakEvent += OnRelease;
            grab.OnReleaseEvent += OnRelease;
        }
        
        void OnGrab(Hand hand, Grabbable grab) {
            changer?.UpdateText(gameObject, message, changeTime);
        }

        void OnRelease(Hand hand, Grabbable grab) {
            changer?.HideText(changeTime, fadeTime);
        }
    }
}

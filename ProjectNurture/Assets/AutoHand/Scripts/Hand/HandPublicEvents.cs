using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Autohand{
    public class HandPublicEvents : MonoBehaviour{
        public Hand hand;
        public UnityEvent OnGrab;
        public UnityEvent OnRelease;
        public UnityEvent OnForceRelease;
        public UnityEvent OnSqueeze;
        public UnityEvent OnUnsqueeze;

        void Start(){
            hand.OnGrabbed += OnGrabEvent;
            hand.OnReleased += OnReleaseEvent;
            hand.OnForcedRelease += OnForceReleaseEvent;
            hand.OnSqueezed += OnSqueezeEvent;
            hand.OnUnsqueezed += OnUnsqueezeEvent;
        }

        public void OnGrabEvent(Hand hand, Grabbable grab) {
            OnGrab?.Invoke();
        }

        public void OnReleaseEvent(Hand hand, Grabbable grab) {
            OnRelease?.Invoke();
        }

        public void OnSqueezeEvent(Hand hand, Grabbable grab) {
            OnSqueeze?.Invoke();
        }

        public void OnUnsqueezeEvent(Hand hand, Grabbable grab) {
            OnUnsqueeze?.Invoke();
        }

        public void OnForceReleaseEvent(Hand hand, Grabbable grab) {
            OnForceRelease?.Invoke();
        }

        private void OnDrawGizmosSelected() {
            if(hand == null && GetComponent<Hand>())
                hand = GetComponent<Hand>();
        }
    }
}

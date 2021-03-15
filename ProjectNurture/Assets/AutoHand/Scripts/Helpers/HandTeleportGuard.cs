using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand{
    public class HandTeleportGuard : MonoBehaviour{
        [Header("Helps prevent hand from passing through static collision boundries")]
        public Hand hand;
        public float buffer = 0.1f;
        public bool alwaysRun = false;
        
        
        Vector3 deltaHandPos;
        int mask;
        void Awake(){
            if(hand == null && GetComponent<Hand>())
                hand = GetComponent<Hand>();
            
            mask = LayerMask.GetMask(Hand.grabbableLayerNameDefault, Hand.grabbingLayerName, Hand.releasingLayerName, "Hand", "HandHolding");
        }

        void Update(){
            if (alwaysRun){
                if (Vector3.Distance(transform.position, deltaHandPos) > buffer)
                    TeleportProtection(deltaHandPos, transform.position);
                deltaHandPos = hand.transform.position;
            }
        }

        /// <summary>Should be called just after a teleportation</summary>
        public void TeleportProtection(Vector3 fromPos, Vector3 toPos) {
            if(hand.gameObject.activeInHierarchy) {
                RaycastHit[] hits = Physics.RaycastAll(fromPos, hand.transform.position-fromPos, Vector3.Distance(fromPos, hand.transform.position), ~mask);
                Vector3 handPos = -Vector3.one;
                foreach(var hit in hits) {
                    if(hit.transform != hand.transform && hit.transform.GetComponent<Rigidbody>() == null) {
                        handPos = Vector3.MoveTowards(hit.point, hand.transform.position, buffer);
                    }
                }
                if(handPos != -Vector3.one)
                    hand.SetHandLocation(handPos, hand.transform.rotation);
            }
        }
    }
}

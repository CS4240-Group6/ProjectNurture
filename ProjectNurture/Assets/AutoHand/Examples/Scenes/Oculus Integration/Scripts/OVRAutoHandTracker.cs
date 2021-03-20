using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand.Demo{
    public class OVRAutoHandTracker : MonoBehaviour{
    
        OVRSkeleton skeleton;
        float indexBend;
        float middleBend;
        float ringBend;
        float pinkyBend;
        float thumbBend;
        
        Quaternion lastIndexFingerRot1;
        Quaternion lastIndexFingerRot2;
        Quaternion lastMiddleFingerRot1;
        Quaternion lastMiddleFingerRot2;
        Quaternion lastRingFingerRot1;
        Quaternion lastRingFingerRot2;
        Quaternion lastPinkyFingerRot1;
        Quaternion lastPinkyFingerRot2;
        Quaternion lastThumbFingerRot1;
        Quaternion lastThumbFingerRot2;
        Quaternion lastThumbFingerRot3;
        
        float GetFingerCurl(OVRFingerEnum finger) {
            switch(finger){
                case OVRFingerEnum.index:
                    return indexBend*0.8f;
                case OVRFingerEnum.middle:
                    return middleBend*0.8f;
                case OVRFingerEnum.ring:
                    return ringBend*0.8f;
                case OVRFingerEnum.pinky:
                    return pinkyBend*0.8f;
                case OVRFingerEnum.thumb:
                    return thumbBend*0.8f;
            };

            return 1;
        }

        //YOU CAN INCREASE THE FINGER BEND SPEED BY INCREASING THE fingerSmoothSpeed VALUE ON EACH FINGER
        //YOU CAN DISABLE FINGER SWAY BY TURNING SWAY STRENGTH ON HAND TO 0 OR DISABLEIK ENABLED

        public Hand hand;
        
        [Header("Bend Fingers")]
        public Finger thumb;
        public Finger index;
        public Finger middle;
        public Finger ring;
        public Finger pinky;
        [Tooltip("Allows fingers to move while holding an object"), Space]
        public bool freeFingers = true;

        
        [Header("Grab Action"), Space]
        [Tooltip("The required fingers to be bent to the required finger bend to call the grab event, all of these fingers needs to be past the required bend value [Represents AND]")]
        public FingerBend[] grabFingersRequired;

        [Header("Squeeze Action"), Space]
        [Tooltip("The required fingers to be bent to the required finger bend to call the squeeze event, all of these fingers needs to be past the required bend value [Represents AND]")]
        public FingerBend[] squeezeFingersRequired;
        
        bool grabbing;
        bool squeezing;

        Transform follow;
        Transform moveTo;
        Vector3 lastMoveTo;
        CollisionDetectionMode mode;

        void Start(){
            skeleton = GetComponent<OVRSkeleton>();
            moveTo = new GameObject().transform;
            follow = hand.follow;
            lastMoveTo = moveTo.transform.position;
            moveTo.transform.position = follow.transform.position;
            moveTo.transform.rotation = follow.transform.rotation;
            hand.follow = moveTo;
            mode = hand.GetComponent<Rigidbody>().collisionDetectionMode;
            hand.disableIK = true;
        }
    
        void FixedUpdate(){
            if(hand.IsGrabbing())
                return;

            if(!skeleton.IsDataHighConfidence){
                moveTo.transform.position = lastMoveTo;
                var body = hand.GetComponent<Rigidbody>();
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                body.isKinematic = true;
                return;
            }
            else{
                var body = hand.GetComponent<Rigidbody>();
                body.isKinematic = false;
                body.collisionDetectionMode = mode;
                lastMoveTo = moveTo.transform.position;
                moveTo.transform.position = follow.transform.position;
                moveTo.transform.rotation = follow.transform.rotation;
            }

            foreach(OVRBone bone in skeleton.Bones) {
                if (bone.Id == OVRSkeleton.BoneId.Hand_Index1) {
                    indexBend += lastIndexFingerRot1.z - bone.Transform.localRotation.z;
                    lastIndexFingerRot1 = bone.Transform.localRotation;
                }
                if (bone.Id == OVRSkeleton.BoneId.Hand_Index2) {
                    indexBend += lastIndexFingerRot2.z - bone.Transform.localRotation.z;
                    lastIndexFingerRot2 = bone.Transform.localRotation;
                }
                if (bone.Id == OVRSkeleton.BoneId.Hand_Middle1) {
                    middleBend += lastMiddleFingerRot1.z - bone.Transform.localRotation.z;
                    lastMiddleFingerRot1 = bone.Transform.localRotation;
                }
                if (bone.Id == OVRSkeleton.BoneId.Hand_Middle2) {
                    middleBend += lastMiddleFingerRot2.z - bone.Transform.localRotation.z;
                    lastMiddleFingerRot2 = bone.Transform.localRotation;
                }
                if (bone.Id == OVRSkeleton.BoneId.Hand_Ring1) {
                    ringBend += lastRingFingerRot1.z - bone.Transform.localRotation.z;
                    lastRingFingerRot1 = bone.Transform.localRotation;
                }
                if (bone.Id == OVRSkeleton.BoneId.Hand_Ring2) {
                    ringBend += lastRingFingerRot2.z - bone.Transform.localRotation.z;
                    lastRingFingerRot2 = bone.Transform.localRotation;
                }
                if (bone.Id == OVRSkeleton.BoneId.Hand_Pinky1) {
                    pinkyBend += lastPinkyFingerRot1.z - bone.Transform.localRotation.z;
                    lastPinkyFingerRot1 = bone.Transform.localRotation;
                }
                if (bone.Id == OVRSkeleton.BoneId.Hand_Pinky2) {
                    pinkyBend += lastPinkyFingerRot2.z - bone.Transform.localRotation.z;
                    lastPinkyFingerRot2 = bone.Transform.localRotation;
                }
                if (bone.Id == OVRSkeleton.BoneId.Hand_Thumb1) {
                    thumbBend += (lastThumbFingerRot1.z - bone.Transform.localRotation.z);
                    lastThumbFingerRot1 = bone.Transform.localRotation;
                }
                if (bone.Id == OVRSkeleton.BoneId.Hand_Thumb2) {
                    thumbBend += lastThumbFingerRot2.z - bone.Transform.localRotation.z;
                    lastThumbFingerRot2 = bone.Transform.localRotation;
                }
                if (bone.Id == OVRSkeleton.BoneId.Hand_Thumb3) {
                    thumbBend += lastThumbFingerRot3.z - bone.Transform.localRotation.z;
                    lastThumbFingerRot3 = bone.Transform.localRotation;
                }
            }
            
            bool grab = IsGrabbing();
            if(!grabbing && grab) {
                grabbing = true;
                hand.Grab();
            }

            if(grabbing && !grab) {
                grabbing = false;
                hand.Release();
            }


            bool squeeze = IsSqueezing();
            if(!squeezing && squeeze) {
                squeezing = true;
                hand.Squeeze();
            }

            if(squeezing && !squeeze) {
                squeezing = false;
                hand.Unsqueeze();
            }
            
            if(hand.holdingObj == null) {
                thumb.bendOffset = GetFingerCurl(OVRFingerEnum.thumb);
                index.bendOffset = GetFingerCurl(OVRFingerEnum.index);
                middle.bendOffset = GetFingerCurl(OVRFingerEnum.middle);
                ring.bendOffset = GetFingerCurl(OVRFingerEnum.ring);
                pinky.bendOffset = GetFingerCurl(OVRFingerEnum.pinky);

                thumb.UpdateFinger();
                index.UpdateFinger();
                middle.UpdateFinger();
                ring.UpdateFinger();
                pinky.UpdateFinger();
            }
            else if(freeFingers && hand.holdingObj.GetComponent<GrabbablePose>() == null){
                thumb.bendOffset = thumb.GetLastHitBend();
                index.bendOffset = index.GetLastHitBend();
                middle.bendOffset = middle.GetLastHitBend();
                ring.bendOffset = ring.GetLastHitBend();
                pinky.bendOffset = pinky.GetLastHitBend();

                if(GetFingerCurl(OVRFingerEnum.thumb) < thumb.GetLastHitBend())
                    thumb.bendOffset = GetFingerCurl(OVRFingerEnum.thumb);

                if(GetFingerCurl(OVRFingerEnum.index) < index.GetLastHitBend())
                    index.bendOffset = GetFingerCurl(OVRFingerEnum.index);

                if(GetFingerCurl(OVRFingerEnum.middle) < middle.GetLastHitBend())
                    middle.bendOffset = GetFingerCurl(OVRFingerEnum.middle);

                if(GetFingerCurl(OVRFingerEnum.ring) < ring.GetLastHitBend())
                    ring.bendOffset = GetFingerCurl(OVRFingerEnum.ring);

                if(GetFingerCurl(OVRFingerEnum.pinky) < pinky.GetLastHitBend())
                    pinky.bendOffset = GetFingerCurl(OVRFingerEnum.pinky);

                thumb.UpdateFinger();
                index.UpdateFinger();
                middle.UpdateFinger();
                ring.UpdateFinger();
                pinky.UpdateFinger();
            }
            
        }
        



        public bool IsGrabbing(){
            bool requiredFingers = true;
            
            if(grabFingersRequired.Length == 0)
                requiredFingers = false;
            else
                for (int i = 0; i < grabFingersRequired.Length; i++){
                    if(GetFingerCurl(grabFingersRequired[i].finger) < grabFingersRequired[i].amount){
                        requiredFingers = false;
                    }
                }

            return requiredFingers;
        }


        public bool IsSqueezing(){
            bool requiredFingers = true;
            
            if(squeezeFingersRequired.Length == 0)
                requiredFingers = false;
            else
                for (int i = 0; i < squeezeFingersRequired.Length; i++){
                    if (GetFingerCurl(squeezeFingersRequired[i].finger) < squeezeFingersRequired[i].amount){
                        requiredFingers = false;
                    }
                }

            return requiredFingers;
        }
    }

    public enum OVRFingerEnum {
        index,
        middle,
        ring,
        pinky,
        thumb
    }

    [System.Serializable]
    public struct FingerBend {
        public float amount;
        public OVRFingerEnum finger;
    }
}

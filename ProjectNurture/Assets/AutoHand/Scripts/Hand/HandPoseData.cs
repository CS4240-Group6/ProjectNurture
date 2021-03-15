using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand {
    [System.Serializable]
    public struct HandPoseData{
        public Vector3 handOffset;
        //This value is deprectaed as of V1.4 and replaced with localQuaternionOffset
        public Vector3 rotationOffset;
        public Quaternion localQuaternionOffset;
        public Vector3[] posePositions;
        public Quaternion[] poseRotations;
        
        /// <summary>Creates a new pose using the current hand relative to a given grabbable</summary>
        public HandPoseData(Hand hand, Grabbable grabbable) {
            posePositions = new Vector3[0];
            poseRotations = new Quaternion[0];
            handOffset = new Vector3();
            rotationOffset = Vector3.zero;
            localQuaternionOffset = Quaternion.identity;
            SavePose(hand, grabbable.transform);
        }

        /// <summary>Creates a new pose using the current hand relative to a given grabbable</summary>
        public HandPoseData(Hand hand, Transform point) {
            posePositions = new Vector3[0];
            poseRotations = new Quaternion[0];
            handOffset = new Vector3();
            rotationOffset = Vector3.zero;
            localQuaternionOffset = Quaternion.identity;
            SavePose(hand, point);
        }
        
        /// <summary>Creates a new pose using the current hand shape</summary>
        public HandPoseData(Hand hand) {
            posePositions = new Vector3[0];
            poseRotations = new Quaternion[0];
            handOffset = new Vector3();
            rotationOffset = Vector3.zero;
            localQuaternionOffset = Quaternion.identity;
            SavePose(hand, null);
        }




        public void SavePose(Hand hand, Transform relativeTo) {
            var posePositionsList = new List<Vector3>();
            var poseRotationsList = new List<Quaternion>();
            
            if(relativeTo != null){
                var handParent = hand.transform.parent;
                hand.transform.parent = relativeTo;
                handOffset = hand.transform.localPosition;
                localQuaternionOffset = hand.transform.localRotation;
                hand.transform.parent = handParent;
            }
            else {
                handOffset = hand.transform.localPosition;
                localQuaternionOffset = hand.transform.localRotation;
            }

            foreach(var finger in hand.fingers) {
                AssignChildrenPose(finger.transform);
            }

            void AssignChildrenPose(Transform obj) {
                AddPoint(obj.localPosition, obj.localRotation);
                for(int j = 0; j < obj.childCount; j++) {
                    AssignChildrenPose(obj.GetChild(j));
                }
            }

            void AddPoint(Vector3 pos, Quaternion rot) {
                posePositionsList.Add(pos);
                poseRotationsList.Add(rot);
            }
            
            posePositions = new Vector3[posePositionsList.Count];
            poseRotations = new Quaternion[posePositionsList.Count];
            for(int i = 0; i < posePositionsList.Count; i++) {
                posePositions[i] = posePositionsList[i];
                poseRotations[i] = poseRotationsList[i];
            }
        }



        public void SetPose(Hand hand, Transform relativeTo){
            //This might prevent static poses from breaking from the update
            if(rotationOffset != Vector3.zero)
                localQuaternionOffset = Quaternion.Euler(rotationOffset);

            if (relativeTo != null && relativeTo != hand.transform){
                var handParent = hand.transform.parent;
                var originalScale = hand.transform.localScale;
                hand.transform.parent = relativeTo;
                hand.transform.localPosition = handOffset;
                hand.transform.localRotation = localQuaternionOffset;
                hand.transform.parent = handParent;
                hand.transform.localScale = originalScale;
            }

            int i = -1;
            void AssignChildrenPose(Transform obj, HandPoseData pose) {
                i++;
                obj.localPosition = pose.posePositions[i];
                obj.localRotation = pose.poseRotations[i];
                for(int j = 0; j < obj.childCount; j++) {
                    AssignChildrenPose(obj.GetChild(j), pose);
                }
            }

            foreach(var finger in hand.fingers) {
                AssignChildrenPose(finger.transform, this);
            }
        }

        public static HandPoseData LerpPose(HandPoseData from, HandPoseData to, float point) {
            var lerpPose = new HandPoseData();
            lerpPose.handOffset = Vector3.Lerp(from.handOffset, to.handOffset, point);
            lerpPose.localQuaternionOffset = Quaternion.Lerp(from.localQuaternionOffset, to.localQuaternionOffset, point);
            lerpPose.posePositions = new Vector3[from.posePositions.Length];
            lerpPose.poseRotations = new Quaternion[from.poseRotations.Length];

            for(int i = 0; i < from.posePositions.Length; i++) {
                lerpPose.posePositions[i] = Vector3.Lerp(from.posePositions[i], to.posePositions[i], point);
                lerpPose.poseRotations[i] = Quaternion.Lerp(from.poseRotations[i], to.poseRotations[i], point);
            }

            return lerpPose;
        }
    }
}

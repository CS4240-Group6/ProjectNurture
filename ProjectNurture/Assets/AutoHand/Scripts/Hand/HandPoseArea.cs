using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand{
    public class HandPoseArea : MonoBehaviour{

        public float transitionTime = 0.2f;
#if UNITY_EDITOR
        [Header("Editor")]
        [Tooltip("Used to pose for the grabbable")]
        public Hand editorHand;
#endif


        [HideInInspector]
        public HandPoseData rightPose;
        [HideInInspector]
        public bool rightPoseSet = false;
        [HideInInspector]
        public HandPoseData leftPose;
        [HideInInspector]
        public bool leftPoseSet = false;


        public HandPoseData GetHandPoseData(bool left) {
            return (left) ? leftPose : rightPose;
        }


        public void SetHandPose(Hand hand) {
            HandPoseData pose;
            if(hand.left){
                if(leftPoseSet) pose = leftPose;
                else return;
            }
            else{
                if(rightPoseSet) pose = rightPose;
                else return;
            }

            pose.SetPose(hand, transform);
        }
        
#if UNITY_EDITOR
        //This is because parenting is used at runtime, but cannot be used on prefabs in editor so a copy is required
        public void EditorCreateCopySetPose(Hand hand) {
            HandPoseData pose = new HandPoseData();

            if(hand.left && leftPoseSet)
                pose = leftPose;
            else if(!hand.left && rightPoseSet)
                pose = rightPose;

            var handParent = hand.transform.parent;
            Hand handCopy;
            if (hand.name != "HAND COPY DELETE")
                handCopy = Instantiate(hand, hand.transform.position, hand.transform.rotation);
            else
                handCopy = hand;

            handCopy.name = "HAND COPY DELETE";
            editorHand = handCopy;
            
            pose.SetPose(handCopy, transform);
        }

        public void EditorSaveGrabPose(Hand hand, bool left){
            var pose = new HandPoseData();
            
            var posePositionsList = new List<Vector3>();
            var poseRotationsList = new List<Quaternion>();
            
            var handCopy = Instantiate(hand, hand.transform.position, hand.transform.rotation);
            var handParent = handCopy.transform.parent;
            handCopy.transform.parent = transform;
            pose.handOffset = handCopy.transform.localPosition;
            pose.localQuaternionOffset = handCopy.transform.localRotation;
            DestroyImmediate(handCopy.gameObject);

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
            
            pose.posePositions = new Vector3[posePositionsList.Count];
            pose.poseRotations = new Quaternion[posePositionsList.Count];
            for(int i = 0; i < posePositionsList.Count; i++) {
                pose.posePositions[i] = posePositionsList[i];
                pose.poseRotations[i] = poseRotationsList[i];
            }

            if(left){
                leftPose = pose;
                leftPoseSet = true;
                Debug.Log("Pose Saved - Left");
            }
            else{
                rightPose = pose;
                rightPoseSet = true;
                Debug.Log("Pose Saved - Right");
            }
        }
        
        public void EditorClearPoses() {
            leftPoseSet = false;
            rightPoseSet = false;
        }
#endif

        public bool HasPose(bool left) {
            return left ? leftPoseSet : rightPoseSet;
        }
    }
}

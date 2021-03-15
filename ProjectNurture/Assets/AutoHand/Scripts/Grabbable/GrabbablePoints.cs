using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand{
    
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class GrabbablePoints : GrabbablePointBase{
        public Transform[] leftPoints;
        public Transform[] rightPoints;

#if UNITY_EDITOR
        [Header("Editor")]
        public Hand editorHand;
        public int index = 0;
        Vector3 preEditorPos;
        Hand lastEditorHand;

        void Start() {
            editorHand = null;
        }

        void Update() {
            if(lastEditorHand != editorHand) {
                if(lastEditorHand != null)
                    lastEditorHand.transform.position = preEditorPos;

                preEditorPos = editorHand.transform.position;
            }

            if(editorHand != null)
                EditorAlign(editorHand, index);

            lastEditorHand = editorHand;
        }

        public bool EditorAlign(Hand hand, int i) {
            if(hand.left && leftPoints.Length > 0){
                hand.transform.position = leftPoints[i].position;
                hand.transform.rotation = leftPoints[i].rotation;
                return true;
            }
            else if(!hand.left && rightPoints.Length > 0){
                hand.transform.position = rightPoints[i].position;
                hand.transform.rotation = rightPoints[i].rotation;
                return true;
            }

            return false;
        }
#endif
        
        public override bool Align(Hand hand) {
            if(hand.left && leftPoints.Length > 0){
                var left = ClosestGrabPoint(leftPoints, hand);
                hand.transform.position = left.position;
                hand.transform.rotation = left.rotation;
                return true;
            }
            else if(!hand.left && rightPoints.Length > 0){
                var right = ClosestGrabPoint(rightPoints, hand);
                hand.transform.position = right.position;
                hand.transform.rotation = right.rotation;
                return true;
            }

            return false;
        }

        /// <summary>Checks all the predetermined points assigned to a grabbable and returns closest point and orientation to the hand</summary>
        Transform ClosestGrabPoint(Transform[] points, Hand hand) {
            int closestIndex = 0;
            float dist = (Vector3.Distance(hand.transform.position, points[0].position) * 10000 + Mathf.Abs(Quaternion.Angle(hand.transform.rotation, points[0].rotation)));
            for(int i = 1; i < points.Length; i++) {
                var newDist = Vector3.Distance(hand.transform.position, points[i].position) * 10000 + Mathf.Abs(Quaternion.Angle(hand.transform.rotation, points[i].rotation));
                if(newDist < dist) {
                    closestIndex = i;
                    dist = newDist;
                }
            }
            return points[closestIndex];
        }
    }
}

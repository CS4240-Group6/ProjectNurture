using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Autohand{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class GrabbablePoint : GrabbablePointBase{
        public Transform rightPoint;
        public Transform leftPoint;


#if UNITY_EDITOR
        [Header("Editor")]
        public Hand editorHand;
        Vector3 preEditorPos;
        Hand lastEditorHand;

        void Start() {
            editorHand = null;
        }

        void Update() {
            if(editorHand != null){
                if(lastEditorHand != editorHand) {
                    if(lastEditorHand != null)
                        lastEditorHand.transform.position = preEditorPos;

                    preEditorPos = editorHand.transform.position;
                }

                Align(editorHand);

                lastEditorHand = editorHand;
            }
            else if(lastEditorHand != null){
                lastEditorHand.transform.position = preEditorPos;
                lastEditorHand = null;
            }

        }
#endif

        public override bool Align(Hand hand) {
            if(hand.left && leftPoint != null){
                hand.transform.position = leftPoint.position;
                hand.transform.rotation = leftPoint.rotation;
                return true;
            }
            else if(!hand.left && rightPoint != null){
                hand.transform.position = rightPoint.position;
                hand.transform.rotation = rightPoint.rotation;
                return true;
            }

            return false;
        }
        
#if UNITY_EDITOR
        [ContextMenu("Test Align")]
        public void EditorAlign(){
            Align(editorHand);
        }
#endif
    }
}

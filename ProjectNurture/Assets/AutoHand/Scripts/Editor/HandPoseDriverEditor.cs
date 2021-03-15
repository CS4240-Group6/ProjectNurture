using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace Autohand {
    [CustomEditor(typeof(HandPoseDriver))]
    public class HandPoseDriverEditor : Editor{
		HandPoseDriver poseDriver;

        private void OnEnable() {
            poseDriver = target as HandPoseDriver;
        }

        public override void OnInspectorGUI() {
            if(poseDriver.gameObject.scene.name == null){
                EditorGUILayout.LabelField("This must be saved in the scene");
                EditorGUILayout.LabelField("-> then use override to prefab");
                return;
            }

            if(poseDriver.gameObject != null && PrefabStageUtility.GetPrefabStage(poseDriver.gameObject) == null){
                DrawDefaultInspector();
                EditorUtility.SetDirty(poseDriver);

                EditorGUILayout.Space();
                EditorGUILayout.Space();


                var rect = EditorGUILayout.GetControlRect();
                if(poseDriver.rightPoseSet)
                    EditorGUI.DrawRect(rect, Color.green);
                else
                    EditorGUI.DrawRect(rect, Color.red);

                rect.width -= 4;
                rect.height -= 2;
                rect.x += 2;
                rect.y += 1;

                if(GUI.Button(rect, "Save Right Pose"))
                    poseDriver.EditorSaveGrabPose(poseDriver.editorHand, false);


                rect = EditorGUILayout.GetControlRect();
                if(poseDriver.leftPoseSet)
                    EditorGUI.DrawRect(rect, Color.green);
                else
                    EditorGUI.DrawRect(rect, Color.red);

                rect.x += 2;
                rect.y += 1;
                rect.width -= 4;
                rect.height -= 2;

                if (GUI.Button(rect, "Save Left Pose"))
                    poseDriver.EditorSaveGrabPose(poseDriver.editorHand, true);


                rect = EditorGUILayout.GetControlRect();
                if (GUI.Button(rect, "Save Both Pose")){
                    poseDriver.EditorSaveGrabPose(poseDriver.editorHand, false);
                    poseDriver.EditorSaveGrabPose(poseDriver.editorHand, true);
                }




                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            
                GUILayout.Label(new GUIContent("-------- For tweaking poses --------"), new GUIStyle(){ fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
                GUILayout.Label(new GUIContent("This will create a copy that should be deleted"), new GUIStyle(){ fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });


                if (GUILayout.Button("Create Copy - Set Pose")){
                    EditorGUIUtility.PingObject(poseDriver.editorHand);
                    poseDriver.EditorCreateCopySetPose(poseDriver.editorHand);
                }

                if (GUILayout.Button("Reset Hand"))
                    poseDriver.editorHand.RelaxHand();

                EditorGUILayout.Space();
                rect = EditorGUILayout.GetControlRect();
                EditorGUI.DrawRect(rect, Color.red);

                if(GUILayout.Button("Delete Copy")){
                    if(string.Equals(poseDriver.editorHand.name, "HAND COPY DELETE"))
                        DestroyImmediate(poseDriver.editorHand.gameObject);
                    else
                        Debug.LogError("Not a copy - Will not delete");
                }
                if(GUILayout.Button("Clear Poses")){
                    poseDriver.EditorClearPoses();
                }
            }
            else {
                GUILayout.Label(new GUIContent(" - This will not work in prefab mode - "), new GUIStyle(){ fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
                GUILayout.Label(new GUIContent("Use scene to create poses"), new GUIStyle(){ alignment = TextAnchor.MiddleCenter });
            }
        }
    }
}

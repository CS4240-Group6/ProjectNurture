using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;



namespace Autohand {
    [CustomEditor(typeof(HandPoseArea))]
    public class HandPoseAreaEditor : Editor{
		HandPoseArea areaPose;

        private void OnEnable() {
            areaPose = target as HandPoseArea;
        }

        public override void OnInspectorGUI() {
            if(areaPose.gameObject.scene.name == null){
                EditorGUILayout.LabelField("This must be saved in the scene");
                EditorGUILayout.LabelField("-> then use override to prefab");
                return;
            }

            if(areaPose.gameObject != null && PrefabStageUtility.GetPrefabStage(areaPose.gameObject) == null){
                DrawDefaultInspector();
                EditorUtility.SetDirty(areaPose);

                EditorGUILayout.Space();
                EditorGUILayout.Space();


                var rect = EditorGUILayout.GetControlRect();
                if(areaPose.rightPoseSet)
                    EditorGUI.DrawRect(rect, Color.green);
                else
                    EditorGUI.DrawRect(rect, Color.red);

                rect.width -= 4;
                rect.height -= 2;
                rect.x += 2;
                rect.y += 1;

                if(GUI.Button(rect, "Save Right Pose"))
                    areaPose.EditorSaveGrabPose(areaPose.editorHand, false);


                rect = EditorGUILayout.GetControlRect();
                if(areaPose.leftPoseSet)
                    EditorGUI.DrawRect(rect, Color.green);
                else
                    EditorGUI.DrawRect(rect, Color.red);

                rect.x += 2;
                rect.y += 1;
                rect.width -= 4;
                rect.height -= 2;

                if (GUI.Button(rect, "Save Left Pose"))
                    areaPose.EditorSaveGrabPose(areaPose.editorHand, true);


                rect = EditorGUILayout.GetControlRect();
                if (GUI.Button(rect, "Save Both Pose")){
                    areaPose.EditorSaveGrabPose(areaPose.editorHand, false);
                    areaPose.EditorSaveGrabPose(areaPose.editorHand, true);
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
                    EditorGUIUtility.PingObject(areaPose.editorHand);
                    areaPose.EditorCreateCopySetPose(areaPose.editorHand);
                }

                if (GUILayout.Button("Reset Hand"))
                    areaPose.editorHand.RelaxHand();

                EditorGUILayout.Space();
                rect = EditorGUILayout.GetControlRect();
                EditorGUI.DrawRect(rect, Color.red);

                if(GUILayout.Button("Delete Copy")){
                    if(string.Equals(areaPose.editorHand.name, "HAND COPY DELETE"))
                        DestroyImmediate(areaPose.editorHand.gameObject);
                    else
                        Debug.LogError("Not a copy - Will not delete");
                }
                if(GUILayout.Button("Clear Poses")){
                    areaPose.EditorClearPoses();
                }
            }
            else {
                GUILayout.Label(new GUIContent(" - This will not work in prefab mode - "), new GUIStyle(){ fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
                GUILayout.Label(new GUIContent("Use scene to create poses"), new GUIStyle(){ alignment = TextAnchor.MiddleCenter });
            }
        }
    }
}

using UnityEditor;
using UnityEngine;

namespace Autohand {
    [CustomEditor(typeof(Grabbable)), CanEditMultipleObjects]
    public class GrabbableEditor : Editor {
        Grabbable grabbable;
        GUIStyle headerStyle;

        private void OnEnable() {
            headerStyle = new GUIStyle(){ fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
            grabbable = target as Grabbable;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            if(grabbable.transform.childCount > 0) {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("makeChildrenGrabbable"));
            }
            
            EditorGUILayout.LabelField(new GUIContent("Break Settings"), headerStyle);
            EditorGUI.BeginDisabledGroup(grabbable.singleHandOnly);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pullApartBreakOnly"));
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("jointBreakForce"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("jointBreakTorque"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("Unity Events"), headerStyle);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hideEvents"));
            if(!grabbable.hideEvents) {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onGrab"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onRelease"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onSqueeze"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onUnsqueeze"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnJointBreak"));
            }
            serializedObject.ApplyModifiedProperties();

        }
    }
}

using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace HarryUtils.UI.Editor {
    [CanEditMultipleObjects, CustomEditor(typeof(NonDrawingGraphic), false)]
    public class NonDrawingGraphicEditor : GraphicEditor {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(m_Script, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();

            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
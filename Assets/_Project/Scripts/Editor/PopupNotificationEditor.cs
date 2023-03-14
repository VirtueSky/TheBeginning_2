#if UNITY_EDITOR
using Pancake.GameService;
using Pancake.UI;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Pancake.Editor
{
    [CustomEditor(typeof(PopupNotification))]
    public class PopupNotificationEditor : UnityEditor.Editor
    {
        private SerializedProperty _txtMessage;
        private SerializedProperty _btnOk;

        protected virtual void OnEnable()
        {
            _txtMessage = serializedObject.FindProperty("txtMessage");
            _btnOk = serializedObject.FindProperty("btnOk");
        }

        protected virtual void OnDrawExtraSetting()
        {
            Uniform.SpaceOneLine();
            Uniform.DrawGroupFoldout("UIPOPUP_NOTIFICATION", "NOTIFICATION SETTING", DrawSetting);
        }

        private void DrawSetting()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Message", GUILayout.Width(110));
            _txtMessage.objectReferenceValue = EditorGUILayout.ObjectField(_txtMessage.objectReferenceValue, typeof(TextMeshProUGUI), allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Ok", GUILayout.Width(110));
            _btnOk.objectReferenceValue = EditorGUILayout.ObjectField(_btnOk.objectReferenceValue, typeof(UIButton), allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            OnDrawExtraSetting();
            Repaint();
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
}
#endif
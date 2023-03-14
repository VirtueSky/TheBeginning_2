#if UNITY_EDITOR
using Pancake.GameService;
using Pancake.UI;
using UnityEditor;
using UnityEngine;

namespace Pancake.Editor
{
    [CustomEditor(typeof(PopupNoInternet))]
    public class PopupNoInternetEditor : UnityEditor.Editor
    {
        private SerializedProperty _btnOk;

        protected virtual void OnEnable()
        {
            _btnOk = serializedObject.FindProperty("btnOk");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            OnDrawExtraSetting();
            Repaint();
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
        
        protected virtual void OnDrawExtraSetting()
        {
            Uniform.SpaceOneLine();
            Uniform.DrawGroupFoldout("UIPOPUP_NOINTERNET", "NO INTERNET SETTING", DrawSetting);
        }

        private void DrawSetting()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Ok", GUILayout.Width(110));
            _btnOk.objectReferenceValue = EditorGUILayout.ObjectField(_btnOk.objectReferenceValue, typeof(UIButton), allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();
        } 
    }
}

#endif
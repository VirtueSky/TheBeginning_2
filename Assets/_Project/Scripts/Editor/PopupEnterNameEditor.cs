#if UNITY_EDITOR
using Pancake.GameService;
using Pancake.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Pancake.Editor
{
    [CustomEditor(typeof(PopupEnterName))]
    public class PopupEnterNameEditor : UnityEditor.Editor
    {
        private SerializedProperty _elementPrefab;
        private SerializedProperty _countryCode;
        private SerializedProperty _btnSpriteLocked;
        private SerializedProperty _ipfEnterName;
        private SerializedProperty _btnCountry;
        private SerializedProperty _btnOk;
        private SerializedProperty _scroller;
        private SerializedProperty _txtWarning;
        private SerializedProperty _imgCurrentCountryIcon;
        private SerializedProperty _txtCurrentCountryName;
        private SerializedProperty _block;
        private SerializedProperty _selectCountryPopup;

        private const int DEFAULT_LABEL_WIDTH = 110;

        protected virtual void OnEnable()
        {
            _elementPrefab = serializedObject.FindProperty("elementPrefab");
            _countryCode = serializedObject.FindProperty("countryCode");
            _btnSpriteLocked = serializedObject.FindProperty("btnSpriteLocked");
            _ipfEnterName = serializedObject.FindProperty("ipfEnterName");
            _btnCountry = serializedObject.FindProperty("btnCountry");
            _btnOk = serializedObject.FindProperty("btnOk");
            _scroller = serializedObject.FindProperty("scroller");
            _txtWarning = serializedObject.FindProperty("txtWarning");
            _imgCurrentCountryIcon = serializedObject.FindProperty("imgCurrentCountryIcon");
            _txtCurrentCountryName = serializedObject.FindProperty("txtCurrentCountryName");
            _block = serializedObject.FindProperty("block");
            _selectCountryPopup = serializedObject.FindProperty("selectCountryPopup");
        }

        protected virtual void OnDrawExtraSetting()
        {
            Uniform.SpaceOneLine();
            Uniform.DrawGroupFoldout("UIPOPUP_LOGIN", "LOGIN SETTING", DrawSetting);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            OnDrawExtraSetting();
            Repaint();
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private void DrawSetting()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Prefab", GUILayout.Width(DEFAULT_LABEL_WIDTH));
            _elementPrefab.objectReferenceValue = EditorGUILayout.ObjectField(_elementPrefab.objectReferenceValue,
                typeof(CountryView), allowSceneObjects: false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Country Code", GUILayout.Width(DEFAULT_LABEL_WIDTH));
            _countryCode.objectReferenceValue = EditorGUILayout.ObjectField(_countryCode.objectReferenceValue,
                typeof(CountryCode), allowSceneObjects: false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Sprite Lock", GUILayout.Width(DEFAULT_LABEL_WIDTH));
            _btnSpriteLocked.objectReferenceValue = EditorGUILayout.ObjectField(_btnSpriteLocked.objectReferenceValue,
                typeof(Sprite), allowSceneObjects: false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Input Field", GUILayout.Width(DEFAULT_LABEL_WIDTH));
            _ipfEnterName.objectReferenceValue = EditorGUILayout.ObjectField(_ipfEnterName.objectReferenceValue,
                typeof(TMP_InputField), allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Button Country", GUILayout.Width(DEFAULT_LABEL_WIDTH));
            _btnCountry.objectReferenceValue = EditorGUILayout.ObjectField(_btnCountry.objectReferenceValue,
                typeof(UIButton), allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Button Ok", GUILayout.Width(DEFAULT_LABEL_WIDTH));
            _btnOk.objectReferenceValue =
                EditorGUILayout.ObjectField(_btnOk.objectReferenceValue, typeof(Button), allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Text Warning", GUILayout.Width(DEFAULT_LABEL_WIDTH));
            _txtWarning.objectReferenceValue = EditorGUILayout.ObjectField(_txtWarning.objectReferenceValue,
                typeof(TextMeshProUGUI), allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Name Country", GUILayout.Width(DEFAULT_LABEL_WIDTH));
            _txtCurrentCountryName.objectReferenceValue =
                EditorGUILayout.ObjectField(_txtCurrentCountryName.objectReferenceValue, typeof(TextMeshProUGUI),
                    allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Icon Country", GUILayout.Width(DEFAULT_LABEL_WIDTH));
            _imgCurrentCountryIcon.objectReferenceValue =
                EditorGUILayout.ObjectField(_imgCurrentCountryIcon.objectReferenceValue, typeof(Image),
                    allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Country Popup", GUILayout.Width(DEFAULT_LABEL_WIDTH));
            _selectCountryPopup.objectReferenceValue =
                EditorGUILayout.ObjectField(_selectCountryPopup.objectReferenceValue, typeof(RectTransform),
                    allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Block", GUILayout.Width(DEFAULT_LABEL_WIDTH));
            _block.objectReferenceValue = EditorGUILayout.ObjectField(_block.objectReferenceValue, typeof(Transform),
                allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Scroller", GUILayout.Width(DEFAULT_LABEL_WIDTH));
            _scroller.objectReferenceValue = EditorGUILayout.ObjectField(_scroller.objectReferenceValue,
                typeof(EnhancedScroller), allowSceneObjects: true);
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif
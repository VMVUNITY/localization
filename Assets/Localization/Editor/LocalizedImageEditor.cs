using System.Collections.Generic;
using System.IO;
using System.Linq;
using Localization.Components;
using Localization.Config;
using Localization.Enums;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Localization.Editor {
    [CustomEditor(typeof(LocalizedImage))]
    [CanEditMultipleObjects]
    public class LocalizedImageEditor : ImageEditor {
        private LocalizationConfig _localizationConfig;
        private static string _pathToLocalization;
        private static string[] _localizationKeys;

        private static Dictionary<string, string> _localizedTextByKey;

        private int _keyIndex;

        private SerializedProperty _key;


        private void GetLocalizationKeys() {
            var json = File.ReadAllText(_pathToLocalization);

            _localizedTextByKey = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            _localizationKeys = _localizedTextByKey.Keys
                .Where(key => key.Contains(_localizationConfig.spriteLocalizationPrefix)).ToArray();
        }

        private int GetIndexOfKey(string key) {
            for (int i = 0; i < _localizationKeys.Length; i++) {
                if (_localizationKeys[i] != key) {
                    continue;
                }

                return i;
            }

            return 0;
        }

        private void DrawLocalization() {
            serializedObject.Update();

            var currentContentColor = GUI.contentColor;

            GUI.contentColor = Color.cyan;

            _keyIndex = EditorGUILayout.Popup("LocalizationKey", GetIndexOfKey(_key.stringValue), _localizationKeys);

            _key.stringValue = _localizationKeys[_keyIndex];

            GUI.contentColor = currentContentColor;

            serializedObject.ApplyModifiedProperties();
        }

        protected override void OnEnable() {
            base.OnEnable();

            _localizationConfig = Resources.Load<LocalizationConfig>(LocalizationConfig.CONFIG_PATH);

            _key = serializedObject.FindProperty("localizationKey");

            //TODO: Fix this hardcode
            if (_localizationConfig.resourceLocation == LocalizationResourceLocation.Addressables) {
                _pathToLocalization =
                    $"{Application.dataPath}/Localization/{string.Format(_localizationConfig.localizationPath, LocalizationLanguage.English)}.json";
            }
            else {
                _pathToLocalization =
                    $"{Application.dataPath}/Localization/Resources/{string.Format(_localizationConfig.localizationPath, LocalizationLanguage.English)}.json";
            }

            GetLocalizationKeys();
        }

        public override void OnInspectorGUI() {
            DrawLocalization();
            base.OnInspectorGUI();
        }
    }
}
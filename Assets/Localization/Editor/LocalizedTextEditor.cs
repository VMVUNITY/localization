using System.Collections.Generic;
using System.IO;
using Localization.Components;
using Localization.Config;
using Localization.Enums;
using Newtonsoft.Json;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace Localization.Editor {
    [CustomEditor(typeof(LocalizedText))]
    [CanEditMultipleObjects]
    public class LocalizedTextEditor : TMP_EditorPanelUI {
        private static string _pathToLocalization;
        private static string[] _localizationKeys;

        private static Dictionary<string, string> _localizedTextByKey;

        private int _keyIndex;

        private SerializedProperty _key;

        private static LocalizationConfig _localizationConfig;

        private void GetLocalizationKeys() {
            var json = File.ReadAllText(_pathToLocalization);

            _localizedTextByKey = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            _localizationKeys = new string[_localizedTextByKey.Count];

            _localizedTextByKey.Keys.CopyTo(_localizationKeys, 0);
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

        protected override void OnEnable() {
            base.OnEnable();

            _localizationConfig = Resources.Load<LocalizationConfig>("LocalizationConfig");

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

        protected override void DrawExtraSettings() {
            DrawLocalization();

            base.DrawExtraSettings();
        }

        private void DrawLocalization() {
            var currentContentColor = GUI.contentColor;

            GUI.contentColor = Color.cyan;

            _keyIndex = EditorGUILayout.Popup("LocalizationKey", GetIndexOfKey(_key.stringValue), _localizationKeys);

            _key.stringValue = _localizationKeys[_keyIndex];

            GUI.contentColor = currentContentColor;
        }
    }
}
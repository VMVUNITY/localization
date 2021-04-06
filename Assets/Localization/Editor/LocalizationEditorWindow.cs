using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Localization.Config;
using Localization.Enums;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Localization.Editor {
    public static class CsvConverter {
        public static string JsonFromCsv(string csv) {
            var splitCsv = csv.Split('\n');

            var localizedTextByKey = new Dictionary<string, string>();

            foreach (var te in splitCsv) {
                //TODO: indicate that this line is not localization
                if (te.Length < 3) {
                    continue;
                }

                var firstIndexOfComma = te.IndexOf(",", StringComparison.Ordinal);

                var key = te.Substring(0, firstIndexOfComma);

                var value = te.Substring(firstIndexOfComma + 1).Replace("\"", string.Empty);

                value = Regex.Replace(value, @"\r\n?|\n", string.Empty);

                localizedTextByKey.Add(key, value);
            }

            return JsonConvert.SerializeObject(localizedTextByKey);
        }
    }

    public class LocalizationEditorWindow : EditorWindow {
        private LocalizationConfig _localizationConfig;

        private static string _localizationPath;

        private string _addedLocalizationKey;
        private string[] _addedTextLocalizations;
        private Sprite[] _addedSpriteLocalizations;


        private static bool _textKeysFoldout;
        private static bool _spriteKeysFoldout;
        private static bool _addTextLocalizationFoldout;
        private static bool _addSpriteLocalizationFoldout;
        private static bool _csvConverterFoldout;
        private static bool _addLocalizationLanguageFoldout;
        private static Vector2 _textKeysScrollPosition;
        private static Vector2 _spriteKeysScrollPosition;
        private static Vector2 _addSpriteLocalizationScrollPosition;


        private static string _addedLocalizationLanguage;

        private static Dictionary<string, Dictionary<string, string>> _localizationByFileName;

        [MenuItem("CandyWings/LocalizationEditor")]
        private static void ShowWindow() {
            GetWindow<LocalizationEditorWindow>();
        }

        private static bool IsInitialized() {
            return Directory.Exists(_localizationPath);
        }

        private void DrawLocalizationKeysPath() {
            EditorGUILayout.LabelField($"Put localization jsons in this path: {_localizationPath}");
        }

        private void DrawInitialize() {
            if (GUILayout.Button("Initialize Localization")) {
                Directory.CreateDirectory(_localizationPath);

                GetLocalizations();
            }
        }

        private void DrawLocalizationsCount() {
            EditorGUILayout.LabelField($"Localizations Count: {_localizationByFileName.Count}");
        }

        private void DrawCheckForAddedLocalizations() {
            if (_localizationByFileName.Count == 0) {
                EditorGUILayout.LabelField(
                    $"Localizations not found. Drag json files to folder\n{_localizationPath}\and press Refresh button");
            }
        }

        private void DrawRefresh() {
            if (GUILayout.Button("Refresh")) {
                GetLocalizations();
            }
        }

        private void DrawAddLocalizationLanguage() {
            _addLocalizationLanguageFoldout =
                EditorGUILayout.Foldout(_addLocalizationLanguageFoldout, "Add LocalizationLanguage");

            if (!_addLocalizationLanguageFoldout) {
                return;
            }

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            _addedLocalizationLanguage = EditorGUILayout.TextField("Localization Language", _addedLocalizationLanguage);

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Add Language")) {
                var enumPath = Path.Combine(Application.dataPath, "Localization/Enums/LocalizationLanguage.cs");

                var text = File.ReadAllText(enumPath);

                var indexOfLastComma = text.LastIndexOf("}\n}", StringComparison.Ordinal);

                text = text.Insert(indexOfLastComma - 5, $",\n\t\t{_addedLocalizationLanguage}");

                File.WriteAllText(enumPath, text);

                AssetDatabase.Refresh();

                _addLocalizationLanguageFoldout = false;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawTextLocalizationKeys() {
            if (_localizationByFileName.Count == 0) {
                return;
            }

            EditorGUILayout.BeginVertical();
            _textKeysFoldout = EditorGUILayout.Foldout(_textKeysFoldout, "Localization Keys");

            if (_textKeysFoldout) {
                _textKeysScrollPosition =
                    EditorGUILayout.BeginScrollView(_textKeysScrollPosition, GUILayout.Height(256));

                foreach (var localizationKey in _localizationByFileName.Values.First().Keys) {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(localizationKey);
                    if (GUILayout.Button("X", GUILayout.Width(32), GUILayout.Height(32))) {
                        DeleteLocalizationWithKey(localizationKey);
                        break;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.EndVertical();
        }

        private void DeleteLocalizationWithKey(string key) {
            if (!EditorUtility.DisplayDialog("Delete Localization", $"Delete Localization with key: {key}", "Yes",
                "No")) {
                return;
            }

            foreach (var localization in _localizationByFileName) {
                localization.Value.Remove(key);

                var filePath = $"{_localizationPath}/{localization.Key}.json";

                File.WriteAllText(filePath, JsonConvert.SerializeObject(localization.Value));
            }
        }

        private void DrawAddSpriteLocalization() {
            _addSpriteLocalizationFoldout =
                EditorGUILayout.Foldout(_addSpriteLocalizationFoldout, "Add Sprite Localization");

            if (_addSpriteLocalizationFoldout) {
                EditorGUILayout.BeginHorizontal();

                _addedLocalizationKey = EditorGUILayout.TextField("Key", _addedLocalizationKey);

                if (GUILayout.Button("Add", GUILayout.Width(32))) {
                    AddSpriteLocalization();
                }

                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginVertical();

                _addSpriteLocalizationScrollPosition =
                    EditorGUILayout.BeginScrollView(_addSpriteLocalizationScrollPosition, GUILayout.Height(128));

                for (int i = 0; i < _addedSpriteLocalizations.Length; i++) {
                    _addedSpriteLocalizations[i] =
                        (Sprite) EditorGUILayout.ObjectField($"{_localizationByFileName.Keys.ToArray()[i]}",
                            _addedSpriteLocalizations[i], typeof(Sprite), true);
                }

                EditorGUILayout.EndScrollView();

                EditorGUILayout.EndVertical();
            }
        }

        private static TextAsset _csvText;

        private LocalizationLanguage _selectedLocalizationLanguage;

        private bool _deleteCsv;

        private void DrawConvertCsv() {
            _csvConverterFoldout =
                EditorGUILayout.Foldout(_csvConverterFoldout, "Convert CSV");

            if (_csvConverterFoldout) {
                _csvText = (TextAsset) EditorGUILayout.ObjectField(_csvText, typeof(TextAsset), true,
                    GUILayout.Width(128), GUILayout.Height(128));

                _selectedLocalizationLanguage =
                    (LocalizationLanguage) EditorGUILayout.EnumPopup("Select Localization",
                        _selectedLocalizationLanguage);

                _deleteCsv = EditorGUILayout.Toggle("Delete CSV", _deleteCsv);

                if (GUILayout.Button("Convert")) {
                    var json = CsvConverter.JsonFromCsv(_csvText.text);

                    var path = EditorUtility.SaveFilePanelInProject(
                        "Save Localization",
                        string.Format(_localizationConfig.localizationPath, _selectedLocalizationLanguage),
                        "json",
                        "Save Localization",
                        $"{Application.dataPath}/Localization/Resources/");

                    if (!string.IsNullOrEmpty(path)) {
                        File.WriteAllText(path, json, Encoding.UTF8);
                    }

                    if (_deleteCsv) {
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_csvText));
                    }

                    _csvText = null;

                    _csvConverterFoldout = false;

                    AssetDatabase.Refresh();
                }
            }
        }

        private void AddSpriteLocalization() {
            var index = 0;

            foreach (var localization in _localizationByFileName) {
                var fileName = localization.Key;
                var filePath = $"{_localizationPath}/{fileName}.json";

                localization.Value.Add($"{_localizationConfig.spriteLocalizationPrefix}{_addedLocalizationKey}",
                    _addedSpriteLocalizations[index++].name);

                File.WriteAllText(filePath, JsonConvert.SerializeObject(localization.Value));
            }

            EditorUtility.DisplayDialog("Localization Added.",
                $"Added sprite localization with Key:{_addedLocalizationKey}", "ok");

            _addedLocalizationKey = string.Empty;
            _addedSpriteLocalizations = new Sprite[_localizationByFileName.Count];
        }

        private void GetLocalizations() {
            var localizations = Directory
                .EnumerateFiles(_localizationPath, "*.json", SearchOption.AllDirectories)
                .Select(ReadLocalizationFile)
                .Select(DeserializeLocalization);

            _localizationByFileName = new Dictionary<string, Dictionary<string, string>>();

            foreach (var localization in localizations) {
                _localizationByFileName.Add(localization.Key, localization.Value);
            }

            _addedSpriteLocalizations = new Sprite[_localizationByFileName.Count];
            _addedTextLocalizations = new string[_localizationByFileName.Count];
        }

        private static KeyValuePair<string, string> ReadLocalizationFile(string filePath) {
            var language = Path.GetFileNameWithoutExtension(filePath);

            var json = File.ReadAllText(filePath);

            return new KeyValuePair<string, string>(language, json);
        }

        private static KeyValuePair<string, Dictionary<string, string>> DeserializeLocalization(
            KeyValuePair<string, string> localization) {
            var key = localization.Key;
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(localization.Value);

            return new KeyValuePair<string, Dictionary<string, string>>(key, dictionary);
        }

        private void DrawCsvConverter() {
        }

        private void OnEnable() {
            _localizationConfig = Resources.Load<LocalizationConfig>(LocalizationConfig.CONFIG_PATH);

            if (_localizationConfig.resourceLocation == LocalizationResourceLocation.Addressables) {
                _localizationPath = $"{Application.dataPath}/Localization";
            }
            else {
                _localizationPath = $"{Application.dataPath}/Localization/Resources";
            }

            if (IsInitialized()) {
                GetLocalizations();
            }
        }

        private void OnGUI() {
            if (!IsInitialized()) {
                DrawInitialize();

                return;
            }

            DrawLocalizationKeysPath();
            DrawLocalizationsCount();
            DrawCheckForAddedLocalizations();
            DrawRefresh();

            DrawAddLocalizationLanguage();

            DrawTextLocalizationKeys();
            DrawAddSpriteLocalization();

            DrawConvertCsv();
        }
    }
}
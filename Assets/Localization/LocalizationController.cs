using System;
using System.Collections.Generic;
using System.ComponentModel;
using Localization.Config;
using Localization.Enums;
using Localization.ResourceProviders;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using Zenject;

namespace Localization {
    public class LocalizationController : ILocalizationProvider, IInitializable {
        private Dictionary<string, string> _localizationDictionary;

        private LocalizationConfig _config;

        private readonly IResourceProvider<string, TextAsset> _textsProvider;
        private readonly IResourceProvider<string, Sprite> _spritesProvider;

        public ReactiveProperty<LocalizationLanguage> Language { get; }

        public LocalizationController(IResourceProvider<string, TextAsset> textsProvider,
            IResourceProvider<string, Sprite> spritesProvider) {
            _textsProvider = textsProvider;
            _spritesProvider = spritesProvider;

            Language = new ReactiveProperty<LocalizationLanguage>(LocalizationLanguage.English);

            Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Space)).Subscribe(_ => {
                SetLocalizationLanguage(LocalizationLanguage.Russian);
            });
        }

        public void Initialize() {
            var configLoader = new DefaultResourceProvider<LocalizationConfig>();

            configLoader.GetResourceByKey(LocalizationConfig.CONFIG_PATH, OnGotLocalizationConfig);
        }

        private void OnGotLocalizationConfig(LocalizationConfig config) {
            _config = config;

            LoadLocalizationTextAssetForLanguage(LocalizationLanguage.English);
        }

        public void SetLocalizationLanguage(LocalizationLanguage language) {
            LoadLocalizationTextAssetForLanguage(language);

            Language.Value = language;
        }

        public string GetLocalizedTextByKey(string key) {
            if (!_localizationDictionary.ContainsKey(key)) {
                throw new InvalidEnumArgumentException(
                    $"No Localization by key {key} for {Language} language.");
            }

            return _localizationDictionary[key];
        }

        public void GetLocalizedSpriteByKey(string key, Action<Sprite> onSpriteLoaded) {
            if (!_localizationDictionary.ContainsKey(key)) {
                throw new InvalidEnumArgumentException(
                    $"No Localization by key {key} for {Language} language.");
            }

            var spriteName = _localizationDictionary[key];
            var spriteAddress = $"{_config.spriteAtlasAddress}[{spriteName}]";

            _spritesProvider.GetResourceByKey(spriteAddress, onSpriteLoaded);
        }

        private void LoadLocalizationTextAssetForLanguage(LocalizationLanguage language) {
            _textsProvider.GetResourceByKey(string.Format(_config.localizationPath, language),
                asset => {
                    var json = asset.text;

                    CreateLocalizationDictionaryFromJson(json);
                });
        }

        private void CreateLocalizationDictionaryFromJson(string json) {
            _localizationDictionary =
                new Dictionary<string, string>(JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
            JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }
    }
}
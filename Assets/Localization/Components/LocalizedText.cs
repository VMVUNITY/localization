using System;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Localization.Components {
    [RequireComponent(typeof(ZenAutoInjecter))]
    public class LocalizedText : TextMeshProUGUI {
        [Inject] private readonly ILocalizationProvider _localizationProvider;

        private IDisposable _onLanguageChanged;

        public string localizationKey;

        private object[] _parameters;

        //TODO: find better way
        public void SetParameters(params object[] parameters) {
            _parameters = parameters;

            SetLocalizedText();
        }

        private void SetLocalizedText() {
            var localizedText = _localizationProvider.GetLocalizedTextByKey(localizationKey);

            if (localizedText.Contains("{")) {
                if (_parameters == null) {
                    return;
                }

                localizedText = string.Format(localizedText, _parameters);
            }

            text = localizedText;
        }

        protected override void OnEnable() {
            base.OnEnable();

#if UNITY_EDITOR
            if (!Application.isPlaying) {
                return;
            }
#endif

            _onLanguageChanged = _localizationProvider.Language
                .Subscribe(_ => { SetLocalizedText(); });
        }

        protected override void OnDisable() {
            base.OnDisable();
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                return;
            }
#endif
            _onLanguageChanged.Dispose();
        }
    }
}
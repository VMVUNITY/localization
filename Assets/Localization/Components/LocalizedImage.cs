using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Localization.Components {
    public class LocalizedImage : Image {
        [Inject] private readonly ILocalizationProvider _localizationProvider;

        private IDisposable _onLanguageChanged;

        public string localizationKey;

        private void GetLocalizedSprite() {
            _localizationProvider.GetLocalizedSpriteByKey(localizationKey, OnGotLocalizedSprite);
        }

        private void OnGotLocalizedSprite(Sprite localizedSprite) {
            sprite = localizedSprite;
        }

        protected override void OnEnable() {
            base.OnEnable();

#if UNITY_EDITOR
            if (!Application.isPlaying) {
                return;
            }
#endif
            _onLanguageChanged = _localizationProvider.Language
                .Subscribe(_ => { GetLocalizedSprite(); });
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
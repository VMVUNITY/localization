using System;
using Localization.Enums;
using UniRx;
using UnityEngine;

namespace Localization {
    public interface ILocalizationProvider {
        ReactiveProperty<LocalizationLanguage> Language { get; }

        string GetLocalizedTextByKey(string key);

        void GetLocalizedSpriteByKey(string key, Action<Sprite> onSpriteLoaded);
    }
}
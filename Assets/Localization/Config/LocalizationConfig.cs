using Localization.Enums;
using UnityEngine;

namespace Localization.Config {
    [CreateAssetMenu(menuName = "CandyWings/Localization/Config", fileName = "LocalizationConfig", order = 0)]
    public class LocalizationConfig : ScriptableObject {
        public const string CONFIG_PATH = "LocalizationConfig";

        public string localizationPath = "Localization({0})";
        public string spriteLocalizationPrefix = "_s_l_";
        public string spriteAtlasAddress = "LocalizationAtlas";
        public string _localizedSpriteAddress = "{0}[{1}]";
        public LocalizationResourceLocation resourceLocation = LocalizationResourceLocation.Addressables;
    }
}
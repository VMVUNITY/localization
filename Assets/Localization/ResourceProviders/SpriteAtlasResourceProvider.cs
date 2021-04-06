using System;
using UnityEngine;
using UnityEngine.U2D;

namespace Localization.ResourceProviders {
    public class SpriteAtlasResourceProvider : IResourceProvider<string, Sprite> {
        private readonly SpriteAtlas _atlas;

        public SpriteAtlasResourceProvider(string atlasPath) {
            _atlas = Resources.Load<SpriteAtlas>(atlasPath);
        }

        public void GetResourceByKey(string key, Action<Sprite> callback) {
            key = key.Split('[', ']')[1];
            callback.Invoke(_atlas.GetSprite(key));
        }
    }
}
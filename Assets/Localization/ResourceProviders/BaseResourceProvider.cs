using System;
using System.Collections.Generic;

namespace Localization.ResourceProviders {
    public abstract class BaseResourceProvider<TKey, TValue> : IResourceProvider<TKey, TValue> {
        private readonly Dictionary<TKey, TValue> _loadedResourceByKey;

        protected BaseResourceProvider() {
            _loadedResourceByKey = new Dictionary<TKey, TValue>();
        }

        public void GetResourceByKey(TKey key, Action<TValue> callback) {
            if (_loadedResourceByKey.ContainsKey(key)) {
                callback?.Invoke(_loadedResourceByKey[key]);

                return;
            }

            LoadResourceByKey(key, value => {
                _loadedResourceByKey.Add(key, value);

                callback?.Invoke(value);
            });
        }

        protected abstract void LoadResourceByKey(TKey key, Action<TValue> handler);
    }
}
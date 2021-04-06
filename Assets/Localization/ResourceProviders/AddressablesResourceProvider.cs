using System;
using UnityEngine.AddressableAssets;

namespace Localization.ResourceProviders {
    public class AddressablesResourceProvider<TKey, TValue> : BaseResourceProvider<TKey, TValue> {
        protected override void LoadResourceByKey(TKey key, Action<TValue> handler) {
            Addressables.LoadAssetAsync<TValue>(key).Completed += handle => handler.Invoke(handle.Result);
        }
    }
}
using System;

namespace Localization.ResourceProviders {
    public interface IResourceProvider<in TKey, out TValue> {
        void GetResourceByKey(TKey key, Action<TValue> callback);
        
        
    }
}
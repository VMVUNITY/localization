using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Localization.ResourceProviders {
    public class DefaultResourceProvider<TValue> : BaseResourceProvider<string, TValue> where TValue : Object {
        protected override void LoadResourceByKey(string key, Action<TValue> handler) {
            var resource = Resources.Load<TValue>(key);

            handler.Invoke(resource);
        }
    }
}
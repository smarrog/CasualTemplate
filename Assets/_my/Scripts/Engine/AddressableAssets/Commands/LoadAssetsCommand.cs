#if SMR_ADDRESSABLES
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Smr.AddressableAssets {
    public class LoadAssetsCommand<T> : AbstractAddressableCommand<IList<T>> {
        private readonly IEnumerable<AddressableKey> _keys;
        private readonly Addressables.MergeMode _mergeMode;

        internal LoadAssetsCommand(IEnumerable<AddressableKey> keys, Addressables.MergeMode mergeMode) {
            _keys = keys;
            _mergeMode = mergeMode;
        }

        protected override AsyncOperationHandle<IList<T>> CreateOperationHandle() {
            return Addressables.LoadAssetsAsync<T>(_keys, LoadItemCallBack, _mergeMode);
        }

        private void LoadItemCallBack(T obj) {}
    }
}
#endif
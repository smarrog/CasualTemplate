#if SMR_ADDRESSABLES
using UnityEngine.AddressableAssets;

namespace Smr.AddressableAssets {
    public class AddressableKey : IKeyEvaluator {
        private readonly object _key;
        public AssetReference AssetRef { get; }

        public AddressableKey(string address) {
            _key = address;
        }

        public AddressableKey(AssetReference assetRef) {
            _key = assetRef;
            AssetRef = assetRef;
        }

        public static implicit operator AddressableKey(string address) {
            return string.IsNullOrWhiteSpace(address) ? null : new AddressableKey(address);
        }

        public static implicit operator AddressableKey(AssetReference assetRef) {
            return assetRef == null || string.IsNullOrEmpty(assetRef.AssetGUID) ? null : new AddressableKey(assetRef);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != GetType()) {
                return false;
            }
            return Equals((AddressableKey)obj);
        }

        private bool Equals(AddressableKey other) {
            var str = ToString();
            return str.Equals(other.ToString());
        }

        public override int GetHashCode() {
            var str = ToString();
            return str != null ? str.GetHashCode() : 0;
        }

        public override string ToString() {
            return RuntimeKey?.ToString();
        }

        bool IKeyEvaluator.RuntimeKeyIsValid() {
            if (_key is IKeyEvaluator evaluator) {
                return evaluator.RuntimeKeyIsValid();
            }
            return _key != null;
        }

        public object RuntimeKey {
            get {
                if (_key is IKeyEvaluator evaluator) {
                    return evaluator.RuntimeKey;
                }
                return _key;
            }
        }
    }
}
#endif
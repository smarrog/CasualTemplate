using System;
using System.Collections.Generic;
using System.Linq;
using Smr.Extensions;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace Smr.Animations {
    [Serializable]
    public class AnimationData {
        public PlayableAsset Asset { get; }
        public bool IsValid => Asset && _bindingsCache != null;

        private bool _isSimilarForAllTracks; // обозначает, что мы пытаемся подставить binding о все треки
        private readonly List<AnimationBindingCache> _bindingsCache;

        // binding должен либо быть реальным типом, который будет использоваться, либо GameObject
        public AnimationData(PlayableAsset asset, Object binding) {
            _isSimilarForAllTracks = true;
            Asset = asset;
            _bindingsCache = new List<AnimationBindingCache> { new(binding) };
        }

        public AnimationData(PlayableAsset asset, IEnumerable<Object> bindings) {
            _isSimilarForAllTracks = false;
            Asset = asset;
            _bindingsCache = bindings.Select(binding => new AnimationBindingCache(binding)).ToList();
        }

        public Object GetGenericBinding(PlayableBinding trackAsset, int index) {
            var cache = GetBindingCache(index);
            var genericBinding = cache?.TryToGetBindingByType(trackAsset.outputTargetType);
            return genericBinding;
        }

        private AnimationBindingCache GetBindingCache(int index) {
            return _isSimilarForAllTracks
                ? _bindingsCache[0]
                : _bindingsCache.GetAtOrDefault(index);
        }
    }

    internal class AnimationBindingCache {
        private readonly Dictionary<Type, Object> _objectByType = new();

        private readonly Object _binding;

        private GameObject GameObject => _binding as GameObject;

        public AnimationBindingCache(Object binding) {
            _binding = binding;
            if (GameObject) {
                _objectByType[typeof(GameObject)] = _binding;
            }
        }

        internal Object TryToGetBindingByType(Type bindingType) {
            if (!_binding || bindingType == null) {
                return null;
            }

            if (!GameObject) {
                return _binding.GetType().IsSubclassOf(bindingType) ? _binding : null;
            }

            if (!_objectByType.ContainsKey(bindingType)) {
                var component = GameObject.GetComponent(bindingType);
                _objectByType[bindingType] = component;
            }
            return _objectByType[bindingType];
        }
    }
}
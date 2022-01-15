using System;
using Smr.Common;
using UnityEngine;

namespace Smr.Animations {
    [Serializable]
    public class TranslationPointData {
        private enum PointType {
            Vector3 = 0,
            Transform = 1
        }

        [SerializeField]
        private PointType _pointType;
        [SerializeField]
        private Vector3 _vector3;
        [SerializeField]
        private ExposedReference<Transform> _exposedTransform;

        public IExposedPropertyTable RuntimeResolver { get; set; }

        private Transform ResolvedTransform {
            get {
                if (RuntimeResolver != null) {
                    return _exposedTransform.Resolve(RuntimeResolver);
                }
                EngineDependencies.Logger.GetChannel(LogChannel.Animation).LogError("Runtime Resolver was not set");
                return null;
            }
        }

        public Vector3 Position {
            get => GetPosition();
            set => SetPosition(value);
        }

        public TranslationPointData(Vector3 position) {
            _pointType = PointType.Vector3;
            _vector3 = position;
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        public TranslationPointData(Transform transform) {
            _pointType = PointType.Transform;
            _exposedTransform.defaultValue = transform;
        }

        private Vector3 GetPosition() {
            return _pointType switch {
                PointType.Vector3 => _vector3,
                PointType.Transform => ResolvedTransform ? ResolvedTransform.position : Vector3.zero,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void SetPosition(Vector3 value) {
            switch (_pointType) {
                case PointType.Vector3:
                    _vector3 = value;
                    break;
                case PointType.Transform:
                    var transform = ResolvedTransform;
                    if (transform) {
                        ResolvedTransform.position = value;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
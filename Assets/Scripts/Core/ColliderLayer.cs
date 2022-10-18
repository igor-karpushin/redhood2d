using System;
using System.Collections.Generic;
using UnityEngine;

namespace Adventure.Core
{
    public enum ColliderLayerMask
    {
        ColliderSolid = 1 << 0,
        ColliderPlatform = 1 << 1,
        ColliderIgnorePlatform = 1 << 2
    }

    public class ColliderLayer
    {
        private int _layer;
        private int _innerMask;
        private readonly Dictionary<ColliderLayerMask, int> _layerIndexes;

        public ColliderLayer()
        {
            _layer = 0;
            _innerMask = 0;
            _layerIndexes = new Dictionary<ColliderLayerMask, int>();

            // fill layers indexes
            var enumType = typeof(ColliderLayerMask);
            var enumNames = Enum.GetNames(enumType);
            var enumValues = Enum.GetValues(enumType);

            for (var i = 0; i < enumValues.Length; ++i)
            {
                var layerIndex = LayerMask.NameToLayer(enumNames[i]);
                _layerIndexes.Add((ColliderLayerMask)enumValues.GetValue(i), layerIndex);
            }
        }

        public LayerMask AddCollider(ColliderLayerMask collider)
        {
            if (HasCollider(collider)) return _layer;
            
            _innerMask |= (int) collider;
            if (_layerIndexes.ContainsKey(collider))
            {
                _layer |= 1 << _layerIndexes[collider];
            }

            return _layer;
        }

        public LayerMask RemoveCollider(ColliderLayerMask collider)
        {
            if (!HasCollider(collider)) return _layer;
            _innerMask &= ~(int)collider;
            if (_layerIndexes.ContainsKey(collider))
            {
                _layer &= ~(1 << _layerIndexes[collider]);
            }
            return _layer;
        }

        public bool HasCollider(ColliderLayerMask collider)
        {
            return (_innerMask & (int)collider) != 0;
        }
    }
}
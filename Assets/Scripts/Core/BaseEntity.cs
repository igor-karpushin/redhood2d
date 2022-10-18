using System;
using System.Collections.Generic;
using Adventure.Components;
using UnityEngine;

namespace Adventure.Core
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class BaseEntity : MonoBehaviour
    {
        [SerializeField] private List<BaseComponent> components;

        private Dictionary<Type, BaseComponent> _hashTypeComponents;
        private Rigidbody2D _rigidbody;
        private int _state;
        private BoxCollider2D _boxCollider;

        public float ColliderLegsPosition => _rigidbody.position.y + _boxCollider.offset.y - _boxCollider.size.y * 0.5f;

        public event Action<EntityState> StateAdded;
        public event Action<EntityState> StateRemoved;

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _hashTypeComponents = new Dictionary<Type, BaseComponent>();
            foreach (var component in components)
            {
                component.Setup(this);
                _hashTypeComponents.Add(component.GetType(), component);
            }
        }
        
        protected virtual void OnEnable()
        {
            _rigidbody.isKinematic = true;
        }

        private void OnDisable()
        {
            _rigidbody.isKinematic = false;
        }

        protected virtual void Update()
        {
            foreach (var component in components)
            {
                component.InnerUpdate();
            }
        }

        private void FixedUpdate()
        {
            foreach (var component in components)
            {
                component.FixedInnerUpdate(_rigidbody);
            }
        }
        
        public bool TurnMovement(float value)
        {
            var localScale = transform.localScale;
            var flipHorizontal = value > 0 ? 1 : -1;

            var result = localScale.x * flipHorizontal < 0;
            transform.localScale = new Vector3(flipHorizontal, 1, 1);
            return result;
        }

        public void AddState(EntityState state)
        {
            if (HasState(state)) return;
            
            _state |= (int)state;
            StateAdded?.Invoke(state);
        }

        public void RemoveState(EntityState state)
        {
            if (!HasState(state)) return;
            
            _state &= ~(int)state;
            StateRemoved?.Invoke(state);
        }

        public bool HasState(EntityState state) =>
            (_state & (int) state) != 0;

        public bool Get<T>(out T component) where T: BaseComponent
        {
            component = default;
            if (!_hashTypeComponents.TryGetValue(typeof(T), out var outComponent)) return false;
            component = (T) outComponent;
            return true;
        }

        private void OnDrawGizmos()
        {
            if (_rigidbody)
            {
                Gizmos.color = Color.blue;
                var position = _rigidbody.position;
                position.y = ColliderLegsPosition;
                Gizmos.DrawSphere(position, 0.04f);
            }
        }

        public float GetDeltaHorizontal(float pointX)
        {
            var deltaPosition = Math.Abs(_rigidbody.position.x - pointX);
            return deltaPosition - _boxCollider.size.x * 0.5f;
        }
    } 
}


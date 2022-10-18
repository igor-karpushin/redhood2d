using System;
using Adventure.Core;
using UnityEngine;

namespace Adventure.Components
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class SpriteComponent : BaseComponent
    {
        private Animator _animator;
        private Type _stateType;

        public event Action<string, int> RuntimeEvent;

        protected override void Init()
        {
            _animator = GetComponent<Animator>();
            _stateType = typeof(EntityState);
            Entity.StateAdded += OnStateAdded;
            Entity.StateRemoved += OnStateRemoved;
        }

        private void RuntimeInnerEvent(AnimationEvent runtimeEvent) =>
            RuntimeEvent?.Invoke(runtimeEvent.stringParameter, runtimeEvent.intParameter);

        private void OnStateAdded(EntityState state) =>
            _animator.SetBool(Enum.GetName(_stateType, state), true);
        
        private void OnStateRemoved(EntityState state) =>
            _animator.SetBool(Enum.GetName(_stateType, state), false);
    } 
}


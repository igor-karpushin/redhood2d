using System;
using System.Collections;
using Adventure.Core;
using UnityEngine;

namespace Adventure.Components
{
    public class MovementComponent : BaseComponent
    {
        [SerializeField] private float maxSpeed;
        private float _deltaMovement = 1f;

        public void ReleaseVertical(bool value = false)
        {
            if (Entity.Get<KinematicComponent>(out var kinematic))
            {
                kinematic.SetVerticalMovement(value);
            }
        }
        public void ReleaseHorizontal(float value = 0f)
        {
            if (!Entity.Get<KinematicComponent>(out var kinematic)) return;
            
            var horizontalMovement = value * maxSpeed;
            kinematic.SetHorizontalMovement(horizontalMovement, _deltaMovement);
                
            if (Math.Abs(horizontalMovement) > 0.1f)
            {
                Entity.AddState(EntityState.MovementPositive);
                Entity.RemoveState(EntityState.AttackLight);

                if (!Entity.TurnMovement(horizontalMovement)) return;
                    
                Entity.AddState(EntityState.MovementTurn);
                    
                // slow speed when turn
                _deltaMovement = 0.4f;
                StartCoroutine(MovementTurnStop());
            }
            else
            {
                Entity.RemoveState(EntityState.MovementPositive);
            }
        }

        private IEnumerator MovementTurnStop()
        {
            var frames = 15;
            while (frames > 0)
            {
                yield return null;
                frames--;
            }

            Entity.RemoveState(EntityState.MovementTurn);

            frames = 40;
            while (frames > 0)
            {
                yield return null;
                frames--;
            }

            _deltaMovement = 1f;
        }
    }
}
using System;
using Adventure.Components;
using UnityEngine;

namespace Adventure.Core
{
    public class PlayerEntity : BaseEntity
    {
        protected override void Update()
        {
            // jump functionality
            if (!HasState(EntityState.AttackLight))
            {
                var jumpButtonDown = Input.GetButtonDown("Jump");
                var jumpButtonUp = Input.GetButtonUp("Jump");

                if (jumpButtonDown || jumpButtonUp)
                {
                    if (Get<JumpComponent>(out var jump))
                    {
                        jump.Release(jumpButtonDown, jumpButtonUp);
                    }
                }
            }

            // movement
            if (!HasState(EntityState.AttackLight))
            {
                if (Get<MovementComponent>(out var movement))
                {
                    if (Input.GetButtonDown("Vertical"))
                    {
                        movement.ReleaseVertical(true);
                    }
                    
                    if (Input.GetButtonUp("Vertical"))
                    {
                        movement.ReleaseVertical(false);
                    }
                    
                    movement.ReleaseHorizontal(Input.GetAxis("Horizontal"));
                }
            }

            // attack
            if (HasState(EntityState.KinematicGrounded))
            {
                if (Get<AttackComponent>(out var attack))
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        attack.Release(true);
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        attack.Release(false);
                    }
                }
            }

            base.Update();
        }
    }
}
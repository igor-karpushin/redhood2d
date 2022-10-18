using System;
using System.Collections;
using Adventure.Core;
using UnityEngine;

namespace Adventure.Components
{
    public class KinematicComponent : BaseComponent
    {
        [SerializeField] private float minGroundNormalY = .65f;

        [SerializeField] private float gravityModifier = 1f;

        private const float MinMoveDistance = 0.001f;
        private const float ShellRadius = 0.01f;

        [NonSerialized] public Vector2 Velocity;
        private float _horizontalMovement;
        private float _horizontalDelta = 1f;
        private float _verticalDelta = 1f;

        private Vector2 _groundNormal;

        private ContactFilter2D _contactFilter;
        private RaycastHit2D[] _hitBuffer;

        private bool _hasGroundedState = false;
        private float _deltaGrounded = 1f;
        private ColliderLayer _colliderMask;

        protected override void Init()
        {
            _groundNormal = Vector2.zero;
            _hitBuffer = new RaycastHit2D[16];
            
            _contactFilter.useTriggers = false;
            _contactFilter.useLayerMask = true;
            
            _colliderMask = new ColliderLayer();
            _colliderMask.AddCollider(ColliderLayerMask.ColliderSolid);
        }

        public override void InnerUpdate()
        {
            Entity.RemoveState(EntityState.VelocityNegative);
            Entity.RemoveState(EntityState.VelocityPositive);

            Entity.AddState(Velocity.y > 0 ? EntityState.VelocityPositive : EntityState.VelocityNegative);
        }

        public override void FixedInnerUpdate(Rigidbody2D body)
        {
            if (Velocity.y < 0)
            {
                Velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
            }
            else
            {
                Velocity += Physics2D.gravity * Time.deltaTime;
            }

            Velocity.x = _horizontalMovement;

            _hasGroundedState = Entity.HasState(EntityState.KinematicGrounded);
            Entity.RemoveState(EntityState.KinematicGrounded);

            var deltaVelocity = Velocity * Time.deltaTime;
            var moveAlongGround = new Vector2(_groundNormal.y, -_groundNormal.x);

            PerformMovementVertical(body, Vector2.up * deltaVelocity.y);
            PerformMovementHorizontal(body, moveAlongGround * deltaVelocity.x);
        }

        public void SetHorizontalMovement(float movement, float delta)
        {
            _horizontalMovement = movement;
            _horizontalDelta = delta;
        }
        
        public void SetVerticalMovement(bool value)
        {
            if (value)
            {
                _colliderMask.RemoveCollider(ColliderLayerMask.ColliderPlatform);
                _contactFilter.SetLayerMask(_colliderMask.AddCollider(ColliderLayerMask.ColliderIgnorePlatform));
            }
            else
            {
                _contactFilter.SetLayerMask(_colliderMask.RemoveCollider(ColliderLayerMask.ColliderIgnorePlatform)); 
            }
        }

        private void PerformMovementHorizontal(Rigidbody2D body, Vector2 move)
        {
            var distance = move.magnitude;
            if (!(distance > MinMoveDistance)) return;
            
            _contactFilter.SetLayerMask(_colliderMask.RemoveCollider(ColliderLayerMask.ColliderPlatform));
            var count = body.Cast(move, _contactFilter, _hitBuffer, distance + ShellRadius);
            if (count > 0)
            {
                distance = Entity.GetDeltaHorizontal(_hitBuffer[0].point.x);
            }
            body.position += move.normalized * distance * _horizontalDelta * _deltaGrounded;
        }

        private void PerformMovementVertical(Rigidbody2D body, Vector2 move)
        {
            var distance = move.magnitude;
            if (distance > MinMoveDistance)
            {
                if (Entity.HasState(EntityState.VelocityNegative))
                {
                    if (!_colliderMask.HasCollider(ColliderLayerMask.ColliderIgnorePlatform))
                    {
                        _contactFilter.SetLayerMask(_colliderMask.AddCollider(ColliderLayerMask.ColliderPlatform));  
                    }
                } else if (Entity.HasState(EntityState.VelocityPositive))
                {
                    _contactFilter.SetLayerMask(_colliderMask.RemoveCollider(ColliderLayerMask.ColliderPlatform));  
                }
                
                var count = body.Cast(move, _contactFilter, _hitBuffer, distance + ShellRadius);
                for (var i = 0; i < count; i++)
                {
                    if (Entity.HasState(EntityState.VelocityNegative))
                    {
                        if (_hitBuffer[i].point.y - Entity.ColliderLegsPosition > 0.01f)
                        {
                            // we are inside collider -> ignore
                            continue; 
                        }
                    }
                    
                    var currentNormal = _hitBuffer[i].normal;
                    if (currentNormal.y > minGroundNormalY)
                    {
                        Entity.AddState(EntityState.KinematicGrounded);
                        _groundNormal = currentNormal;

                        if (!_hasGroundedState)
                        {
                            // just grounded -> slow movement
                            _deltaGrounded = 0.5f;
                            StartCoroutine(KinematicLandingStop());
                        }
                    }
                    
                    // not inside other collider
                    if (Math.Abs(currentNormal.y) > 0.01f)
                    {
                        if (Entity.HasState(EntityState.KinematicGrounded))
                        {
                            var projection = Vector2.Dot(Velocity, currentNormal);
                            if (projection < 0)
                            {
                                Velocity -= projection * currentNormal;
                            }
                            
                            var modifiedDistance = _hitBuffer[i].distance - ShellRadius;
                            distance = modifiedDistance < distance ? modifiedDistance : distance;
                        }
                        else
                        {
                            Velocity.y = Mathf.Min(Velocity.y, 0);
                        }
                    }
                }
            }

            body.position += move.normalized * distance * _verticalDelta;
        }

        private IEnumerator KinematicLandingStop()
        {
            var frames = 80;
            while (frames > 0)
            {
                yield return null;
                frames--;
            }

            _deltaGrounded = 1f;
        }
    }
}
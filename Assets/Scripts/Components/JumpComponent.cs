using Adventure.Core;

namespace Adventure.Components
{
    public class JumpComponent : BaseComponent
    {
        private enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
        
        public float jumpTakeOffSpeed = 7;
        public float jumpModifier = 0.8f;
        public float jumpDeceleration = 0f;
        
        private JumpState _jumpState;
        private bool _isJump;
        private bool _stopJump;
        
        protected override void Init()
        {
            _jumpState = JumpState.Grounded;
            _isJump = false;
            _stopJump = false;
        }

        public override void InnerUpdate()
        {
            InnerUpdateState();
            InnerUpdateVelocity();
        }

        private void InnerUpdateVelocity()
        {
            if (!Entity.Get<KinematicComponent>(out var kinematic)) return;
            
            if (_isJump && Entity.HasState(EntityState.KinematicGrounded))
            {
                kinematic.Velocity.y = jumpTakeOffSpeed * jumpModifier;
                _isJump = false;
            }
            else if (_stopJump)
            {
                _stopJump = false;
                if (kinematic.Velocity.y > 0)
                {
                    kinematic.Velocity.y *= jumpDeceleration;
                }
            }
        }

        private void InnerUpdateState()
        {
            switch (_jumpState)
            {
                case JumpState.PrepareToJump:
                    _jumpState = JumpState.Jumping;
                    _isJump = true;
                    _stopJump = false;
                    break;
                
                case JumpState.Jumping:
                    if (!Entity.HasState(EntityState.KinematicGrounded))
                    {
                        _jumpState = JumpState.InFlight;
                    }
                    break;
                
                case JumpState.InFlight:
                    if (Entity.HasState(EntityState.KinematicGrounded))
                    {
                        _jumpState = JumpState.Landed;
                    }
                    break;
                
                case JumpState.Landed:
                    _jumpState = JumpState.Grounded;
                    break;
            }
        }

        public void Release(bool jumpStart, bool jumpEnd)
        {
            if (_jumpState == JumpState.Grounded && jumpStart)
            {
                _jumpState = JumpState.PrepareToJump;
            } else if (jumpEnd)
            {
                _stopJump = true;
            }
        }
    } 
}


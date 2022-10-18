using Adventure.Core;

namespace Adventure.Components
{
    public class AttackComponent : BaseComponent
    {
        protected override void Init()
        {
            if (Entity.Get<SpriteComponent>(out var sprite))
            {
                sprite.RuntimeEvent += RuntimeEventHandle;
            }
        }

        public void Release(bool state)
        {
            if (state)
            {
                Entity.AddState(EntityState.AttackLight);
                if (Entity.Get<MovementComponent>(out var movement))
                {
                    movement.ReleaseHorizontal(0f);
                }
            }
            else
            {
                Entity.RemoveState(EntityState.AttackLight);
            }
        }
        
        private void RuntimeEventHandle(string animName, int index)
        {
            //UnityEngine.Debug.Log($"anim:{animName} index:{index}");
        }
    } 
}


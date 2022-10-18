using Adventure.Core;
using UnityEngine;

namespace Adventure.Components
{
    public abstract class BaseComponent : MonoBehaviour
    {
        protected BaseEntity Entity;

        public void Setup(BaseEntity parent)
        {
            Entity = parent;
            Init();
        }

        protected virtual void Init()
        {
        }

        public virtual void InnerUpdate()
        {
        }

        public virtual void FixedInnerUpdate(Rigidbody2D body)
        {
            
        }
    } 
}


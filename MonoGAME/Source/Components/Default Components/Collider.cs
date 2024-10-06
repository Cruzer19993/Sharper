using Microsoft.Xna.Framework;
using MonoGAME.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGAME.Components
{
    public abstract class Collider : Component
    {
        public Collider()
        {
            componentSignature.Set(4, true);
        }
        protected Vector2 size;
        
        public Vector2 Size
        {
            get { return size; }
            private set { size = value; }
        }

        //parameter other is the object we collided with
        public virtual void OnCollision(Collider other)
        {

        }
    }
}

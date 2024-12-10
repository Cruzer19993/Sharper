using Microsoft.Xna.Framework;
using Sharper.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharper.Components
{
    public abstract class Collider : Component
    {
        public Collider()
        {
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

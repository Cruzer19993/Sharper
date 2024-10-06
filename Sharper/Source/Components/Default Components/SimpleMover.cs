using Sharper.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharper.ECS
{
    public class SimpleMover : Component
    {
        public SimpleMover()
        {
            movementSpeed = 5;
            componentSignature.Set(6, true);
        }
        public float movementSpeed;
    }
}

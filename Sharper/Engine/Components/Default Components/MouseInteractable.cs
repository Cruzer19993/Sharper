using Microsoft.Xna.Framework;
using Sharper.Systems.Backend;
using Sharper.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharper.Components
{
    public class MouseInteractable : Component
    {
        public MouseInteractable () 
        {
            isMouseOverEntity = false;
            useOwnHitboxSize = false;
            renderHitboxOutline = false;
            hitboxSize = Vector2.Zero;
        }
        public MouseInteractable(bool useOwnHitbox = false, bool renderHitboxOutline = false)
        {
            isMouseOverEntity = false;
            useOwnHitboxSize = useOwnHitbox;
            this.renderHitboxOutline = renderHitboxOutline;
            hitboxSize = Vector2.Zero;
        }

        public override void CopyComponentData(Component reference)
        {
            if(reference is MouseInteractable mi)
            {
                isMouseOverEntity = mi.isMouseOverEntity;
                useOwnHitboxSize = mi.useOwnHitboxSize;
                hitboxSize = mi.hitboxSize;
            }
        }
        public bool isMouseOverEntity;
        public bool useOwnHitboxSize;
        public bool renderHitboxOutline;
        public Vector2 hitboxSize;
    }
}

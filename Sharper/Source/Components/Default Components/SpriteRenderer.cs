using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Sharper.ECS
{
    public class SpriteRenderer : Component
    {

        public SpriteRenderer(SpriteRenderer other)
        {
            sprite.atlasY = other.sprite.atlasY;
            sprite.atlasX = other.sprite.atlasX;
            sprite.zIndex = other.sprite.zIndex;
            spriteColor = other.spriteColor;
            componentSignature.Set(2, true);
        }

        public SpriteRenderer()
        {
            sprite = new Sprite(0, 0);
            spriteColor = Color.White;
            componentSignature.Set(2, true);
        }

        public Sprite sprite;
        public Color spriteColor;

        public override void CopyComponentData(Component reference)
        {
            if (reference is SpriteRenderer rend)
            {
                sprite.atlasX = rend.sprite.atlasX;
                sprite.atlasY = rend.sprite.atlasY;
                sprite.zIndex = rend.sprite.zIndex;
                spriteColor = rend.spriteColor;
            }
        }
    }

    public class Sprite
    {
        public Sprite(int x = 0, int y = 0, int z_index = 0)
        {
            atlasX = x;
            atlasY = y;
            zIndex = z_index;
        }

        public int atlasX, atlasY, zIndex;
    }
}

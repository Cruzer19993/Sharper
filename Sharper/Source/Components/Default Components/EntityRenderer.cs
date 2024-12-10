using Microsoft.Xna.Framework;
using Sharper.Components.GUI;
using Sharper.Systems.Backend;
using System.Collections.Generic;

namespace Sharper.ECS
{
    public class EntityRenderer : Component
    {
        public EntityRenderer() 
        {
            m_sprite = new Sprite();
            m_allowRendering = true;
            rectHeight = RenderingSystem.Instance.SpriteAtlasSettings.pixelsPerSprite;
            rectWidth = RenderingSystem.Instance.SpriteAtlasSettings.pixelsPerSprite;
        }
        public EntityRenderer(EntityRenderer other)
        {
            m_sprite.m_atlasY = other.m_sprite.m_atlasY;
            m_sprite.m_atlasX = other.m_sprite.m_atlasX;
        }

        public EntityRenderer(int rectWidth, int rectHeight, string textureName = "", bool allowRendering = true)
        {
            m_sprite = new Sprite(0, 0);
            allowRendering = true;
            this.rectHeight = rectHeight;
            this.rectWidth = rectWidth;
        }
        public int rectHeight, rectWidth;
        public Sprite m_sprite;
        public bool m_allowRendering;
        public Rectangle renderRectangle;
        public override void CopyComponentData(Component reference)
        {
            if (reference is EntityRenderer rend)
            {
                m_sprite.m_atlasX = rend.m_sprite.m_atlasX;
                m_sprite.m_atlasY = rend.m_sprite.m_atlasY;
            }
        }
    }

    public class Sprite
    {
        public Sprite(int x = 0, int y = 0)
        {
            m_atlasX = x;
            m_atlasY = y;
            this.m_color = Color.White;
            m_textureName = "";
        }

        public Sprite(int x, int y, Color color)
        {
            m_atlasX = x;
            m_atlasY = y;
            this.m_color = color;
            m_textureName = "";
        }
        public Sprite(string textureName,Color color)
        {
            m_atlasX = m_atlasY = 0;
            m_color = color;
            m_textureName = textureName;
        }

        public int m_atlasX, m_atlasY;
        public Color m_color;
        public string m_textureName;
    }
}

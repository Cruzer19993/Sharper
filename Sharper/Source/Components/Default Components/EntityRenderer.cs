using Microsoft.Xna.Framework;
using Sharper.Components.GUI;
using System.Collections.Generic;

namespace Sharper.ECS
{
    public class EntityRenderer : Component
    {
        public EntityRenderer() 
        {
            m_sprite = new Sprite();
            textToRender = null;
            m_renderTarget = RenderTarget.GridSpace;
            m_allowRendering = true;
        }
        public EntityRenderer(EntityRenderer other)
        {
            m_sprite.m_atlasY = other.m_sprite.m_atlasY;
            m_sprite.m_atlasX = other.m_sprite.m_atlasX;
        }

        public EntityRenderer(string textureName = "",RenderTarget target = RenderTarget.GridSpace, bool allowRendering = true)
        {
            m_sprite = new Sprite(0, 0);
            m_renderTarget = target;
            allowRendering = true;
        }
        public GUIText textToRender;
        public Sprite m_sprite;
        public RenderTarget m_renderTarget;
        public bool m_allowRendering;
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
        }

        public Sprite(int x, int y, Color color)
        {
            m_atlasX = x;
            m_atlasY = y;
            this.m_color = color;
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
    public enum RenderTarget
    {
        GridSpace, //tiles of the world etc
        WorldSpace, //dynamic objects like players, projectiles etc
        GUI, //GUI elements
        Text //text
    }
}

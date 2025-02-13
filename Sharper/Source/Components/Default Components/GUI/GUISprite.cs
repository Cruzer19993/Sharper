using Sharper.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sharper.Components.GUI
{
    public class GUISprite : Component
    {
        public GUISprite(string textureName, Color color)
        {
            m_sprite.m_textureName = textureName;
            m_sprite.m_color = color;
        }
        public GUISprite(int atlasX, int atlasY, Color color)
        {
            m_sprite.m_atlasX = atlasX;
            m_sprite.m_atlasY = atlasY;
            m_sprite.m_color = color;
        }
        public GUISprite(Sprite sprite)
        {
            m_sprite = sprite;
        }

        public GUISprite()
        {
            m_sprite = new Sprite();
            m_sprite.m_textureName = "GUIRectangleHR";
            m_sprite.m_color = Color.Black;

        }

        public Sprite m_sprite;
    }
}

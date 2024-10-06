using MonoGAME.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MonoGAME.Components.GUI
{
    public class GUISprite : Component
    {
        public GUISprite(int atlasX, int atlasY)
        {
            m_textureName = "atlas";
            m_atlasX = atlasX;
            m_atlasY = atlasY;
        }
        public GUISprite(string textureName)
        {
            m_textureName = textureName;
        }

        public GUISprite()
        {
            m_textureName = "GUIDefaultTexture";
            m_atlasX = -1;
            m_atlasY = -1;
        }

        public string m_textureName;
        public int m_atlasX;
        public int m_atlasY;
    }
}

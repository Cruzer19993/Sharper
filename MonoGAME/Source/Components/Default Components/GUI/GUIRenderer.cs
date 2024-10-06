using Microsoft.Xna.Framework;
using MonoGAME.ECS;

namespace MonoGAME.Components.GUI
{
    public class GUIRenderer : Component
    {
        public GUIRenderer(string textureName,Color color,bool renderable = true)
        {
            m_renderable = renderable;
            m_textureName = textureName;
            m_color = color;
            componentSignature.Set(8, true);
        }
        public GUIRenderer()
        {
            m_textureName = "GUIDefaultTexture";
            m_renderable = true;
            m_color = Color.White;
            componentSignature.Set(8, true);
        }
        public string m_textureName;
        public bool m_renderable;
        public Color m_color;
    }
}

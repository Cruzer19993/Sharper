using Microsoft.Xna.Framework;
using Sharper.ECS;

namespace Sharper.Components.GUI
{
    public class GUIText : Component
    {
        public GUIText() 
        {
            m_text = "";
            m_fontSize = 12;
            m_color = Color.White;
            componentSignature.Set(11, true);
        }
        public string m_text;
        public int m_fontSize;
        public Color m_color;
    }
}

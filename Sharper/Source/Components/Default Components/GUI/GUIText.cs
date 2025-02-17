using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharper.ECS;
using Sharper.Systems.Backend.Standalone;
namespace Sharper.Components.GUI
{
    public class GUIText : Component
    {
        private string m_text;
        public string Text {
            get { return m_text; }
            set { 
                m_text = value;
                if (m_dynamicRectSize)
                {
                    GUILayoutManager.ResizeText(this.owner.GetComponent<GUIRect>(), m_text);
                }
            }

        }
        public Color m_color;
        public float m_scale;
        public bool m_dynamicRectSize;
        public GUIText()
        {
            m_text = "";
            m_color = Color.Black;
            m_scale = 1f;
            m_dynamicRectSize = true;
        }
    }
}

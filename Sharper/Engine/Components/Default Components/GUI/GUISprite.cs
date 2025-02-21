using Sharper.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharper.ECS;
namespace Sharper.Components.GUI
{
    public class GUISprite : Component
    {
        public Sprite m_uiSprite;

        public GUISprite()
        {
            m_uiSprite = new Sprite();
        }
    }
}

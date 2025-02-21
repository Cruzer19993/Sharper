using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Sharper.ECS;
namespace Sharper.Components.GUI
{
    public class GUIInputBox : Component
    {
        public GUIText m_inputBoxTextDisplay;
        public Color m_activeColor;
        public Color m_inactiveColor;
        public bool m_isActive;

        public GUIInputBox()
        {
            m_inputBoxTextDisplay = null;
            m_activeColor = Color.Green;
            m_inactiveColor = Color.White;
            m_isActive = false;
        }
        public void SetActive(bool state)
        {
            m_isActive = state;
        }
        public void SwitchActive()
        {
            m_isActive = !m_isActive;
        }
        public string GetValue()
        {
            return m_inputBoxTextDisplay.Text;
        }

        public void SetValue(string value)
        {
            m_inputBoxTextDisplay.Text = value;
        }
    }
}

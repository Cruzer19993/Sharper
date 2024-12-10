using Sharper.ECS;

namespace Sharper.Components.GUI
{
    public class GUIInputBox : Component
    {
        public GUIInputBox()
        {
            m_active = false;
        }
        public bool m_active;
    }
}

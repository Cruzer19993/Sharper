using MonoGAME.ECS;

namespace MonoGAME.Components.GUI
{
    public class GUIInputBox : Component
    {
        public GUIInputBox()
        {
            m_text = "";
            m_active = false;
            componentSignature.Set(10, true);
        }
        public string m_text;
        public bool m_active;
    }
}

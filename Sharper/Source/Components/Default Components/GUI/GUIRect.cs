using Microsoft.Xna.Framework;
using Sharper.ECS;

namespace Sharper.Components.GUI
{
    public class GUIRect : Component
    {
        public Vector2 m_size;
        public Vector2 m_position;
        public Quaternion m_rotation;
        public GUIRect()
        {
            m_size = Vector2.One*16f;
            m_position = Vector2.Zero;
            m_rotation = Quaternion.Identity;
        }

        public Rectangle GetRect()
        {
            return new Rectangle((int)m_position.X, (int)m_position.Y, (int)m_size.X,(int)m_size.Y);
        }
    }
}

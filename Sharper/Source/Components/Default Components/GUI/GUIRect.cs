using Microsoft.Xna.Framework;
using Sharper.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharper.Components.GUI
{
    public class GUIRect : Component
    {
        public GUIRect(Vector2 position, Vector2 size, Vector2 scale)
        {
            m_Position = position;
            m_size = size;
            m_scale = scale;
            componentSignature.Set(7, true);
        }
        public GUIRect(Vector2 position, Vector2 size)
        {
            m_size = size;
            m_Position = position;
            m_scale = Vector2.One;
            componentSignature.Set(7, true);
        }
        public GUIRect()
        {
            m_size = Vector2.Zero;
            m_Position = Vector2.Zero;
            m_scale = Vector2.One;
            componentSignature.Set(7, true);
        }
        protected Vector2 m_scale;
        protected Vector2 m_size;
        protected Vector2 m_Position;
        public bool m_isManaged;
        public Rectangle GetRect()
        {
            return new Rectangle((int)m_Position.X,(int)m_Position.Y,(int)m_size.X,(int)m_size.Y);
        }
        public Vector2 GetScale()
        {
            return m_scale;
        }
        public Vector2 GetPosition()
        {
            return m_Position;
        }
        public Vector2 GetSize()
        {
            return m_size;
        }
        public void SetPosition(Vector2 newPosition)
        {
            m_Position = newPosition;
        }
        public void SetScale(Vector2 newScale)
        {
            m_scale = newScale;
        }
        public void SetSize(Vector2 newSize)
        {
            m_size = newSize;
        }
    }
}

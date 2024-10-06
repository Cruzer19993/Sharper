using Sharper.ECS;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace Sharper.Components.GUI
{
    public class GUILayout : Component
    {
        //Parameterless constructor is needed for every component
        public GUILayout(Vector2 spacing, Vector2 padding, Vector2 offset)
        {
            m_spacing = spacing;
            m_padding = padding;
            m_offset = offset;
            m_managedObjects = new List<GUIRect>();
        }
        public GUILayout(Vector2 spacing, Vector2 padding)
        {
            m_spacing = spacing;
            m_padding = padding;
            m_managedObjects = new List<GUIRect>();
        }
        public GUILayout()
        {
            m_managedObjects = new List<GUIRect>();
            m_spacing = Vector2.Zero;
            m_padding = Vector2.Zero;
            m_offset = Vector2.Zero;
        }
        public List<GUIRect> m_managedObjects;
        public Vector2 m_spacing;
        public Vector2 m_padding;
        public Vector2 m_offset;
        public event EventHandler<EventArgs> OnUpdate;

        public void InvokeOnUpdate()
        {
            OnUpdate?.Invoke(this, new EventArgs());
        }
    }
}

using Sharper.ECS;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace Sharper.Components.GUI
{
    public class GUILayout : Component
    {
        //Parameterless constructor is needed for every component
        public GUILayout(Vector2 spacing, Vector2 padding, Vector2 offset, params GUILayoutOptions[] options)
        {
            m_spacing = spacing;
            m_padding = padding;
            m_offset = offset;
            m_managedObjects = new List<GUIRect>();
            m_layoutOptions = options;
        }
        public GUILayout()
        {
            m_managedObjects = new List<GUIRect>();
            m_layoutOptions = new GUILayoutOptions[0];
            m_spacing = Vector2.Zero;
            m_padding = Vector2.Zero;
            m_offset = Vector2.Zero;
        }
        public List<GUIRect> m_managedObjects;
        public GUILayoutOptions[] m_layoutOptions;
        public Vector2 m_spacing;
        public Vector2 m_padding;
        public Vector2 m_offset;
        public event EventHandler<EventArgs> OnUpdate;

        public void InvokeOnUpdate()
        {
            OnUpdate?.Invoke(this, new EventArgs());
        }
    }

    public enum GUILayoutOptions
    {
        STRETCH_WIDTH,
        STRETCH_HEIGHT,
        CONTENT_CENTER_HORIZONTAL,
        CONTENT_CENTER_VERTICAL,
        SNAP_LEFT,
        SNAP_RIGHT,
        SNAP_BOTTOM,
        SNAP_TOP
    }
}

using Microsoft.Xna.Framework;
using Sharper;
using Sharper.ECS;
using System.Collections.Generic;

namespace Sharper.Components.GUI
{
    public class GUILayout : Component
    {
        public GUILayout m_root;
        public bool m_isBranch;
        public Vector2 m_spacing;
        public Vector2 m_padding;
        public Vector2 m_maxBranchSize;
        public List<GUIRect> m_content;
        public List<GUILayout> m_branches;
        public GUILayoutOptions[] m_options;
        public GUILayout()
        {
            m_root = null;
            m_isBranch = false;
            m_spacing = Vector2.Zero;
            m_padding = Vector2.Zero;
            m_content = new List<GUIRect>();
            m_branches = new List<GUILayout>();
            m_options = new GUILayoutOptions[0];
        }

        public GUILayout GetMasterRoot()
        {
            if (!m_isBranch) return this;
            else
            {
                return m_root.GetMasterRoot();
            }
        }
    }

    public enum GUILayoutOptions
    {
        STRETCH_WIDTH,
        STRETCH_HEIGHT,
        CONTENT_CENTER,
        CONTENT_CENTER_VERTICAL,
        CONTENT_CENTER_HORIZONTAL,
        SNAP_LEFT,
        SNAP_RIGHT,
        SNAP_TOP,
        SNAP_BOTTOM,
    }
}

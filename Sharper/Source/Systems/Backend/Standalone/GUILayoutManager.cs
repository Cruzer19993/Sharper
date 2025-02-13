using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sharper.Components.GUI;
using Sharper.Systems.Backend;
using System;
using System.Linq;
namespace Sharper.Backend.Standalone
{
    //manages GUIRect positions and sizes for layout groups
    public class GUILayoutManager
    {
        static GraphicsDeviceManager device = RenderingSystem.Instance._graphics;
        public static void UpdateGUILayout(GUILayout layout)
        {
            GUIRect layoutRect = layout.owner.GetComponent<GUIRect>();
            Vector2 currentRectSize = layoutRect.GetSize();
            Vector2 currentRectPos = layoutRect.GetPosition();
            int windowHeight = device.PreferredBackBufferHeight;
            int windowWidth = device.PreferredBackBufferWidth;
            //Apply layout options
            foreach (GUILayoutOptions option in layout.m_layoutOptions)
            {
                switch (option)
                {
                    case GUILayoutOptions.STRETCH_WIDTH:
                        layoutRect.SetSize(new Vector2(windowWidth, currentRectSize.Y));
                        break;
                    case GUILayoutOptions.STRETCH_HEIGHT:
                        layoutRect.SetSize(new Vector2(currentRectSize.X, windowHeight));
                        break;
                    case GUILayoutOptions.SNAP_LEFT:
                        layoutRect.SetPosition(new Vector2(0, currentRectPos.Y));
                        break;
                    case GUILayoutOptions.SNAP_RIGHT:
                        layoutRect.SetPosition(new Vector2(windowWidth - currentRectSize.X, currentRectPos.Y));
                        break;
                    case GUILayoutOptions.SNAP_TOP:
                        layoutRect.SetPosition(new Vector2(currentRectPos.X, 0));
                        break;
                    case GUILayoutOptions.SNAP_BOTTOM:
                        layoutRect.SetPosition(new Vector2(currentRectPos.X, windowHeight - currentRectSize.Y));
                        break;
                    case GUILayoutOptions.CONTENT_CENTER_HORIZONTAL:
                        if (layout.m_managedObjects.Count <= 0) break;
                        int biggestWidth = (int)layout.m_managedObjects.Max(x => x.GetSize().X);
                        int newPaddingX = (int)(Math.Abs(currentRectSize.X - biggestWidth) / 2);
                        layout.m_padding.X = newPaddingX;
                        break;
                    case GUILayoutOptions.CONTENT_CENTER_VERTICAL:
                        if (layout.m_managedObjects.Count <= 0) break;
                        int biggestHeight = (int)layout.m_managedObjects.Max(x => x.GetSize().Y);
                        int newPaddingY = (int)(Math.Abs(currentRectSize.Y - biggestHeight) / 2);
                        layout.m_padding.Y = newPaddingY;
                        break;
                }
                Vector2 padding = layout.m_padding;
                Vector2 offset = layout.m_offset;
                Vector2 spacing = layout.m_spacing;
                Vector2 layoutDistanceCovered = Vector2.Zero;
                foreach (GUIRect rect in layout.m_managedObjects)
                {
                    Vector2 rectSize = rect.GetSize();
                    Vector2 rectCoverDistance = new Vector2(spacing.X == 0 ? 0:rectSize.X,spacing.Y == 0 ? 0:rectSize.Y);
                    //move objects according to padding and spacing
                    if (rect == layout.m_managedObjects.First()) {
                        rect.SetPosition(new Vector2(currentRectPos.X + padding.X+spacing.X, currentRectPos.Y + padding.Y+spacing.Y));
                        layoutDistanceCovered += padding + offset + rectCoverDistance;
                    }
                    else if(rect == layout.m_managedObjects.Last())
                    {
                        rect.SetPosition(new Vector2(currentRectPos.X + layoutDistanceCovered.X + offset.Y, currentRectPos.Y + layoutDistanceCovered.Y + offset.Y));
                        layoutDistanceCovered +=rectCoverDistance+spacing+padding;
                        layoutRect.SetSize(new Vector2(layoutDistanceCovered.X == 0 ? layout.m_managedObjects.Max(x => x.GetSize().X):layoutDistanceCovered.X, layoutDistanceCovered.Y == 0 ? layout.m_managedObjects.Max(x => x.GetSize().Y):layoutDistanceCovered.Y));
                    }
                    else
                    {
                        rect.SetPosition(new Vector2(currentRectPos.X+layoutDistanceCovered.X+spacing.X,currentRectPos.Y+layoutDistanceCovered.Y+spacing.Y));
                        layoutDistanceCovered += rectCoverDistance + spacing;
                    }
                    //update parent layout with new positions of child layout (changed rect size etc).
                    if (rect.m_isManaged)
                    {
                        rect.m_rectManager.InvokeOnUpdate();
                    }
                }
            }
        }

        public static void AddToLayoutGroup(ref GUILayout layoutGroup, GUIRect GUIRect)
        {
            layoutGroup.m_managedObjects.Add(GUIRect);
            GUIRect.m_rectManager = layoutGroup;
            GUIRect.m_isManaged = true;
            UpdateGUILayout(layoutGroup);
        }
    }
}

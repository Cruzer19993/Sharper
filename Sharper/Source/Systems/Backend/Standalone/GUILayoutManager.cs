using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sharper.Components.GUI;
using Sharper.Systems.Backend.Management;
namespace Sharper.Systems.Backend.Standalone
{   
    static public class GUILayoutManager
    {
        static GraphicsDeviceManager graphicsDevManager = RenderingSystem.Instance._graphics;
        
        static void UpdateLayout(GUILayout layout)
        {
            int windowWidth = graphicsDevManager.PreferredBackBufferWidth;
            int windowHeight = graphicsDevManager.PreferredBackBufferHeight;
            GUIRect layoutRect = layout.owner.GetComponent<GUIRect>();
            Vector2 rootRectSize = new Vector2(windowWidth, windowHeight);
            bool centerVert = false;
            bool centerHor = false;
            if (layout.m_isBranch) 
            {
                rootRectSize = layout.m_root.owner.GetComponent<GUIRect>().m_size - layout.m_root.m_padding*2;
            }
            //Apply layout options
            foreach(GUILayoutOptions option in layout.m_options)
            {
                switch (option)
                {
                    case GUILayoutOptions.STRETCH_WIDTH:
                        Vector2 WnewLayoutSize = new Vector2(rootRectSize.X, layoutRect.m_size.Y);
                        layoutRect.m_size = WnewLayoutSize;
                        break;
                    case GUILayoutOptions.STRETCH_HEIGHT:
                        Vector2 HnewLayoutSize = new Vector2(layoutRect.m_size.X,rootRectSize.Y);
                        layoutRect.m_size = HnewLayoutSize;
                        break;
                    case GUILayoutOptions.CONTENT_CENTER_VERTICAL:
                        centerVert = true;
                        break;
                    case GUILayoutOptions.CONTENT_CENTER_HORIZONTAL:
                        centerHor = true;
                        break;
                    case GUILayoutOptions.CONTENT_CENTER:
                        centerVert = true;
                        centerHor = true;
                        break;
                }
            }
            //This should run starting with master root layout.
            Vector2 currentPosition = layout.owner.GetComponent<GUIRect>().m_position + layout.m_padding;
            //Calculate currentPosition for centering.
            Vector2 layoutCenter = Vector2.Zero;
            if (centerHor || centerVert)
            {
                layoutCenter = layoutRect.m_size / 2;
                Vector2 totalContentSize = new Vector2(centerHor? layout.m_content.Max(x => x.m_size.X):layout.m_content.Sum(x => x.m_size.X) ,centerVert ?layout.m_content.Max(y => y.m_size.Y) : layout.m_content.Sum(y => y.m_size.Y));
                //find the starting position for content.
                currentPosition = layoutRect.m_position + layoutCenter * new Vector2(centerHor ? 1 : 0,centerVert ? 1 : 0);
            }
            foreach(GUIRect element in layout.m_content)
            {
                //Calculate new content positions
                
                Vector2 newElementPos = currentPosition + layout.m_spacing - ((element.m_size / 2) * new Vector2(centerHor ? 1 : 0, centerVert ? 1 : 0));
                element.m_position = newElementPos;
                currentPosition += element.m_size * new Vector2(layout.m_spacing.X > 0 ? 1 : 0, layout.m_spacing.Y > 0 ? 1 : 0) + layout.m_spacing;
                
            }
            //Update branches.
            foreach(GUILayout branch in layout.m_branches)
            {
                UpdateLayout(branch);
            }
        }

        public static void ResizeText(GUIRect rect,string textString)
        {
            rect.m_size = new Vector2(Math.Max(40, ResourceManager.Instance.GetDefaultFont().MeasureString(textString).X+2), 20);
        }
        public static void RootLayout(GUILayout rootLayout, GUILayout branchLayout)
        {
            branchLayout.m_isBranch = true;
            branchLayout.m_root = rootLayout;
            rootLayout.m_content.Add(branchLayout.owner.GetComponent<GUIRect>());
            rootLayout.m_branches.Add(branchLayout);
        }
        public static void AddContent(GUILayout layout, GUIRect content)
        {
            layout.m_content.Add(content);
            UpdateLayout(layout.GetMasterRoot());
        }
    }
}

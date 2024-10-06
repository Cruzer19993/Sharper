using Microsoft.Xna.Framework;
using MonoGAME.Components.GUI;
namespace MonoGAME.Backend.Standalone
{
    //manages GUIRect positions and sizes for layout groups
    public class GUILayoutManager
    {
        public static void UpdateGUILayout(ref GUILayout layout)
        {
            bool enforceCellSize = false;
            Vector2 cellSize = Vector2.Zero;
            Vector2 distanceCovered = Vector2.Zero; //adds size of group objects so that we don't overlap them
            Vector2 padding = layout.m_padding;
            Vector2 spacing = layout.m_spacing;
            Vector2 offset = layout.m_offset;
            Vector2 layoutGroupPosition = layout.owner.GetComponent<GUIRect>().GetPosition();
            int gridColumns = 0;
            layoutGroupPosition += offset;
            int gridColumn = 0;
            int gridLine = 0;
            int index = 0;
            int size = layout.m_managedObjects.Count;
            if(layout is GUIGrid)
            {
                GUIGrid grid = layout as GUIGrid;
                enforceCellSize = grid.m_enforceCellSize;
                cellSize = grid.m_cellSize;
                gridColumns = grid.m_columns;
            }
            foreach (GUIRect groupObjectRect in layout.m_managedObjects)
            {
                distanceCovered = new Vector2(spacing.X == 0 ? 0 : distanceCovered.X, spacing.Y == 0 ? 0 : distanceCovered.Y);
                Vector2 newRectPosition = layoutGroupPosition + (Vector2.One * distanceCovered) + spacing + padding;
                groupObjectRect.SetPosition(newRectPosition);
                if (enforceCellSize)
                {
                    if(gridColumn == gridColumns)
                    {
                        gridColumn = 0;
                        gridLine++;
                    }
                    newRectPosition = layoutGroupPosition + (spacing*new Vector2(gridColumn,gridLine)+cellSize * new Vector2(gridColumn, gridLine)) + padding;
                    groupObjectRect.SetPosition(newRectPosition);
                    groupObjectRect.SetSize(cellSize);
                    gridColumn++;
                }
                distanceCovered += groupObjectRect.GetSize() + spacing;
                GUIRect layoutGUIRect = layout.owner.GetComponent<GUIRect>();
                Vector2 localPosition = (newRectPosition - layoutGroupPosition);
                Vector2 layoutGUIRectSize = layoutGUIRect.GetSize();
                Vector2 objectBoundarySize = localPosition + groupObjectRect.GetSize();
                Vector2 newLayoutSize = new Vector2(layoutGUIRectSize.X < objectBoundarySize.X ? objectBoundarySize.X : layoutGUIRectSize.X, layoutGUIRectSize.Y < objectBoundarySize.Y ? objectBoundarySize.Y : layoutGUIRectSize.Y);
                layoutGUIRect.SetSize(newLayoutSize);
                index++;
            }
            layout.InvokeOnUpdate();
        }
        public static void AddToLayoutGroup(ref GUILayout layoutGroup, GUIRect GUIRect)
        {
            layoutGroup.m_managedObjects.Add(GUIRect);
            if (GUIRect.owner.HasComponent<GUILayout>())
            {
                GUILayout childLayout = GUIRect.owner.GetComponent<GUILayout>();
                layoutGroup.OnUpdate += (sender,e) => UpdateGUILayout(ref childLayout);
            }
            UpdateGUILayout(ref layoutGroup);
        }
    }
}

using Microsoft.Xna.Framework;
using Sharper.Components.GUI;
using Sharper.ECS;

namespace Sharper.Components.GUI
{
    public class GUIGrid : GUILayout
    {
        //Parameterless constructor is needed for every component
        public GUIGrid(Vector2 spacing, Vector2 padding, Vector2 offset, Vector2 cellSize, int columns,bool enforceCellSize) : base(spacing, padding, offset)
        {
            m_cellSize = cellSize;
            m_enforceCellSize = enforceCellSize;
            m_columns = columns;
        }
        public GUIGrid() : base()
        {
            m_cellSize = Vector2.One;
            m_enforceCellSize = true;
            m_columns = 0;
        }
        public Vector2 m_cellSize;
        public bool m_enforceCellSize;
        public int m_columns;
    }
}

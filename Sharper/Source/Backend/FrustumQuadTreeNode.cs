using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharper.ECS;
using Sharper.Systems.Backend;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Sharper.Backend
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Xml.Linq;

    public class FrustumQuadTreeNode
    {
        public FrustumQuadTreeNode(Rectangle nodeRect, int level, int maxLevels)
        {
            nodeBoundary = nodeRect;
            this.level = level;
            nodeObjects = new List<int>();
            maxLevel = maxLevels;
            if(this.level < maxLevels)
            {
                Subdivide(maxLevels);
            }
        }
        private int level;
        int maxLevel;
        public FrustumQuadTreeNode[] children = null;
        public Rectangle nodeBoundary;
        public List<int> nodeObjects;


        public void Insert(Rectangle entityRect, int entityID)
        {
            //traverse the tree
            if(children != null)
                foreach (var child in children)
                {
                    if (child.nodeBoundary.Intersects(entityRect))
                    {
                        child.Insert(entityRect, entityID);
                        return;
                    }
                }
            nodeObjects.Add(entityID);
        }

        private void Subdivide(int maxLevels)
        {
            int halfWidth = nodeBoundary.Width / 2;
            int halfHeight = nodeBoundary.Height / 2;
            children = new FrustumQuadTreeNode[4];
            children[0] = new FrustumQuadTreeNode(new Rectangle((int)nodeBoundary.X, (int)nodeBoundary.Y, halfWidth, halfHeight), level + 1, maxLevels); // Top-left
            children[1] = new FrustumQuadTreeNode(new Rectangle((int)(nodeBoundary.X + halfWidth), (int)nodeBoundary.Y, halfWidth, halfHeight), level + 1, maxLevels); // Top-right
            children[2] = new FrustumQuadTreeNode(new Rectangle((int)nodeBoundary.X, (int)(nodeBoundary.Y + halfHeight), halfWidth, halfHeight), level + 1, maxLevels); // Bottom-left
            children[3] = new FrustumQuadTreeNode(new Rectangle((int)(nodeBoundary.X + halfWidth), (int)(nodeBoundary.Y + halfHeight), halfWidth, halfHeight), level + 1, maxLevels); // Bottom-right
        }

        public void Query(Rectangle frustum, List<int> results)
        {
            // If this node does not intersect the frustum, skip it
            if (nodeBoundary.Intersects(frustum))
            {//if we intersect, skip this node and go to m_children.
                if (children != null)
                {
                    foreach (var child in children)
                    {
                        child.Query(frustum, results);
                    }
                }
                results.AddRange(nodeObjects);
            }
            else if (frustum.Contains(nodeBoundary) || level == (maxLevel - 1)) //if we are contained in the frustum.
            {
                results.AddRange(nodeObjects);
            }
            else return;

            // Add objects in this node to the results
        }
    }

}

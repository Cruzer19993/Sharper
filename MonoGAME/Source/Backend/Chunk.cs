using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGAME.ECS;
using MonoGAME.Systems.Backend;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace MonoGAME.Backend
{
    public class Chunk
    {
        protected event EventHandler<EntityAddedToChunkEventArgs> EntityAddedToChunk;
        public Chunk(Rectangle rect, int level)
        {
            chunkRect = rect;
            chunkEntities = new Dictionary<int, Entity>();
            this.level = level;
            PPS = RenderingSystem.Instance.SpriteAtlasSettings.pixelsPerSprite;
        }
        private Rectangle chunkRect;
        public Dictionary<int, Entity> chunkEntities;
        public Chunk[] nodes;
        public int nodeDepth;
        private int level;
        private int PPS;

        //Subdivides the chunk into 4 smaller chunks, if depth > 0, it will subdivide the chunks of levl {depth} also

        public Rectangle GetChunkRect()
        {
            return chunkRect;
        }
        public Chunk[] GetNodes()
        {
            return nodes;
        }
        public Chunk[] GetNodesAtLevel(int level)
        {
            List<Chunk> nodes = new List<Chunk>();

            // Check if the current node is at the desired level
            if (this.level == level)
            {
                nodes.Add(this);
            }
            else
            {
                // Recursively traverse the child nodes
                foreach (Chunk node in this.nodes)
                {
                    Chunk[] deeperNodes = node.GetNodesAtLevel(level);
                    nodes.AddRange(deeperNodes);
                }
            }

            return nodes.ToArray();
        }
        public void DeepenChunk()
        {
            int subdivisions = 2;
            int newChunkLevel = level + 1;
            int chunkWidth = chunkRect.Width / subdivisions;
            int chunkHeight = chunkRect.Height / subdivisions;
            //if this node is subdivided already.
            if (nodes != null)
            {
                foreach (Chunk node in nodes)
                {
                    node.DeepenChunk();
                    foreach(Chunk deeperNode in node.nodes)
                    {
                        EntityAddedToChunk += deeperNode.OnEntityAddedToSurfaceChunk;
                    }
                }
            }
            else
            {
                nodes = new Chunk[subdivisions * subdivisions];

                for (int x = 0; x < subdivisions; x++)
                {
                    for (int y = 0; y < subdivisions; y++)
                    {
                        Chunk newChunk = new Chunk(new Rectangle(chunkRect.X + (chunkWidth * x), chunkRect.Y + (chunkHeight * y), chunkWidth, chunkHeight), newChunkLevel);
                        newChunk.AddEntities(chunkEntities.Values.ToArray());
                        EntityAddedToChunk += newChunk.OnEntityAddedToSurfaceChunk;
                        nodes[x + y * subdivisions] = newChunk;
                    }
                }
            }
            nodeDepth++;
        }

        protected void OnEntityAddedToSurfaceChunk(object sender, EntityAddedToChunkEventArgs args)
        {
            if (chunkRect.Contains(args.entityRect))
            {
                chunkEntities.Add(args.entityID, args.entity);
            }
        }

        public void AddEntities(Entity[] entities)
        {
            foreach (Entity entity in entities)
            {
                if (!entity.HasComponent<Transform>()) continue;
                Transform entityTransform = entity.GetComponent<Transform>();
                Rectangle gridBoundingBox = new Rectangle((int)entityTransform.position.X,(int)entityTransform.Position.Y, PPS, PPS);   
                if (chunkRect.Contains(gridBoundingBox))
                {
                    chunkEntities.Add(entity.entityID, entity);
                }
            }
            if (level > 0 && entities.Length > 0) Debug.WriteLine($"Got {entities.Length} entities, added {chunkEntities.Count} entities.");
        }

        public void AddEntity(ref Entity entity)
        {
            if (level > 0) return;
            if(!entity.HasComponent<Transform>()) return;
            Transform entityTransform = entity.GetComponent<Transform>();
            Rectangle boundingBox = new Rectangle((int)entityTransform.position.X, (int)entityTransform.Position.Y, PPS, PPS);
            if (chunkRect.Contains(boundingBox))
            {
                chunkEntities.Add(entity.entityID, entity);
                EntityAddedToChunk.Invoke(this, new EntityAddedToChunkEventArgs(ref boundingBox,ref entity));
            }
        }

        public void RemoveEntity(ref int entityID)
        {
            if (chunkEntities.ContainsKey(entityID))
            {
                chunkEntities.Remove(entityID);
            }
        }
        public void SubdivideChunk()
        {
            // Define the number of subdivisions
            int subdivisions = 2;
            int newChunkLevel = level + 1;
            int chunkWidth = chunkRect.Width / subdivisions;
            int chunkHeight = chunkRect.Height / subdivisions;

            // Initialize the nodes array if it's null
            if (nodes == null)
            {
                nodes = new Chunk[subdivisions * subdivisions];
            }

            // Subdivide the chunk into quarters
            for (int x = 0; x < subdivisions; x++)
            {
                for (int y = 0; y < subdivisions; y++)
                {
                    // Calculate the rectangle for the new chunk
                    Rectangle newChunkRect = new Rectangle(
                        chunkRect.X + (chunkWidth * x),
                        chunkRect.Y + (chunkHeight * y),
                        chunkWidth,
                        chunkHeight
                    );

                    // Create the new chunk and add it to the nodes array
                    Chunk newChunk = new Chunk(newChunkRect, newChunkLevel);
                    nodes[x + y * subdivisions] = newChunk;
                }
            }

            // Update the node depth
            nodeDepth = newChunkLevel;

            // Recursively subdivide the nodes if they are already subdivided
            foreach (var node in nodes)
            {
                node.SubdivideChunk();
            }
        }
    }
    public class EntityAddedToChunkEventArgs : EventArgs
    {
        public EntityAddedToChunkEventArgs(ref Rectangle rect,ref Entity entity)
        {
            entityRect = rect;
            this.entity = entity;
            this.entityID = this.entity.entityID;
        }
        public Rectangle entityRect;
        public Entity entity;
        public int entityID;
    }
}

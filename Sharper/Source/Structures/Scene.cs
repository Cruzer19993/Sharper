using Sharper.Systems.Backend.Loaders;
using Sharper.Systems.Backend.Management;
using Sharper.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharper.Systems.Backend;
using System.Diagnostics;
using Sharper.Backend;
using Microsoft.Xna.Framework;

namespace Sharper.Structures
{
    public class Scene
    {
        //size of level 0 chunks in grid units. (get the pixel size: PPS * ZERO_LEVEL_CHUNK_SIZE)
        private static readonly int ZERO_LEVEL_CHUNK_SIZE = 128;
        private static readonly int ZERO_LEVEL_CHUNK_DISTANCE = 32;
        private int PPS;
        public Scene(string sceneName = "")
        {
            this.sceneName = sceneName;
            entityManager = new EntityManager();
            systemManager = new SystemManager();
            worldLoader = new WorldLoader();
        }
        public bool useWorldLoader;
        public string sceneName;
        public EntityManager entityManager;
        public SystemManager systemManager;
        public WorldLoader worldLoader;
        public List<Chunk> entityChunks;
        public void OnSceneLoad(SceneManager manager)
        {
            PPS = RenderingSystem.Instance.SpriteAtlasSettings.pixelsPerSprite;
            CreateSurfaceChunks();
            string currentMapName;
            if (useWorldLoader)
            {
                foreach (Entity tile in worldLoader.LoadMapFromTextureRefactored("testmap", out currentMapName))
                {
                   // SpawnEntity(ref tile);
                }
            }
            manager.DoneLoadingScene();
        }


        public void CreateSurfaceChunks()
        {
            if(entityChunks == null)
            {
                entityChunks = new List<Chunk>(ZERO_LEVEL_CHUNK_DISTANCE * ZERO_LEVEL_CHUNK_DISTANCE);
            }
            int halfChunkDistance = ZERO_LEVEL_CHUNK_DISTANCE / 2;
            //Creating the chunks.
            for (int x = -halfChunkDistance; x < halfChunkDistance; x++)
            {
                for (int y = -halfChunkDistance; y < halfChunkDistance; y++)
                {
                    Rectangle chunkRect = new Rectangle(x * ZERO_LEVEL_CHUNK_SIZE * PPS, y * ZERO_LEVEL_CHUNK_SIZE * PPS, ZERO_LEVEL_CHUNK_SIZE*PPS, ZERO_LEVEL_CHUNK_SIZE*PPS);
                    Chunk newChunk = new Chunk(chunkRect, 0);
                    entityChunks.Add(newChunk);
                    newChunk.DeepenChunk();
                    newChunk.DeepenChunk();
                }
            }
        }
        public void SpawnEntity(Entity entity)
        {
            entityManager.RegisterEntity(ref entity);
        }
    }
}

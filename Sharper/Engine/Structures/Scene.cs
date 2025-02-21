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
        public void OnSceneLoad(SceneManager manager)
        {
            PPS = RenderingSystem.Instance.SpriteAtlasSettings.pixelsPerSprite;
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

        public void SpawnEntity(Entity entity)
        {
            entityManager.RegisterEntity(ref entity);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGAME.Components;
using MonoGAME.ECS;
using MonoGAME.Systems.Backend.Management;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MonoGAME.Structures;
using MonoGAME.Backend;
namespace MonoGAME.Systems.Backend
{
    public class RenderingSystem : ECSSystem
    {
        public GraphicsDeviceManager _graphics;
        public static EventHandler<CameraChangedEventArgs> CameraChangedEvent;
        public RenderingSystem()
        {
            if (instance == null) instance = this;
        }
        private static RenderingSystem instance = null;
        public static RenderingSystem Instance
        {
            get { return instance; }
            private set {  instance = value; }
        }
        public Entity[] availableCameras;
        public Camera currentCamera;
        private Scene currentScene;
        public Transform currentCameraTransform;
        public int RENDERING_SYSTEM_MINIMUM_STARTING_MATCHING_ENTITIES_SIZE = 1024;
        public Texture2D textureAtlas;
        public EntityManager currentLevelEntityManager;
        private SpriteAtlasSettings atlasSettings = new SpriteAtlasSettings();
        public SpriteAtlasSettings SpriteAtlasSettings { get { return atlasSettings; } set { atlasSettings = value; } }
        public int frustumCullingOffset = 8;
        public void SetRenderingSettings(SpriteAtlasSettings _atlasSettings)
        {
            atlasSettings = _atlasSettings;
        }
        public Dictionary<int, CachedEntity> renderingCache = new Dictionary<int, CachedEntity>(2000);
        public override void Initialize()
        {
            matchingComponentTypes = new Type[] { typeof(SpriteRenderer),typeof(Transform)};
            base.Initialize();
            Entity[] resizeTemp = matchingEntities;
            matchingEntities = new Entity[RENDERING_SYSTEM_MINIMUM_STARTING_MATCHING_ENTITIES_SIZE];
            resizeTemp.CopyTo(matchingEntities, 0);
            resizeTemp = null;
            availableCameras = entityManager.GetEntitiesWithComponents(new Type[] { typeof(Camera), typeof(Transform) });
            foreach(Entity camera in availableCameras)
            {
                if (camera.GetComponent<Camera>().isMainCamera)
                {
                    currentCamera = camera.GetComponent<Camera>();
                    currentCameraTransform = camera.GetComponent<Transform>();
                }
            }
        }

        public override void OnNewEntity(Entity newEntity)
        {
            // Get the position of the new entity
            Transform transform = newEntity.GetComponent<Transform>();
            Vector2 position = new Vector2(transform.Position.X, transform.Position.Y);

            // Find the surface chunk that contains the entity's position
            Chunk surfaceChunk = currentScene.entityChunks.Find(chunk => chunk.GetChunkRect().Contains(position));

            // Add the new entity to the surface chunk
            surfaceChunk.AddEntity(ref newEntity);
        }
        public bool isCached(ref int entityID)
        {
            if(renderingCache.ContainsKey(entityID))
                return true;
            else return false;
        }
        public void DecacheEntity(ref int entityID)
        {
            if(renderingCache.ContainsKey(entityID))
                renderingCache.Remove(entityID);
        }
        public void CacheEntity(ref int entityID, ref CachedEntity cachedEntity)
        {
            renderingCache.Add(entityID, cachedEntity);
        }
        public bool TryGetCachedEntity(ref int entityID, out CachedEntity cachedEntity)
        {
            if (renderingCache.TryGetValue(entityID, out cachedEntity))
            {
                return true;
            }
            else return false;
        }
            public void CheckForCameras()
        {
            availableCameras = entityManager.GetEntitiesWithComponents(new Type[] { typeof(Camera), typeof(Transform) });
            if(availableCameras.Length > 0)
                foreach (Entity camera in availableCameras)
                {
                    if (camera.GetComponent<Camera>().isMainCamera)
                    {
                        currentCamera = camera.GetComponent<Camera>();
                        currentCameraTransform = camera.GetComponent<Transform>();
                        CameraChangedEvent.Invoke(this, new CameraChangedEventArgs(currentCamera, camera));
                        Debug.WriteLine("[INFO]Found a camera");
                    }
                }
        }

        public void OnSceneChange(object sender,SceneEventArgs args)
        {
            currentScene = args.argScene;
            entityManager = currentScene.entityManager;
        }
        public override void GameUpdate()
        {
        }
        public override void EntityUpdate(Entity target)
        {

        }
        public override void Start()
        {

        }
        public Rectangle GetCameraFrustumRectangle()
        {
            if (currentCamera == null || currentCameraTransform == null)
                return Rectangle.Empty;
            //When i multiplied this by 4 only I and god knew why, now, only god knows
            float cameraScale = 1/currentCamera.cameraZoom;
            int frustumWidth = (int)(_graphics.PreferredBackBufferWidth*cameraScale);
            int frustumHeight = (int)(_graphics.PreferredBackBufferHeight*cameraScale);
            int frustumX = (int)(currentCameraTransform.Position.X*cameraScale);
            int frustumY = (int)(currentCameraTransform.Position.Y * cameraScale);
            return new Rectangle(frustumX, frustumY, frustumWidth, frustumHeight);
        }
    }

    public class CameraChangedEventArgs : EventArgs
    {
        public CameraChangedEventArgs(Camera camera, Entity entity)
        {
            newCamera = camera;
            newCameraEntity = entity;
        }
        public Camera newCamera;
        public Entity newCameraEntity;
    }

    public struct CachedEntity
    {
        public CachedEntity(ref Entity _entity, ref Transform _transform, ref SpriteRenderer _renderer, ref Sprite _sprite)
        {
            entity = _entity;
            entityTransform = _transform;
            entityRenderer = _renderer;
            entitySprite = _sprite;
        }
        public Entity entity;
        public Transform entityTransform;
        public SpriteRenderer entityRenderer;
        public Sprite entitySprite;
    }
    public struct SpriteAtlasSettings
    {
        public SpriteAtlasSettings(int _pixelsPerSprite, int _spritesPerLine, int _lines)
        {
            pixelsPerSprite = _pixelsPerSprite;
            spritesPerLine = _spritesPerLine;
            lines = _lines;
        }
        //16x16, 32x32 etc, square sprites only
        public int pixelsPerSprite;
        public int spritesPerLine;
        public int lines;
    }
}

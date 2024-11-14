using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sharper.Components;
using Sharper.ECS;
using Sharper.Systems.Backend.Management;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sharper.Structures;
using Sharper.Backend;
using Sharper.Components.GUI;
using System.Linq;
namespace Sharper.Systems.Backend
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
            private set { instance = value; }
        }
        public Entity[] availableCameras;
        public int frustumCullingOffset = 8;
        public Camera currentCamera;
        public Transform currentCameraTransform;
        public int RENDERING_SYSTEM_MINIMUM_STARTING_MATCHING_ENTITIES_SIZE = 1024;
        public Texture2D textureAtlas;
        public EntityManager currentLevelEntityManager;

        private SpriteAtlasSettings atlasSettings = new SpriteAtlasSettings();
        private bool entitiesChanged = true;
        private Entity[] gridEntities;
        private Entity[] worldEntities;
        private Entity[] gUIEntities;
        private Entity[] textEntities;
        private Scene currentScene;

        public Entity[] GridEntities
        {
            get { return gridEntities; }
            private set { gridEntities = value; }
        }
        public Entity[] WorldEntities
        {
            get { return worldEntities; }
            private set { worldEntities = value; }
        }
        public Entity[] GUIEntities
        {
            get { return gUIEntities; }
            private set { gUIEntities = value; }
        }
        public Entity[] TextEntities
        {
            get { return textEntities; }
            private set { textEntities = value; }
        }

        public SpriteAtlasSettings SpriteAtlasSettings { get { return atlasSettings; } set { atlasSettings = value; } }

        public void SetRenderingSettings(SpriteAtlasSettings _atlasSettings)
        {
            atlasSettings = _atlasSettings;
        }
        public override void Initialize()
        {

            matchingComponentTypes = new Type[] { typeof(EntityRenderer) };
            partialMatchingComponentTypes = new Type[] { typeof(GUIRect), typeof(Transform), typeof(GUIText) };
            base.Initialize();
            Entity[] resizeTemp = matchingEntities;
            matchingEntities = new Entity[RENDERING_SYSTEM_MINIMUM_STARTING_MATCHING_ENTITIES_SIZE];
            resizeTemp.CopyTo(matchingEntities, 0);
            resizeTemp = null;
            availableCameras = entityManager.GetEntitiesWithComponents(new Type[] { typeof(Camera), typeof(Transform) });
            foreach (Entity camera in availableCameras)
            {
                if (camera.GetComponent<Camera>().isMainCamera)
                {
                    currentCamera = camera.GetComponent<Camera>();
                    currentCameraTransform = camera.GetComponent<Transform>();
                }
            }
        }
        public void UpdateGridEntities()
        {
            if (realArrayEntityCount <= 0) return;
            if (!entitiesChanged) return;
            Rectangle frustumRect = GetCameraFrustumRectangle();
            Chunk[] gridChunks = currentScene.entityChunks.ToArray();
            List<Entity> visibleEntities = new List<Entity>();
            int pps = atlasSettings.pixelsPerSprite;
            foreach (Chunk chunk in gridChunks) //check surface chunks
            {
                //surface level checks
                Rectangle surfaceChunkRect = chunk.GetChunkRect(); //fully visible
                if (frustumRect.Contains(surfaceChunkRect))
                {
                    visibleEntities.AddRange(chunk.chunkEntities.Values);
                }
                else if (frustumRect.Intersects(surfaceChunkRect)) //intersects.
                {
                    bool checkEntitiesOnFinalDepth = false;
                    for (int i = 1; i < chunk.nodeDepth; i++)
                    {
                        Chunk[] deeperChunks = chunk.GetNodesAtLevel(i);
                        foreach (Chunk deeperChunk in deeperChunks)
                        {
                            Rectangle deeperChunkRect = deeperChunk.GetChunkRect();
                            if (frustumRect.Contains(deeperChunkRect))//fully visible
                            {
                                visibleEntities.AddRange(deeperChunk.chunkEntities.Values);
                            }
                            if (frustumRect.Intersects(deeperChunkRect)) //partially visible
                            {
                                if (i == chunk.nodeDepth - 1)
                                {
                                    checkEntitiesOnFinalDepth = true;
                                }
                                else continue;
                            }
                            else continue;//not visible
                        }
                    }
                    if (checkEntitiesOnFinalDepth)
                    {
                        Chunk[] finalChunks = chunk.GetNodesAtLevel(chunk.nodeDepth);
                        foreach (Chunk finalChunk in finalChunks)
                        {
                            foreach (Entity entity in finalChunk.chunkEntities.Values)
                            {
                                Transform entityTransform = entity.GetComponent<Transform>();
                                Rectangle entityRect = new Rectangle((int)entityTransform.position.X, (int)entityTransform.position.Y, pps, pps);
                                if (frustumRect.Intersects(entityRect)) //check if entity is visible
                                {
                                    visibleEntities.Add(entity);
                                }
                            }
                        }
                        checkEntitiesOnFinalDepth = false;
                    }
                }
                else continue; //not visible
            }
            GridEntities = visibleEntities.ToArray();
        }
        public void UpdateWorldEntites()
        {
            if (realArrayEntityCount <= 0) return;
            worldEntities = matchingEntities.ToList<Entity>().FindAll(x => x.GetComponent<EntityRenderer>().m_renderTarget == RenderTarget.WorldSpace).ToArray();
        }
        public void UpdateGUIEntities()
        {
            if (realArrayEntityCount <= 0) return;
            gUIEntities = matchingEntities.ToList<Entity>().FindAll(x => x.GetComponent<EntityRenderer>().m_renderTarget == RenderTarget.GUI).ToArray();
        }
        public void UpdateTextEntities()
        {
            if (realArrayEntityCount <= 0) return;
            textEntities = matchingEntities.ToList<Entity>().FindAll(x => x.GetComponent<EntityRenderer>().m_renderTarget == RenderTarget.Text).ToArray();
        }
        public override void OnNewEntity(Entity newEntity)
        {
            RenderTarget entityRenderTarget = newEntity.GetComponent<EntityRenderer>().m_renderTarget;
            // Get the position of the new entity
            Transform transform = newEntity.GetComponent<Transform>();
            switch (entityRenderTarget) {
                case RenderTarget.GridSpace:
                Vector2 position = new Vector2(transform.Position.X, transform.Position.Y);

                // Find the surface chunk that contains the entity's position
                Chunk surfaceChunk = currentScene.entityChunks.Find(chunk => chunk.GetChunkRect().Contains(position));

                // Add the new entity to the surface chunk
                surfaceChunk.AddEntity(ref newEntity);
                    UpdateGridEntities();
                    break;
                case RenderTarget.WorldSpace:
                    UpdateWorldEntites();
                    break;
                case RenderTarget.GUI:
                    UpdateGUIEntities();
                    break;
                case RenderTarget.Text:
                    UpdateTextEntities();
                    break;
            }
        }
        public void CheckForCameras()
        {
            availableCameras = entityManager.GetEntitiesWithComponents(new Type[] { typeof(Camera), typeof(Transform) });
            if (availableCameras.Length > 0)
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

        public void OnSceneChange(object sender, SceneEventArgs args)
        {
            currentScene = args.argScene;
            entityManager = currentScene.entityManager;
        }

        private void OnEntityChanged(object sender, EventArgs args)
        {
            UpdateGridEntities();
            UpdateWorldEntites();
            UpdateGUIEntities();
            UpdateTextEntities();
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
        public CachedEntity(ref Entity _entity, ref Transform _transform, ref EntityRenderer _renderer, ref Sprite _sprite)
        {
            entity = _entity;
            entityTransform = _transform;
            entityRenderer = _renderer;
            entitySprite = _sprite;
        }
        public Entity entity;
        public Transform entityTransform;
        public EntityRenderer entityRenderer;
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

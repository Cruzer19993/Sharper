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
using System.Collections;
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
        public Entity[] availableCameras = new Entity[0];
        public int frustumCullingOffset = 8;
        public Camera currentCamera;
        public Transform currentCameraTransform;
        public Texture2D textureAtlas;
        public EntityManager currentLevelEntityManager;
        private FrustumQuadTreeNode worldObjectsQuadTreeMainNode;
        private SpriteAtlasSettings atlasSettings = new SpriteAtlasSettings();
        private bool entitiesChanged = true;
        private Scene currentScene;
        private MatchingPattern worldEntityPattern = new MatchingPattern(typeof(Transform),typeof(EntityRenderer));
        private MatchingPattern GUIEntityPattern = new MatchingPattern(typeof(GUIRect),typeof(GUISprite));
        private MatchingPattern TextEntityPattern = new MatchingPattern(typeof(GUIRect),typeof(GUIText));
        public SpriteAtlasSettings SpriteAtlasSettings { get { return atlasSettings; } set { atlasSettings = value; } }

        public Entity[] GetVisibleWorldEntities()
        {
            if (!matchingEntities.ContainsKey(worldEntityPattern)) return new Entity[0];
            if (matchingEntities[worldEntityPattern].Count <= 0) return new Entity[0];
            List<int> visibleEntitiesIDs = new List<int>();
            worldObjectsQuadTreeMainNode.Query(GetCameraFrustumRectangle(), visibleEntitiesIDs);
            var idSet = new HashSet<int>(visibleEntitiesIDs); // Use HashSet for efficient lookups
            Debug.WriteLine(visibleEntitiesIDs.Count);
            return matchingEntities[worldEntityPattern].Where(entity => idSet.Contains(entity.entityID)).ToArray();
        }

        public Entity[] GetGUIEntities()
        {
            if (!matchingEntities.ContainsKey(GUIEntityPattern)) return new Entity[0];
            if (matchingEntities[GUIEntityPattern].Count <= 0) return new Entity[0];
            return matchingEntities[GUIEntityPattern].ToArray();
        }

        public Entity[] GetTextEntities()
        {
            if (!matchingEntities.ContainsKey(TextEntityPattern)) return new Entity[0];
            if (matchingEntities[TextEntityPattern].Count <= 0) return new Entity[0];
            return matchingEntities[TextEntityPattern].ToArray();
        }

        public void SetRenderingSettings(SpriteAtlasSettings _atlasSettings)
        {
            atlasSettings = _atlasSettings;
        }
        public override void Initialize()
        {
            AddMatchingPatterns(worldEntityPattern, GUIEntityPattern, TextEntityPattern);
            CreateMainFrustumNode((2048*SpriteAtlasSettings.pixelsPerSprite)); //4096x4096 tiles, so we have to multiply by pixels to get actual size.
            base.Initialize();
        }

        private void CreateMainFrustumNode(int size)
        {
            int posX = size / 2;
            int posY = size / 2;
            worldObjectsQuadTreeMainNode = new FrustumQuadTreeNode(new Rectangle(-posX, -posY, size, size), 1, 8);
        }
        public override void OnEntityAttached(Entity newEntity,MatchingPattern pattern)
        {
            if(pattern.patternSignature == worldEntityPattern.patternSignature)
            {
                Transform transform = newEntity.GetComponent<Transform>();
                EntityRenderer rend = newEntity.GetComponent<EntityRenderer>();
                if(rend.renderRectangle == Rectangle.Empty)
                {
                    rend.renderRectangle = new Rectangle((int)transform.position.X, (int)transform.position.Y, rend.rectWidth, rend.rectHeight);
                }
                worldObjectsQuadTreeMainNode.Insert(rend.renderRectangle, newEntity.entityID);
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
            CheckForCameras();
        }

        public override void OnEntityDetached(Entity target)
        {

        }
        public override void GameUpdate()
        {
        }
        public override void OnEntityUpdate(Entity target, MatchingPattern pattern)
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

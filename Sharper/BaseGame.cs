using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sharper.Backend;
using Sharper.Components.GUI;
using Sharper.ECS;
using Sharper.Helpers;
using Sharper.Structures;
using Sharper.Systems.Backend;
using Sharper.Systems.Backend.GUI;
using Sharper.Systems.Backend.Management;
using Sharper.Systems.GameSystems;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;
namespace Sharper
{
    public class BaseGame : Game
    {
        protected GraphicsDeviceManager _graphics;
        protected SpriteBatch _spriteBatch;
        protected SceneManager _sceneManager = new SceneManager();
        protected RenderingSystem _renderingSystem = new RenderingSystem();
        protected InputSystem _inputSystem = new InputSystem();
        protected ResourceManager _resourceManager = new ResourceManager();
        protected GUIRenderingSystem _guiRenderingSystem = new GUIRenderingSystem();
        protected MouseEntityInteractionSystem _mouseInteractionSystem = new MouseEntityInteractionSystem();
        protected GUIEventSystem _guiEventSystem = new GUIEventSystem();
        protected SpriteFont _defaultFont;
        protected Vector2 viewportCenter;
        protected bool useTestScene = false;
        protected RenderTarget2D _guiRenderTarget;
        protected RenderTarget2D _textRenderTarget;
        //DEBUG VARS
        Texture2D debugMapTex;
        Scene testScene = new Scene();
        bool bpswitch = false;
        public BaseGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            //change window settings
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.ApplyChanges();
            //prepare test level
            //set sprite atlas settings
            _renderingSystem.SetRenderingSettings(new SpriteAtlasSettings(16, 2, 2));
            SceneManager.OnSceneLoad += AttachDerivedBackendSystems;
            SceneManager.OnSceneLoad += _renderingSystem.OnSceneChange;
            _renderingSystem._graphics = _graphics;
            base.Initialize();
        }

        //Derived systems are systems that work with entites of the current level.
        public void AttachDerivedBackendSystems(object sender, SceneEventArgs args)
        {
            args.argScene.systemManager.AttachSystem(_guiRenderingSystem);
            args.argScene.systemManager.AttachSystem(_mouseInteractionSystem);
            args.argScene.systemManager.AttachSystem(_renderingSystem);
            args.argScene.systemManager.AttachSystem(_guiEventSystem);
            Debug.WriteLine("[INFO]Attached derived backend systems.");
        }
        protected virtual void InitializeAfterContent()
        {
            if (!useTestScene) return;
            _sceneManager.LoadScene(testScene); //load level
            _sceneManager.GetCurrentScene().systemManager.AttachSystem(new SimpleMovement());
            SceneManager.CurrentScene.SpawnEntity(EntityHelper.CreateCameraMovable(System.Numerics.Vector3.One, 2f, true));
        }

        protected override void LoadContent()
        {
            //load all textures into Resource Manager
            DirectoryInfo texturesDirectory = new DirectoryInfo(Content.RootDirectory);
            FileInfo[] textureFiles = texturesDirectory.GetFiles("Textures\\");
            Debug.WriteLine($"[INFO]Trying to load {textureFiles.Length} textures into the resource manager...");
            foreach (FileInfo textureFile in textureFiles)
            {
                string pureFileName = Path.GetFileNameWithoutExtension(textureFile.Name);
                Texture2D loadedTexture = Content.Load<Texture2D>($"Textures\\{pureFileName}");
                _resourceManager.LoadTexture(pureFileName, loadedTexture);
            }
            _defaultFont = Content.Load<SpriteFont>("defaultFont");
            Debug.WriteLine($"[INFO] {textureFiles.Length} Textures loaded into resource manager successfully.");
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //load the sprite atlas texture
            _renderingSystem.textureAtlas = _resourceManager.GetTexture("atlas");
            InitializeAfterContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (_sceneManager.GetCurrentScene() == null) return;
            _sceneManager.GetCurrentScene().systemManager.UpdateEvent.Invoke(this, (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            var currentScene = _sceneManager.GetCurrentScene();
            if (currentScene == null) return;

            if (_renderingSystem.availableCameras.Length < 1 || !_renderingSystem.isInitialized)
            {
                Debug.WriteLine("[WARN] Camera not found for rendering.");
                _renderingSystem.CheckForCameras();
                return;
            }

            GraphicsDevice.SetRenderTarget(null);
            Texture2D hitboxOutlineTex = _resourceManager.GetTexture("GUIRectangleHR");
            Matrix viewMatrix = Matrix.CreateScale(_renderingSystem.currentCamera.cameraZoom);
            Transform cameraTransform = _renderingSystem.currentCameraTransform;
            viewMatrix *= Matrix.CreateTranslation(-cameraTransform.Position.X, -cameraTransform.Position.Y, 0);
            int pps = _renderingSystem.SpriteAtlasSettings.pixelsPerSprite;
            GraphicsDevice.Clear(Color.Black);
            //Render chunk borders.
            /*_spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: viewMatrix);
            Rectangle _gridSourceRect = new Rectangle(0, 0, pps, pps);
            foreach (Chunk chunk in currentScene.entityChunks)
            {
                if (chunk.nodeDepth == 0)
                {
                    Rectangle chunkRect = chunk.GetChunkRect();
                    _spriteBatch.Draw(hitboxOutlineTex, chunkRect, Color.Red);
                }
                else
                {
                    for (int i = chunk.nodeDepth; i >= 0; i--)
                    {
                        Color nodeDepthColor = i > 0 ? Color.Blue : Color.Red;
                        nodeDepthColor = i > 1 ? Color.Green : nodeDepthColor;
                        Chunk[] levelChunks = chunk.GetNodesAtLevel(i);
                        foreach (Chunk currChunk in levelChunks)
                        {
                            Rectangle chunkRect = currChunk.GetChunkRect();
                            _spriteBatch.Draw(hitboxOutlineTex, chunkRect, nodeDepthColor);
                        }
                    }
                }
            }
            _spriteBatch.End();*/
            // Frustum Culling with Chunks.
            Rectangle frustumRect = _renderingSystem.GetCameraFrustumRectangle();
            Chunk[] worldChunks = currentScene.entityChunks.ToArray();
            List<Entity> visibleEntities = new List<Entity>();
            if (_renderingSystem.RealEntityCount > 0)
            {
                foreach (Chunk chunk in worldChunks) //check surface chunks
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
            }
            //Debug.WriteLine($"[INFO]VIsible objects: {visibleEntities.Count}");
            // Render everything else with sprite renderer
            int pixelsPerSprite = _renderingSystem.SpriteAtlasSettings.pixelsPerSprite;
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: viewMatrix);
            foreach (var visibleEntity in visibleEntities)
            {
                Transform entityTransform = visibleEntity.GetComponent<Transform>();
                SpriteRenderer entityRenderer = visibleEntity.GetComponent<SpriteRenderer>();
                int col = entityRenderer.sprite.atlasX;
                int row = entityRenderer.sprite.atlasY;
                Vector3 entityPosition = entityTransform.position;
                Rectangle sourceRect = new Rectangle(pixelsPerSprite * col, pixelsPerSprite * row, pixelsPerSprite, pixelsPerSprite); // Cut a rect out of the texture atlas
                _spriteBatch.Draw(_renderingSystem.textureAtlas, new Vector2(entityPosition.X, entityPosition.Y), sourceRect, entityRenderer.spriteColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            _spriteBatch.End();

            if (_guiRenderingSystem.RealEntityCount > 0)
            {
                // Render GUI to separate render target instead of the backbuffer
                GraphicsDevice.SetRenderTarget(_guiRenderTarget);
                _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
                var guiMatchingEntities = _guiRenderingSystem.MatchingEntities;
                for (int i = 0; i < _guiRenderingSystem.RealEntityCount; i++)
                {
                    Entity currEntity = guiMatchingEntities[i];
                    GUIRect guiRect = currEntity.GetComponent<GUIRect>();
                    GUIRenderer guiRenderer = currEntity.GetComponent<GUIRenderer>();
                    if (guiRenderer.m_renderable)
                    {
                        Texture2D guiTexture = _resourceManager.GetTexture(guiRenderer.m_textureName);
                        _spriteBatch.Draw(guiTexture, guiRect.GetRect(), guiRenderer.m_color);
                    }
                    if (currEntity.HasComponent<GUIInputBox>())
                    {
                        string text = currEntity.GetComponent<GUIInputBox>().m_text;
                        _spriteBatch.DrawString(_defaultFont, text, guiRect.GetPosition() + Vector2.UnitX * 8 + Vector2.UnitY * 2, Color.WhiteSmoke);
                    }
                    if (currEntity.HasComponent<GUISprite>())
                    {
                        GUISprite sprite = currEntity.GetComponent<GUISprite>();
                        if(sprite.m_atlasX == -1)
                        {
                            Texture2D spriteTex = ResourceManager.Instance.GetTexture(sprite.m_textureName);
                            _spriteBatch.Draw(spriteTex, guiRect.GetRect(), new Rectangle(sprite.m_atlasX, sprite.m_atlasY, pps, pps), Color.White);
                        }
                        else
                        {
                            Rectangle sourceRect = new Rectangle(pixelsPerSprite * sprite.m_atlasX, pixelsPerSprite * sprite.m_atlasY, pixelsPerSprite, pixelsPerSprite);
                            _spriteBatch.Draw(_renderingSystem.textureAtlas, guiRect.GetRect(), sourceRect, Color.White);
                        }
                    }
                    if (currEntity.HasComponent<GUIText>())
                    {
                        string text = currEntity.GetComponent<GUIText>().m_text;
                        Color color = currEntity.GetComponent<GUIText>().m_color;
                        _spriteBatch.DrawString(_defaultFont, text, guiRect.GetPosition(), color);
                    }
                }
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}

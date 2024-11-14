﻿using Microsoft.Xna.Framework;
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
        protected MouseEntityInteractionSystem _mouseInteractionSystem = new MouseEntityInteractionSystem();
        protected GUIEventSystem _guiEventSystem = new GUIEventSystem();
        protected SpriteFont _defaultFont;
        protected Vector2 viewportCenter;
        protected bool useTestScene = false;
        protected RenderTarget2D _gridEntitiesRenderTarget;
        protected RenderTarget2D _worldEntitiesRenderTarget;
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
            //set m_sprite atlas settings
            _renderingSystem.SetRenderingSettings(new SpriteAtlasSettings(16, 2, 2));
            SceneManager.OnSceneLoad += AttachDerivedBackendSystems;
            SceneManager.OnSceneLoad += _renderingSystem.OnSceneChange;
            SceneManager.OnSceneLoad += OnSceneLoad;
            _renderingSystem._graphics = _graphics;
            base.Initialize();
        }

        //Derived systems are systems that work with entites of the current level.
        public void AttachDerivedBackendSystems(object sender, SceneEventArgs args)
        {
            args.argScene.systemManager.AttachSystem(_mouseInteractionSystem);
            args.argScene.systemManager.AttachSystem(_renderingSystem);
            args.argScene.systemManager.AttachSystem(_guiEventSystem);
            Debug.WriteLine("[INFO]Attached derived backend systems.");
        }
        protected virtual void OnSceneLoad(object sender, SceneEventArgs args)
        {

        }
        protected virtual void OnEngineReady()
        {

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
            //load the m_sprite atlas texture
            _renderingSystem.textureAtlas = _resourceManager.GetTexture("atlas");
            OnEngineReady();
        }

        protected override void Update(GameTime gameTime)
        {
            if (_sceneManager.GetCurrentScene() == null) return;
            _sceneManager.GetCurrentScene().systemManager.UpdateEvent.Invoke(this, (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            var currentScene =_sceneManager.GetCurrentScene();
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
            int pixelsPerSprite = _renderingSystem.SpriteAtlasSettings.pixelsPerSprite;
            GraphicsDevice.SetRenderTarget(_gridEntitiesRenderTarget);
            _spriteBatch.Begin(samplerState:SamplerState.PointClamp, transformMatrix: viewMatrix);
                foreach(Entity x in _renderingSystem.GridEntities)
                {
                    Transform entityTransform = x.GetComponent<Transform>();
                    EntityRenderer entityRenderer = x.GetComponent<EntityRenderer>();
                    int row = entityRenderer.m_sprite.m_atlasX;
                    int col = entityRenderer.m_sprite.m_atlasY;
                    Color entityColor = entityRenderer.m_sprite.m_color; 
                    Rectangle sourceRect = new Rectangle(pixelsPerSprite * col, pixelsPerSprite * row, pixelsPerSprite, pixelsPerSprite);
                _spriteBatch.Draw(_renderingSystem.textureAtlas, new Vector2(entityTransform.position.X, entityTransform.position.Y),sourceRect,entityColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                }
            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(_worldEntitiesRenderTarget);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: viewMatrix);
            foreach (Entity x in _renderingSystem.WorldEntities)
            {
                Transform entityTransform = x.GetComponent<Transform>();
                EntityRenderer entityRenderer = x.GetComponent<EntityRenderer>();
                int row = entityRenderer.m_sprite.m_atlasX;
                int col = entityRenderer.m_sprite.m_atlasY;
                Color entityColor = entityRenderer.m_sprite.m_color;
                Rectangle sourceRect = new Rectangle(pixelsPerSprite * col, pixelsPerSprite * row, pixelsPerSprite, pixelsPerSprite);
                _spriteBatch.Draw(_renderingSystem.textureAtlas, new Vector2(entityTransform.position.X, entityTransform.position.Y), sourceRect, entityColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(_guiRenderTarget);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
            foreach(Entity x in _renderingSystem.GUIEntities)
            {
                GUIRect rect = x.GetComponent<GUIRect>();
                Sprite sprite = x.GetComponent<EntityRenderer>().m_sprite;
                if (sprite.m_atlasX == -1)
                {
                    Texture2D spriteTex = ResourceManager.Instance.GetTexture(sprite.m_textureName);
                    _spriteBatch.Draw(spriteTex, rect.GetRect(), new Rectangle(sprite.m_atlasX, sprite.m_atlasY, pps, pps), Color.White);
                }
                else
                {
                    Rectangle sourceRect = new Rectangle(pixelsPerSprite * sprite.m_atlasX, pixelsPerSprite * sprite.m_atlasY, pixelsPerSprite, pixelsPerSprite);
                    _spriteBatch.Draw(_renderingSystem.textureAtlas, rect.GetRect(), sourceRect, Color.White);
                }
            }
            _spriteBatch.End();
            GraphicsDevice.SetRenderTarget(_textRenderTarget);
            _spriteBatch.Begin();
            foreach(Entity x in _renderingSystem.TextEntities)
            {
                GUIRect textRect = x.GetComponent<GUIRect>();
                string text = x.GetComponent<EntityRenderer>().textToRender.m_text;
                Color color = x.GetComponent<EntityRenderer>().textToRender.m_color;
                _spriteBatch.DrawString(_defaultFont, text, textRect.GetPosition(), color);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

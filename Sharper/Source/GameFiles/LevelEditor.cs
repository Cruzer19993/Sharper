using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sharper.Backend.Standalone;
using Sharper.Components;
using Sharper.Components.GUI;
using Sharper.ECS;
using Sharper.Helpers;
using Sharper.Source.Helpers;
using Sharper.Structures;
using Sharper.Systems.Backend;
using Sharper.Systems.Backend.Management;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Sharper.GameFiles
{
    public class LevelEditor : BaseGame
    {
        public LevelEditor()
        {
            currentEditorGridEntites = new List<Entity>();
        }
        Stopwatch fpsSamplingStopwatch = new Stopwatch();
        Scene editorScene = new Scene();
        GUIButton generateWorldButton;
        GUIText worldSizeXInputBox;
        GUIText worldSizeYInputBox;
        GUIText FpsCounterText;
        GUILayout worldGenerationLayoutGroup;
        List<Entity> currentEditorGridEntites;
        GUILayout leftSideLayoutGroup;
        EntityRenderer selectedTileRenderer;
        int selectedTextureAtlasX, selectedTextureAtlasY;
        static readonly Type[] EditorTileGridComponentTypes = { typeof(Transform), typeof(MouseInteractable), typeof(EntityRenderer) };
        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void OnSceneLoad(object sender, SceneEventArgs args)
        {
            PrepareEditor();
        }
        protected override void OnEngineReady()
        {
            editorScene.useWorldLoader = false;
            _sceneManager.LoadScene(editorScene);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!fpsSamplingStopwatch.IsRunning)
            {
                fpsSamplingStopwatch.Start();
            }
            if (fpsSamplingStopwatch.Elapsed.TotalSeconds >= 1)
            {
                FpsCounterText.m_text = $"FPS: {(1 / gameTime.ElapsedGameTime.TotalSeconds).ToString("0")}";
                fpsSamplingStopwatch.Reset();
            }

            if (InputSystem.ButtonDown(MouseButton.Left))
            {
                PaintEntity();
            }
        }
        void PrepareEditor()
        {
            //Spawn a camera.
            editorScene.systemManager.AttachSystem(new LevelEditorCameraMover());
            editorScene.SpawnEntity(EntityHelper.CreateCameraMovable(Vector3.Zero, 1, true));
            FpsCounterText = GUIHelper.CreateText(new Vector2(600, 0), "999", Color.WhiteSmoke).GetComponent<GUIText>();
            GUIHelper.CreateGUILayout(Vector2.UnitY * 30f, Vector2.UnitX * 4, Vector2.Zero, out leftSideLayoutGroup);
            CreateWorldGenerationUI();
            CreateTileEditorUI();
            CreateLayerEditorUI();
        }

        void RemoveCurrentGridEntities()
        {
            if (currentEditorGridEntites == null) return;
            foreach (Entity tile in currentEditorGridEntites)
            {
                _sceneManager.GetCurrentScene().entityManager.ReleaseEntity(tile.entityID);
                _sceneManager.GetCurrentScene().entityManager.RemoveEntity(tile.entityID);
            }
            currentEditorGridEntites.Clear();
        }

        void CreateTileGrid(int x, int y)
        {
            if (currentEditorGridEntites == null) currentEditorGridEntites = new List<Entity>(x * y);
            else currentEditorGridEntites.Clear();
            int pps = _renderingSystem.SpriteAtlasSettings.pixelsPerSprite;
            int linearIndex = x * y;
            for (int i = 0; i < linearIndex; i++)
            {
                int posx = i % x;
                int posy = (int)(i / x);
                Entity tile = EntityHelper.CreateGridTile(new Vector3(posx * pps, posy * pps, 0f));
                editorScene.SpawnEntity(tile);
            }
        }
        void CreateLayerEditorUI()
        {

        }
        void CreateTileEditorUI()
        {
            GUIHelper.CreateGUILayout(Vector2.UnitY * 8f, Vector2.UnitX * 4, Vector2.Zero, out GUILayout tileEditorLayoutGroup);
            GUIHelper.CreateGUIGrid(Vector2.One * 8f, Vector2.UnitX * 4, Vector2.UnitX * 4f, Vector2.One * 32f, 3, true, out GUILayout tileEditorGrid);
            GUILayoutManager.AddToLayoutGroup(ref tileEditorLayoutGroup, tileEditorGrid.owner.GetComponent<GUIRect>());
            GUILayoutManager.AddToLayoutGroup(ref leftSideLayoutGroup, tileEditorLayoutGroup.owner.GetComponent<GUIRect>());
            SpriteAtlasSettings atlasSettings = _renderingSystem.SpriteAtlasSettings;
            Texture2D atlasTexture = ResourceManager.Instance.GetTexture("atlas");
            int textureWidth = atlasTexture.Width;
            int textureHeight = atlasTexture.Height;
            for (int x = 0; x < textureWidth; x += atlasSettings.pixelsPerSprite)
            {
                for (int y = 0; y < textureHeight; y += atlasSettings.pixelsPerSprite)
                {
                    GUILayoutManager.AddToLayoutGroup(ref tileEditorGrid, GUIHelper.CreateSpriteButton(Vector2.Zero, out EntityRenderer rend, out GUIButton btn).GetComponent<GUIRect>());
                    int localSpriteX = (int)x / atlasSettings.pixelsPerSprite;
                    int localSpriteY = (int)y / atlasSettings.pixelsPerSprite;
                    rend.m_sprite.m_atlasX = localSpriteX;
                    rend.m_sprite.m_atlasY = localSpriteY;
                    btn.OnClick += delegate { SelectTexture(localSpriteX, localSpriteY); };
                }
            }
            GUIHelper.CreateGUILayout(Vector2.UnitX * 20f, Vector2.Zero, Vector2.Zero, out var selectedTextureDisplayHLG);
            GUILayoutManager.AddToLayoutGroup(ref selectedTextureDisplayHLG, GUIHelper.CreateText(Vector2.Zero, "Selected Texture", Color.White).GetComponent<GUIRect>());
            GUILayoutManager.AddToLayoutGroup(ref selectedTextureDisplayHLG, GUIHelper.CreateSpriteButton(Vector2.Zero,out selectedTileRenderer, out GUIButton btn20).GetComponent<GUIRect>());
            GUILayoutManager.AddToLayoutGroup(ref tileEditorLayoutGroup, selectedTextureDisplayHLG.owner.GetComponent<GUIRect>());
        }

        void CreateWorldGenerationUI()
        {
            GUIHelper.CreateGUILayout(Vector2.UnitY * 8f, Vector2.UnitX * 4, Vector2.Zero, out worldGenerationLayoutGroup);
            GUILayoutManager.AddToLayoutGroup(ref leftSideLayoutGroup, worldGenerationLayoutGroup.owner.GetComponent<GUIRect>());
            GUILayoutManager.AddToLayoutGroup(ref worldGenerationLayoutGroup, GUIHelper.CreateText(Vector2.Zero, "World Grid Settings", Color.White).GetComponent<GUIRect>());
            //Width input box
            GUIHelper.CreateGUILayout(Vector2.UnitX * 8f, Vector2.Zero, Vector2.Zero, out var inputBoxWorldWidthHLG);
            GUILayoutManager.AddToLayoutGroup(ref inputBoxWorldWidthHLG, GUIHelper.CreateText(Vector2.Zero, "Grid Width", Color.WhiteSmoke).GetComponent<GUIRect>());
            GUILayoutManager.AddToLayoutGroup(ref inputBoxWorldWidthHLG, GUIHelper.CreateInputBox(Vector2.Zero, new Vector2(40, 20), out worldSizeXInputBox).GetComponent<GUIRect>());
            GUILayoutManager.AddToLayoutGroup(ref worldGenerationLayoutGroup, inputBoxWorldWidthHLG.owner.GetComponent<GUIRect>());
            //Height input box
            GUIHelper.CreateGUILayout(Vector2.UnitX * 8f, Vector2.Zero, Vector2.Zero, out var inputBoxWorldHeightHLG);
            GUILayoutManager.AddToLayoutGroup(ref inputBoxWorldHeightHLG, GUIHelper.CreateText(Vector2.Zero, "Grid Height", Color.WhiteSmoke).GetComponent<GUIRect>());
            GUILayoutManager.AddToLayoutGroup(ref inputBoxWorldHeightHLG, GUIHelper.CreateInputBox(Vector2.Zero, new Vector2(40, 20), out worldSizeYInputBox).GetComponent<GUIRect>());
            GUILayoutManager.AddToLayoutGroup(ref worldGenerationLayoutGroup, inputBoxWorldHeightHLG.owner.GetComponent<GUIRect>());
            //Generate button
            GUILayoutManager.AddToLayoutGroup(ref worldGenerationLayoutGroup, GUIHelper.CreateButton(Vector2.Zero, new Vector2(150, 20), "Generate New Grid", out generateWorldButton).GetComponent<GUIRect>());
            generateWorldButton.OnClick += GenerateNewGridButton;
        }

        void SelectTexture(int x, int y)
        {
            selectedTileRenderer.m_sprite.m_atlasX = x;
            selectedTileRenderer.m_sprite.m_atlasY = y;
        }

        void PaintEntity()
        {
            Entity entityUnderCursor = _mouseInteractionSystem.GetEntityUnderCursor();
            if (entityUnderCursor == null) return;
            if(entityUnderCursor.GetComponent<EntityRenderer>() != null)
            {
                EntityRenderer renderer = entityUnderCursor.GetComponent<EntityRenderer>();
                renderer.m_sprite.m_atlasX = selectedTileRenderer.m_sprite.m_atlasX;
                renderer.m_sprite.m_atlasY = selectedTileRenderer.m_sprite.m_atlasY;
            }
        }

        void GenerateNewGridButton(object sender, EventArgs args)
        {
            int gridWidth, gridHeight;
            if (!int.TryParse(worldSizeXInputBox.m_text, out gridWidth))
            {
                Debug.WriteLine("Couldn't parse grid width as int");
                return;
            }
            if (!int.TryParse(worldSizeXInputBox.m_text, out gridHeight))
            {
                Debug.WriteLine("Couldn't parse grid height as int");
                return;
            }
            Stopwatch workTimer = new Stopwatch();
            workTimer.Start();
            Debug.WriteLine("[INFO]Generating a new world grid...");
            RemoveCurrentGridEntities();
            CreateTileGrid(gridWidth, gridHeight);
            workTimer.Stop();
            Debug.WriteLine($"[INFO]Generated a new world grid in {workTimer.Elapsed.TotalSeconds}s");
        }

        public static Entity CreateGridTile(Vector3 position)
        {
            Entity gridTile = new Entity("Grid Tile", EditorTileGridComponentTypes);
            Transform transform = gridTile.GetComponent<Transform>();
            EntityRenderer spriteRenderer = gridTile.GetComponent<EntityRenderer>();

            // Directly set the properties to avoid multiple calls to GetComponent
            transform.position = position;
            spriteRenderer.m_sprite.m_atlasX = 0;
            spriteRenderer.m_sprite.m_atlasY = 1;
            spriteRenderer.m_sprite.m_color = Color.White;

            return gridTile;
        }
    }
}

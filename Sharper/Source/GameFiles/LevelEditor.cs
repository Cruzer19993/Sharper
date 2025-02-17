using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sharper.Systems.Backend.Standalone;
using Sharper.Components;
using Sharper.Components.GUI;
using Sharper.ECS;
using Sharper.Helpers;
using Sharper.Source.Structures;
using Sharper.Structures;
using Sharper.Systems.Backend;
using Sharper.Systems.Backend.Management;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sharper.Source.Helpers;
namespace Sharper.GameFiles
{
    public class LevelEditor : BaseGame
    {
        Stopwatch fpsSamplingStopwatch = new Stopwatch();
        Scene editorScene = new Scene();
        GUILayout leftSideLayoutGroup;
        GUIRect leftSideLayoutRect;
        GUIText mapSizeText;
        Map currentMap;
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
                //FpsCounterText.m_text = $"FPS: {(1 / gameTime.ElapsedGameTime.TotalSeconds).ToString("0")}";
                fpsSamplingStopwatch.Reset();
            }
        }
        void PrepareEditor()
        {
            //Spawn a camera.
            editorScene.systemManager.AttachSystem(new LevelEditorCameraMover());
            editorScene.SpawnEntity(EntityHelper.CreateCameraMovable(Vector3.Zero, 1, true));
            GUIHelper.CreateLayout(new Vector2(600, 1), Vector2.UnitY * 8, out leftSideLayoutGroup, new GUILayoutOptions[] { GUILayoutOptions.CONTENT_CENTER_HORIZONTAL,GUILayoutOptions.STRETCH_HEIGHT});
            CreateMapGenerationUI();
        }

        void CreateMapGenerationUI()
        {
            GUILayoutManager.AddContent(leftSideLayoutGroup, GUIHelper.CreateText("World Generation Settings").GetComponent<GUIRect>());
            GUILayoutManager.AddContent(leftSideLayoutGroup, GUIHelper.CreateText("World Generation Settings 2").GetComponent<GUIRect>());
            GUIHelper.CreateLayout(new Vector2(302, 22), Vector2.UnitX * 4, out GUILayout testLayout, new GUILayoutOptions[] { GUILayoutOptions.CONTENT_CENTER});
            GUILayoutManager.RootLayout(leftSideLayoutGroup,testLayout);
            GUILayoutManager.AddContent(testLayout, GUIHelper.CreateText("Text 1").GetComponent<GUIRect>());
            GUILayoutManager.AddContent(testLayout, GUIHelper.CreateText("Text 2").GetComponent<GUIRect>());
            GUILayoutManager.AddContent(leftSideLayoutGroup, GUIHelper.CreateImage(Vector2.Zero, new Sprite(1, 1), out GUISprite sprite).GetComponent<GUIRect>());
            GUIHelper.CreateInputBox("12345678", out GUIInputBox ib1).GetComponent<GUIRect>();
            GUILayoutManager.RootLayout(leftSideLayoutGroup,ib1.owner.GetComponent<GUILayout>());
            GUILayoutManager.AddContent(leftSideLayoutGroup, ib1.owner.GetComponent<GUIRect>());
            
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

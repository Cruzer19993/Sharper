using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sharper.Backend.Standalone;
using Sharper.Components;
using Sharper.Components.GUI;
using Sharper.ECS;
using Sharper.Helpers;
using Sharper.Source.Helpers;
using Sharper.Source.Structures;
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
        Stopwatch fpsSamplingStopwatch = new Stopwatch();
        Scene editorScene = new Scene();
        GUILayout leftSideLayoutGroup;
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
            GUIHelper.CreateGUILayout(new Vector2(300, 200), Vector2.UnitY * 8,Vector2.Zero, Vector2.Zero, out leftSideLayoutGroup,GUILayoutOptions.STRETCH_HEIGHT,GUILayoutOptions.CONTENT_CENTER_HORIZONTAL);
            CreateMapGenerationUI();
        }

        void CreateMapGenerationUI()
        {
            GUILayoutManager.AddToLayoutGroup(ref leftSideLayoutGroup, GUIHelper.CreateText(Vector2.Zero, "Map Generation Settings",Color.White,out GUIText mgt).GetComponent<GUIRect>());
            GUILayoutManager.AddToLayoutGroup(ref leftSideLayoutGroup, GUIHelper.CreateGUILayout(Vector2.Zero, Vector2.UnitX*8, Vector2.Zero, Vector2.Zero, out GUILayout InputBoxLayout,GUILayoutOptions.CONTENT_CENTER_VERTICAL).GetComponent<GUIRect>());
            GUILayoutManager.AddToLayoutGroup(ref InputBoxLayout, GUIHelper.CreateText(Vector2.Zero, "Map Size", Color.Black, out GUIText mst).GetComponent<GUIRect>());
            GUILayoutManager.AddToLayoutGroup(ref InputBoxLayout, GUIHelper.CreateInputBox(Vector2.Zero,new Vector2(80,20),out mapSizeText,out GUIInputBox sip,"0").GetComponent<GUIRect>());
            GUILayoutManager.UpdateGUILayout(InputBoxLayout);
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

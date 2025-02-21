using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sharper.ECS;
using Microsoft.Xna.Framework;
using Sharper;
using Sharper.Structures;
using Sharper.Components;
using Sharper.Helpers;
using Sharper.Systems.Backend.Management;
using Sharper.Components.GUI;
using Sharper.Source.Helpers;
using Sharper.Systems.Backend.Standalone;
using System.Diagnostics;
using Gamespace;
namespace GameSpace
{
    public class MainGame : BaseGame
    {
        public Scene mainScene = new Scene();
        public Camera mainCamera = new Camera();

        //DEBUG VARS
        GUILayout leftSideLayout;
        GUIText noiseSizeText;
        GUIText noiseScaleText;
        GUISprite noiseTex;
        protected override void OnEngineReady()
        {
            base.OnEngineReady();
            _sceneManager.LoadScene(mainScene);
            mainScene.systemManager.AttachSystem(new LevelEditorCameraMover());
            mainCamera = EntityHelper.CreateCameraMovable(Vector3.Zero, 1, true).GetComponent<Camera>();
            mainCamera.m_backbufferColor = Color.Green;
        }
        protected override void OnSceneLoad(object sender, SceneEventArgs args)
        {
            CreateDebugUI();
        }

        void CreateDebugUI()
        {
            GUIHelper.CreateLayout(new Vector2(600, 1), Vector2.UnitY * 8, out leftSideLayout, new GUILayoutOptions[] { GUILayoutOptions.STRETCH_HEIGHT, GUILayoutOptions.CONTENT_CENTER_HORIZONTAL });
            GUIHelper.CreateButton("GENERATE MAP", out GUIButton btn);
            GUILayoutManager.RootLayout(leftSideLayout, btn.owner.GetComponent<GUILayout>());
            GUILayoutManager.AddContent(leftSideLayout, btn.owner.GetComponent<GUIRect>());
            GUIHelper.CreateInputBox("1024", out GUIInputBox ib);
            noiseSizeText = ib.m_inputBoxTextDisplay;
            GUILayoutManager.RootLayout(leftSideLayout,ib.owner.GetComponent<GUILayout>());
            GUILayoutManager.AddContent(leftSideLayout,ib.owner.GetComponent<GUIRect>());
            GUIHelper.CreateInputBox("0,097", out GUIInputBox ib2);
            noiseScaleText = ib2.m_inputBoxTextDisplay;
            GUILayoutManager.RootLayout(leftSideLayout, ib2.owner.GetComponent<GUILayout>());
            GUILayoutManager.AddContent(leftSideLayout, ib2.owner.GetComponent<GUIRect>());
            GUIHelper.CreateImage(Vector2.Zero, new Sprite("GUIRectangleHR", Color.White), out noiseTex);
            noiseTex.owner.GetComponent<GUIRect>().m_size = Vector2.One * 512;
            GUILayoutManager.AddContent(leftSideLayout,noiseTex.owner.GetComponent<GUIRect>());
            noiseTex.m_uiSprite.m_color = Color.White;
            btn.buttonClicked += GenerateNoiseTextureOnClick;
        }

        void GenerateNoiseTextureOnClick(object sender, EventArgs e)
        {
            if (int.TryParse(noiseSizeText.Text, out int result))
            {
                if (float.TryParse(noiseScaleText.Text, out float scaleResult))
                {
                    noiseTex.m_uiSprite.m_ownTexture = MapGenerator.PerlinNoiseTexture(result, scaleResult);
                    Debug.WriteLine("Generated a noise texture");
                }
                else Debug.WriteLine("Couldn't parse input box");
            }
            else Debug.WriteLine("Couldn't parse inputbox");
        }

    }
}

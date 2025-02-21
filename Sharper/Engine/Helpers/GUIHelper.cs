using Microsoft.Xna.Framework;
using Sharper.Components.GUI;
using Sharper.ECS;
using Sharper.Systems.Backend.Management;
using Sharper.Systems.Backend.Standalone;
using System;
namespace Sharper.Source.Helpers
{
    public static class GUIHelper
    {
        public static Entity CreateLayout(Vector2 layoutSize, Vector2 contentSpacing, out GUILayout layout, params GUILayoutOptions[] options)
        {
            Entity layoutEntity = new Entity("GUILayout", new Type[] { typeof(GUIRect), typeof(GUILayout) });
            layoutEntity.GetComponent<GUILayout>().m_spacing = contentSpacing;
            layoutEntity.GetComponent<GUILayout>().m_options = options;
            layoutEntity.GetComponent<GUIRect>().m_size = layoutSize;
            layout = layoutEntity.GetComponent<GUILayout>();
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(layoutEntity);
            }
            return layoutEntity;
        }
        public static Entity CreateLayout(Vector2 layoutSize, Vector2 contentSpacing,Vector2 position ,out GUILayout layout, params GUILayoutOptions[] options)
        {
            Entity layoutEntity = new Entity("GUILayout", new Type[] { typeof(GUIRect), typeof(GUILayout) });
            layoutEntity.GetComponent<GUILayout>().m_spacing = contentSpacing;
            layoutEntity.GetComponent<GUILayout>().m_options = options;
            layoutEntity.GetComponent<GUIRect>().m_size = layoutSize;
            layoutEntity.GetComponent<GUIRect>().m_position = position;
            layout = layoutEntity.GetComponent<GUILayout>();
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(layoutEntity);
            }
            return layoutEntity;
        }
        public static Entity CreateLayout(Vector2 layoutSize, Vector2 contentSpacing,Vector2 position, Vector2 padding ,out GUILayout layout)
        {
            Entity layoutEntity = new Entity("GUILayout", new Type[] { typeof(GUIRect), typeof(GUILayout) });
            layoutEntity.GetComponent<GUILayout>().m_spacing = contentSpacing;
            layoutEntity.GetComponent<GUILayout>().m_padding = padding;
            layoutEntity.GetComponent<GUIRect>().m_size = layoutSize;
            layoutEntity.GetComponent<GUIRect>().m_position=position;
            layout = layoutEntity.GetComponent<GUILayout>();
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(layoutEntity);
            }
            return layoutEntity;
        }

        public static Entity CreateText(string initialText, out GUIRect textRect)
        {
            Entity textEntity = new Entity("Text", new Type[] { typeof(GUIRect), typeof(GUIText) });
            textEntity.GetComponent<GUIText>().Text = initialText;
            textRect = textEntity.GetComponent<GUIRect>();
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(textEntity);
            }
            return textEntity;
        }
        public static Entity CreateText(string initialText)
        {
            Entity textEntity = new Entity("Text", new Type[] { typeof(GUIRect), typeof(GUIText) });
            textEntity.GetComponent<GUIText>().Text= initialText;
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(textEntity);
            }
            return textEntity;
        }
        public static Entity CreateText(string initialText,Vector2 rectSize)
        {
            Entity textEntity = new Entity("Text", new Type[] { typeof(GUIRect), typeof(GUIText) });
            textEntity.GetComponent<GUIText>().Text = initialText;
            textEntity.GetComponent<GUIRect>().m_size = rectSize;
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(textEntity);
            }
            return textEntity;
        }
        public static Entity CreateImage(Vector2 position, Sprite imageSprite, out GUISprite image)
        {
            Entity imageEntity = new Entity("Image", new Type[] { typeof(GUISprite), typeof(GUIRect) });
            imageEntity.GetComponent<GUIRect>().m_position = position;
            imageEntity.GetComponent<GUISprite>().m_uiSprite = imageSprite;
            image = imageEntity.GetComponent<GUISprite>();
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(imageEntity);
            }
            return imageEntity;
        }

        public static Entity CreateInputBox(string initialValue,out GUIInputBox inputBox)
        {
            Entity inputBoxEntity = new Entity("UIInputBox", new Type[] { typeof(GUIRect), typeof(GUISprite), typeof(GUIInputBox),typeof(GUILayout)});
            GUILayout ibLayout = inputBoxEntity.GetComponent<GUILayout>();
            ibLayout.m_options = new GUILayoutOptions[] {GUILayoutOptions.CONTENT_CENTER };
            inputBoxEntity.GetComponent<GUISprite>().m_uiSprite = new Sprite("GUIRectangle", Color.White);
            Entity inputBoxTextEntity = new Entity("UIInputBoxText", new Type[] { typeof(GUIRect), typeof(GUIText) });
            inputBoxTextEntity.GetComponent<GUIText>().Text = initialValue;
            inputBoxTextEntity.GetComponent<GUIText>().m_color = Color.Black;
            GUIInputBox ibcomp = inputBoxEntity.GetComponent<GUIInputBox>();
            ibcomp.m_inputBoxTextDisplay = inputBoxTextEntity.GetComponent<GUIText>();
            inputBoxEntity.GetComponent<GUIRect>().m_size = inputBoxTextEntity.GetComponent<GUIRect>().m_size+Vector2.One*4+Vector2.UnitX*12;
            inputBox = ibcomp;
            GUILayoutManager.AddContent(ibLayout, inputBoxTextEntity.GetComponent<GUIRect>());
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(inputBoxEntity);
                SceneManager.CurrentScene.SpawnEntity(inputBoxTextEntity);
            }
            return inputBoxEntity;
        }

        public static Entity CreateButton(string buttonName,out GUIButton button)
        {
            Entity buttonEntity = new Entity("UIButton", new Type[] { typeof(GUIRect), typeof(GUISprite), typeof(GUIButton), typeof(GUILayout) });
            Entity buttonTextEntity = new Entity("UIButtonText", new Type[] { typeof(GUIRect), typeof(GUIText) });
            GUILayout btnLayout = buttonEntity.GetComponent<GUILayout>();
            btnLayout.m_options = new GUILayoutOptions[] { GUILayoutOptions.CONTENT_CENTER};
            buttonEntity.GetComponent<GUISprite>().m_uiSprite = new Sprite("GUIDefaultTexture", Color.White);
            buttonTextEntity.GetComponent<GUIText>().Text = buttonName;
            buttonTextEntity.GetComponent<GUIText>().m_color = Color.Black;
            button = buttonEntity.GetComponent<GUIButton>();
            buttonEntity.GetComponent<GUIRect>().m_size = buttonTextEntity.GetComponent<GUIRect>().m_size + Vector2.UnitX*20f + Vector2.UnitY*4f;
            GUILayoutManager.AddContent(btnLayout, buttonTextEntity.GetComponent<GUIRect>());
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(buttonEntity);
                SceneManager.CurrentScene.SpawnEntity(buttonTextEntity);
            }
            return buttonEntity;
        }
    }
}

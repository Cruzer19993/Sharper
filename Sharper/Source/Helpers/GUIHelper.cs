using Microsoft.Xna.Framework;
using Sharper.Components.GUI;
using Sharper.ECS;
using Sharper.Systems.Backend.Management;
using System;
namespace Sharper.Source.Helpers
{
    public static class GUIHelper
    {
        public static Entity CreateLayout(Vector2 layoutSize, Vector2 contentSpacing, out GUILayout layout, params GUILayoutOptions[] options)
        {
            Entity layoutEntity = new Entity("GUILayout", new Type[] { typeof(GUIRect), typeof(GUILayout), typeof(GUISprite) });
            layoutEntity.GetComponent<GUILayout>().m_spacing = contentSpacing;
            layoutEntity.GetComponent<GUILayout>().m_options = options;
            layoutEntity.GetComponent<GUIRect>().m_size = layoutSize;
            layoutEntity.GetComponent<GUISprite>().m_uiSprite.m_textureName = "GUIRectangleHR";
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
    }
}

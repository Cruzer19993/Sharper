﻿using Sharper.Components;
using Sharper.Components.GUI;
using Sharper.ECS;
using System;
using Microsoft.Xna.Framework;
using Sharper.Systems.Backend.Management;
namespace Sharper.Source.Helpers
{
    public class GUIHelper
    {
        public static Entity CreateButton(Vector2 position, Vector2 size, string buttonText, out GUIButton buttonComponent, bool renderHitbox = false)
        {
            Entity buttonEntity = new Entity("Button", new Type[] { typeof(GUIRect), typeof(GUIButton), typeof(EntityRenderer), typeof(GUIText)});
            GUIRect btnRect = buttonEntity.GetComponent<GUIRect>();
            btnRect.SetPosition(position);
            btnRect.SetSize(size);
            buttonEntity.GetComponent<GUIText>().m_text = buttonText;
            buttonEntity.GetComponent<GUIText>().m_color = Color.Black;
            buttonEntity.GetComponent<EntityRenderer>().m_sprite.m_textureName = "GUIDefaultTexture";
            buttonComponent = buttonEntity.GetComponent<GUIButton>();
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(buttonEntity);
            }
            return buttonEntity;
        } 

        public static Entity CreateInputBox(Vector2 position, Vector2 size, out GUIText text,string startingText = "")
        {
            Entity inputBoxEntity = new Entity("InputBox", new Type[] { typeof(GUIRect), typeof(EntityRenderer), typeof(GUIInputBox), typeof(GUIText) });
            GUIRect ibRect = inputBoxEntity.GetComponent<GUIRect>();
            GUIText ibText = inputBoxEntity.GetComponent<GUIText>();
            ibRect.SetPosition(position);
            ibRect.SetSize(size);
            ibText.m_color = Color.Black;
            ibText.m_text = startingText;
            text = ibText;
            inputBoxEntity.GetComponent<EntityRenderer>().m_sprite.m_textureName = "GUIRectangle";
            inputBoxEntity.GetComponent<EntityRenderer>().m_sprite.m_color = Color.WhiteSmoke;
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(inputBoxEntity);
            }
            return inputBoxEntity;

        }

        public static Entity CreateUIPanel(Vector2 position, Vector2 size, Color color)
        {
            Entity panelEntity = new Entity("UIPanel", new Type[] {typeof(GUIRect),typeof(EntityRenderer) });
            panelEntity.GetComponent<GUIRect>().SetPosition(position);
            panelEntity.GetComponent<GUIRect>().SetSize(size);
            panelEntity.GetComponent<EntityRenderer>().m_sprite.m_color = color;
            panelEntity.GetComponent<EntityRenderer>().m_sprite.m_textureName = "GUIDefaultTexture";
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(panelEntity);
            }
            return panelEntity;
        }

        public static Entity CreateText(Vector2 position, string text, Color textColor)
        {
            Entity textEntity = new Entity("Text", new Type[] {typeof(GUIRect),typeof(GUIText) });
            textEntity.GetComponent<GUIRect>().SetPosition(position);
            textEntity.GetComponent<GUIRect>().SetSize(new Vector2(100,20));
            textEntity.GetComponent<GUIText>().m_text = text;
            if(SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(textEntity);
            }
            return textEntity;
        }

        public static Entity CreateSpriteButton(Vector2 position, out EntityRenderer spriteComponent, out GUIButton buttonComponent, string textureName="")
        {
            Entity buttonEntity = new Entity("Button", new Type[] { typeof(GUIRect), typeof(GUIButton), typeof(EntityRenderer) });
            GUIRect btnRect = buttonEntity.GetComponent<GUIRect>();
            btnRect.SetPosition(position);
            Vector2 texSize = new Vector2(32, 32);
            btnRect.SetSize(texSize);
            buttonComponent = buttonEntity.GetComponent<GUIButton>();
            buttonEntity.GetComponent<EntityRenderer>().m_sprite.m_textureName = textureName;
            spriteComponent = buttonEntity.GetComponent<EntityRenderer>();
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(buttonEntity);
            }
            return buttonEntity;
        }

        public static  Entity CreateGUILayout(Vector2 spacing, Vector2 padding, Vector2 offset, out GUILayout newLayoutGroup)
        {
            Entity layoutGroupEntity = new Entity("HorizontalLayoutGroup", new Type[] { typeof(GUILayout), typeof(GUIRect) });
            newLayoutGroup = layoutGroupEntity.GetComponent<GUILayout>();
            newLayoutGroup.m_spacing = spacing;
            newLayoutGroup.m_padding = padding;
            newLayoutGroup.m_offset = offset;
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(layoutGroupEntity);
            }
            return layoutGroupEntity;
        }

        public static Entity CreateGUIGrid(Vector2 spacing, Vector2 padding, Vector2 offset, Vector2 cellSize,int numberOfColumns ,bool enforceCellSize, out GUILayout newLayoutGroup)
        {
            Entity layoutGroupEntity = new Entity("HorizontalLayoutGroup", new Type[] { typeof(GUIGrid), typeof(GUIRect) });
            newLayoutGroup = layoutGroupEntity.GetComponent<GUIGrid>();
            GUIGrid newLayoutGroupGrid = (GUIGrid)newLayoutGroup;
            newLayoutGroupGrid.m_spacing = spacing;
            newLayoutGroupGrid.m_padding = padding;
            newLayoutGroupGrid.m_offset = offset;
            newLayoutGroupGrid.m_cellSize = cellSize;
            newLayoutGroupGrid.m_enforceCellSize = enforceCellSize;
            newLayoutGroupGrid.m_columns = numberOfColumns;
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(layoutGroupEntity);
            }
            return layoutGroupEntity;
        }
    }
}

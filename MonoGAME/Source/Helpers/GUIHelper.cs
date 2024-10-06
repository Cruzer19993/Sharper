using MonoGAME.Components;
using MonoGAME.Components.GUI;
using MonoGAME.ECS;
using System;
using Microsoft.Xna.Framework;
using MonoGAME.Systems.Backend.Management;
namespace MonoGAME.Source.Helpers
{
    public class GUIHelper
    {
        public static Entity CreateButton(Vector2 position, Vector2 size, string buttonText, out GUIButton buttonComponent, bool renderHitbox = false)
        {
            Entity buttonEntity = new Entity("Button", new Type[] { typeof(GUIRect), typeof(GUIButton), typeof(GUIRenderer), typeof(GUIText)});
            GUIRect btnRect = buttonEntity.GetComponent<GUIRect>();
            btnRect.SetPosition(position);
            btnRect.SetSize(size);
            buttonEntity.GetComponent<GUIText>().m_text = buttonText;
            buttonEntity.GetComponent<GUIText>().m_color = Color.Black;
            buttonComponent = buttonEntity.GetComponent<GUIButton>();
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(buttonEntity);
            }
            return buttonEntity;
        } 

        public static Entity CreateInputBox(Vector2 position, Vector2 size, out GUIInputBox inputBox, string labelText = "")
        {
            Entity inputBoxEntity = new Entity("InputBox", new Type[] { typeof(GUIRect), typeof(GUIRenderer), typeof(GUIInputBox) });
            GUIRect ibRect = inputBoxEntity.GetComponent<GUIRect>();
            ibRect.SetPosition(position);
            ibRect.SetSize(size);
            inputBox = inputBoxEntity.GetComponent<GUIInputBox>();
            inputBoxEntity.GetComponent<GUIRenderer>().m_textureName = "GUIRectangle";
            inputBoxEntity.GetComponent<GUIRenderer>().m_color = Color.WhiteSmoke;
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(inputBoxEntity);
            }
            return inputBoxEntity;

        }

        public static Entity CreateUIPanel(Vector2 position, Vector2 size, Color color)
        {
            Entity panelEntity = new Entity("UIPanel", new Type[] {typeof(GUIRect),typeof(GUIRenderer) });
            panelEntity.GetComponent<GUIRect>().SetPosition(position);
            panelEntity.GetComponent<GUIRect>().SetSize(size);
            panelEntity.GetComponent<GUIRenderer>().m_color = color;
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(panelEntity);
            }
            return panelEntity;
        }

        public static Entity CreateText(Vector2 position, string text, Color textColor)
        {
            Entity textEntity = new Entity("Text", new Type[] {typeof(GUIText),typeof(GUIRect),typeof(GUIRenderer) });
            textEntity.GetComponent<GUIRect>().SetPosition(position);
            textEntity.GetComponent<GUIRect>().SetSize(new Vector2(100,20));
            textEntity.GetComponent<GUIText>().m_text = text;
            textEntity.GetComponent<GUIText>().m_color = textColor;
            textEntity.GetComponent<GUIRenderer>().m_renderable = false;
            if(SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(textEntity);
            }
            return textEntity;
        }

        public static Entity CreateSpriteButton(string textureName,Vector2 position, out GUISprite spriteComponent, out GUIButton buttonComponent)
        {
            Entity buttonEntity = new Entity("Button", new Type[] { typeof(GUIRect), typeof(GUIButton), typeof(GUIRenderer), typeof(GUISprite) });
            GUIRect btnRect = buttonEntity.GetComponent<GUIRect>();
            btnRect.SetPosition(position);
            Vector2 texSize = new Vector2(32, 32);
            btnRect.SetSize(texSize);
            buttonComponent = buttonEntity.GetComponent<GUIButton>();
            buttonEntity.GetComponent<GUISprite>().m_textureName = textureName;
            spriteComponent = buttonEntity.GetComponent<GUISprite>();
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

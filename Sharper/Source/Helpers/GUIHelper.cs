using Sharper.Components;
using Sharper.Components.GUI;
using Sharper.ECS;
using System;
using Microsoft.Xna.Framework;
using Sharper.Systems.Backend.Management;
using Sharper.Systems.Backend;
using System.Linq;
namespace Sharper.Source.Helpers
{
    public class GUIHelper
    {
        public static Entity CreateButton(Vector2 position, Vector2 size, string displayText,MouseButton activeButton ,out GUIRect buttonRect, out GUIText buttonText, out GUIButton buttonComp)
        {
            Entity buttonEntity = new Entity("Button", new Type[] { typeof(GUIRect), typeof(GUIButton), typeof(GUIText),typeof(GUISprite)});
            GUIRect btnRect = buttonEntity.GetComponent<GUIRect>();
            GUIText btnText = buttonEntity.GetComponent<GUIText>();
            GUIButton btnComp = buttonEntity.GetComponent<GUIButton>();
            GUISprite btnSprite = buttonEntity.GetComponent<GUISprite>();
            btnRect.SetPosition(position);
            btnRect.SetSize(size);
            btnText.m_text = displayText;
            btnText.m_color = Color.Black;
            btnComp.activateButton = activeButton;
            buttonRect = btnRect;
            buttonText = btnText;
            buttonComp = btnComp;
            btnSprite.m_sprite.m_textureName = "GUIDefaultTexture";
            btnSprite.m_sprite.m_color = Color.White;
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(buttonEntity);
            }
            return buttonEntity;
        }
        public static Entity CreateButton(Vector2 position, Vector2 size, string displayText, MouseButton activeButton, Sprite buttonSprite, out GUIRect buttonRect, out GUIText buttonText, out GUIButton buttonComp)
        {
            Entity buttonEntity = new Entity("Button", new Type[] { typeof(GUIRect), typeof(GUIButton), typeof(GUIText) });
            GUIRect btnRect = buttonEntity.GetComponent<GUIRect>();
            GUIText btnText = buttonEntity.GetComponent<GUIText>();
            GUIButton btnComp = buttonEntity.GetComponent<GUIButton>();
            buttonEntity.GetComponent<GUISprite>().m_sprite = buttonSprite;
            btnRect.SetPosition(position);
            btnRect.SetSize(size);
            btnText.m_text = displayText;
            btnText.m_color = Color.Black;
            btnComp.activateButton = activeButton;
            buttonRect = btnRect;
            buttonText = btnText;
            buttonComp = btnComp;
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(buttonEntity);
            }
            return buttonEntity;
        }
        public static Entity CreateInputBox(Vector2 position, Vector2 size,out GUIText InputBoxText,out GUIInputBox InputBoxComponent,string startingText = "")
        {
            Entity inputBoxEntity = new Entity("InputBox", new Type[] { typeof(GUIRect), typeof(GUIInputBox), typeof(GUIText),typeof(GUISprite) });
            GUIRect ibRect = inputBoxEntity.GetComponent<GUIRect>();
            GUIText ibText = inputBoxEntity.GetComponent<GUIText>();
            ibRect.SetPosition(position);
            ibRect.SetSize(size);
            ibText.m_color = Color.Black;
            ibText.m_text = startingText;
            InputBoxText = ibText;
            inputBoxEntity.GetComponent<GUISprite>().m_sprite.m_textureName = "GUIRectangle";
            inputBoxEntity.GetComponent<GUISprite>().m_sprite.m_color = Color.White;
            InputBoxComponent = inputBoxEntity.GetComponent<GUIInputBox>();
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(inputBoxEntity);
            }
            return inputBoxEntity;

        }

        public static Entity CreateUIPanel(Vector2 position, Vector2 size, Sprite panelSprite)
        {
            Entity panelEntity = new Entity("UIPanel", new Type[] {typeof(GUIRect),typeof(GUISprite) });
            panelEntity.GetComponent<GUIRect>().SetPosition(position);
            panelEntity.GetComponent<GUIRect>().SetSize(size);
            panelEntity.GetComponent<GUISprite>().m_sprite = panelSprite;
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(panelEntity);
            }
            return panelEntity;
        }

        public static Entity CreateText(Vector2 position, string text, Color textColor, out GUIText Text)
        {
            Entity textEntity = new Entity("Text", new Type[] {typeof(GUIRect),typeof(GUIText) });
            textEntity.GetComponent<GUIRect>().SetPosition(position);
            textEntity.GetComponent<GUIRect>().SetSize(new Vector2(text.Count()*10,20));
            textEntity.GetComponent<GUIText>().m_text = text;
            Text = textEntity.GetComponent<GUIText>();
            if(SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(textEntity);
            }
            return textEntity;
        }

        public static  Entity CreateGUILayout(Vector2 size,Vector2 spacing, Vector2 padding, Vector2 offset, out GUILayout newLayoutGroup, params GUILayoutOptions[] options)
        {
            Entity layoutGroupEntity = new Entity("LayoutGroup", new Type[] { typeof(GUILayout), typeof(GUIRect),typeof(GUISprite) });
            newLayoutGroup = layoutGroupEntity.GetComponent<GUILayout>();
            newLayoutGroup.m_spacing = spacing;
            newLayoutGroup.m_padding = padding;
            newLayoutGroup.m_offset = offset;
            newLayoutGroup.m_layoutOptions = options;
            layoutGroupEntity.GetComponent<GUIRect>().SetSize(size);
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(layoutGroupEntity);
            }
            return layoutGroupEntity;
        }

        public static Entity CreateGridGUILayout(Vector2 size,Vector2 spacing, Vector2 padding, Vector2 offset, Vector2 cellSize,int numberOfColumns ,bool enforceCellSize, out GUILayout newLayoutGroup,params GUILayoutOptions[] options)
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
            newLayoutGroupGrid.m_layoutOptions = options;
            layoutGroupEntity.GetComponent<GUIRect>().SetScale(size);
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(layoutGroupEntity);
            }
            return layoutGroupEntity;
        }

        public static Entity CreateCheckbox(bool startingValue, out GUICheckbox gUICheckbox)
        {
            Entity checkboxEntity = new Entity("Checkbox", new Type[] { typeof(GUIRect), typeof(EntityRenderer), typeof(GUICheckbox) });
            GUICheckbox cb = checkboxEntity.GetComponent<GUICheckbox>();
            cb.isChecked = startingValue;
            checkboxEntity.GetComponent<EntityRenderer>().m_sprite.m_textureName = startingValue ? cb.checkedTexName :cb.uncheckedTexName;
            checkboxEntity.GetComponent<GUIRect>().SetSize(Vector2.One * 16);
            gUICheckbox = cb;
            if (SceneManager.CurrentScene != null)
            {
                SceneManager.CurrentScene.SpawnEntity(checkboxEntity);
            }
            return checkboxEntity;
        }
    }
}

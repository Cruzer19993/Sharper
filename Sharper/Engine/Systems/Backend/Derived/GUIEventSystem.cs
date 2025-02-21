using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Sharper.Components.GUI;
using Sharper.ECS;
namespace Sharper.Systems.Backend.Derived
{
    public class GUIEventSystem : ECSSystem
    {
        MatchingPattern ButtonMatchingPattern = new MatchingPattern(typeof(GUIRect),typeof(GUIButton));
        MatchingPattern InputBoxMatchingPattern = new MatchingPattern(typeof(GUIRect), typeof(GUIInputBox));
        GUIInputBox activeInputBox;
        GUIText activeInputBoxText;
        public override void Initialize()
        {
            AddMatchingPatterns(ButtonMatchingPattern,InputBoxMatchingPattern);
            base.Initialize();
        }
        public override void Start()
        {

        }


        void DeactivateActiveInputBox()
        {
            activeInputBox.owner.GetComponent<GUISprite>().m_uiSprite.m_color = activeInputBox.m_inactiveColor;
            activeInputBox.m_isActive = false;
            activeInputBoxText = null;
            activeInputBox = null;
        }
        void ActivateInputBox(Entity inputBoxEntity)
        {
            GUIInputBox ibcomp = inputBoxEntity.GetComponent<GUIInputBox>();
            GUISprite ibSprite = inputBoxEntity.GetComponent<GUISprite>();
            ibSprite.m_uiSprite.m_color = ibcomp.m_activeColor;
            ibcomp.m_isActive = true;
            activeInputBox = ibcomp;
            activeInputBoxText = ibcomp.m_inputBoxTextDisplay;
        }
        public override void GameUpdate()
        {
            if (InputSystem.ButtonPressed(MouseButton.Left))
            {
                Vector2 MousePos = InputSystem.MousePosition();
                foreach(Entity btn in matchingEntities[ButtonMatchingPattern])
                {
                    if (btn.GetComponent<GUIRect>().GetRect().Contains(MousePos))
                    {
                        if(btn.GetComponent<GUIButton>().buttonClicked != null)
                            btn.GetComponent<GUIButton>().buttonClicked.Invoke(this, EventArgs.Empty);
                        return;
                    }
                }
                foreach(Entity ib in matchingEntities[InputBoxMatchingPattern])
                {
                    if (ib.GetComponent<GUIRect>().GetRect().Contains(MousePos))
                    {
                        if(activeInputBox != null)
                        {
                            DeactivateActiveInputBox();
                        }
                        ActivateInputBox(ib);
                        return;
                    }
                }
                if (activeInputBox != null) DeactivateActiveInputBox();
            }
            if (activeInputBox == null) return;
            if (InputSystem.KeyPressed(Keys.Back))
            {
                if (activeInputBoxText.Text.Length > 0) activeInputBoxText.Text = activeInputBoxText.Text.Remove(activeInputBoxText.Text.Length - 1);
            }
            foreach (Keys key in Enum.GetValues(typeof(Keys))){
                if (InputSystem.KeyPressed(key))
                {
                    activeInputBoxText.Text += InputSystem.ConvertKeyEnumToChar(key);               
                }
            }
        }
        public override void OnEntityAttached(Entity target, MatchingPattern pattern)
        {
            matchingEntities[pattern].Add(target);
        }
        public override void OnEntityUpdate(Entity target, MatchingPattern pattern)
        {
        }
        public override void OnEntityDetached(Entity target)
        {

        }
    }
}

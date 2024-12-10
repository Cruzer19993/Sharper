using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Sharper.Components.GUI;
using Sharper.ECS;
using System;
using System.Diagnostics;

namespace Sharper.Systems.Backend.GUI
{
    public class GUIEventSystem : ECSSystem
    {
        bool activatedInputBox = false;
        GUIText currentInputBoxText;
        MatchingPattern GES_BTN_Pattern = new MatchingPattern(typeof(GUIRect),typeof(EntityRenderer) ,typeof(GUIButton));
        MatchingPattern GES_IB_Pattern = new MatchingPattern(typeof(GUIRect),typeof(EntityRenderer), typeof(GUIInputBox), typeof(GUIText));
        public GUIEventSystem()
        {
            if (instance == null) instance = this;
        }
        private static GUIEventSystem instance;
        public static GUIEventSystem Instance
        {
            get { return instance; }
            private set { instance = value; }
        }
        //Runs only once before Start
        public override void Initialize()
        {
            base.Initialize();
            AddMatchingPatterns(GES_IB_Pattern, GES_BTN_Pattern);
        }
        //Runs once after Initialize
        public override void Start()
        {
        }
        //Updates every frame
        public override void GameUpdate()
        {
            foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
            {
                if (InputSystem.ButtonPressed(button))
                {
                    CheckIfButtonClicked(button);
                }
            }
            if (activatedInputBox)
            {
                if (InputSystem.KeyPressed(Keys.Back))
                {
                    if (currentInputBoxText.m_text.Length > 0)
                    {
                        string newText = currentInputBoxText.m_text.Remove(currentInputBoxText.m_text.Length - 1);
                        currentInputBoxText.m_text = newText;
                    }
                }
                foreach (Keys key in Enum.GetValues(typeof(Keys)))
                {
                    if (InputSystem.KeyPressed(key))
                    {
                        currentInputBoxText.m_text += InputSystem.ConvertKeyEnumToChar(key, InputSystem.IsKeyDown(Keys.LeftShift));
                        Debug.WriteLine($"CURRENT INPUT BOX TEXT: {currentInputBoxText.m_text}");
                    }
                }
            }
        }
        //Applies changed to target entities
        public override void OnEntityUpdate(Entity target,MatchingPattern pattern)
        {
        }
        public override void OnEntityAttached(Entity newEntity, MatchingPattern pattern)
        {

        }
        public override void OnEntityDetached(Entity newEntity)
        {

        }
        public void CheckIfButtonClicked(MouseButton btn)
        {
            if (!matchingEntities.ContainsKey(GES_BTN_Pattern)) return;
            Vector2 mousePos = InputSystem.MousePosition();
            foreach(MatchingPattern pattern in matchingEntities.Keys)
            {
                for(int i=0;i < matchingEntities[pattern].Count; i++)
                {
                    if(pattern.patternSignature == GES_BTN_Pattern.patternSignature)
                    {
                        GUIRect btnRect = matchingEntities[pattern][i].GetComponent<GUIRect>();
                        GUIButton btnComponent = matchingEntities[pattern][i].GetComponent<GUIButton>();
                        if (btnRect.GetRect().Contains(mousePos))
                        {
                            btnComponent.Clicked();
                            return;
                        }
                    }
                    if(pattern.patternSignature == GES_IB_Pattern.patternSignature)
                    {
                        GUIRect ibRect = matchingEntities[pattern][i].GetComponent<GUIRect>();
                        GUIText ibText = matchingEntities[pattern][i].GetComponent<GUIText>();
                        GUIInputBox ibComp = matchingEntities[pattern][i].GetComponent<GUIInputBox>();

                        if (ibRect.GetRect().Contains(mousePos))
                        {
                            if (currentInputBoxText != null)
                                currentInputBoxText.m_color = Color.Black;
                            ibText.m_color = Color.Green;
                            ibComp.m_active = true;
                            activatedInputBox = true;
                            currentInputBoxText = ibText;
                            return;
                        }

                    }
                }
            }
            if (activatedInputBox)
            {
                GUIInputBox ibComp = currentInputBoxText.owner.GetComponent<GUIInputBox>();
                currentInputBoxText.m_color = Color.Black;
                activatedInputBox = false;
                ibComp.m_active = false;
                currentInputBoxText = null;
            }
        }
    }
}

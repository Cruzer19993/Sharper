using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGAME.Components.GUI;
using MonoGAME.ECS;
using System;
using System.Diagnostics;

namespace MonoGAME.Systems.Backend.GUI
{
    public class GUIEventSystem : ECSSystem
    {
        bool activatedInputBox = false;
        GUIInputBox currentInputBox;
        public GUIEventSystem()
        {
            if (instance == null) instance = this;
            getEntitesMatchingPartially = true;
            matchingComponentTypes = new System.Type[]
            {
                typeof(GUIRect)
            };
            partialMatchingComponentTypes = new Type[]
            {
                typeof(GUIInputBox),
                typeof(GUIButton)
            };
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
                    if (currentInputBox.m_text.Length > 0)
                    {
                        string newText = currentInputBox.m_text.Remove(currentInputBox.m_text.Length - 1);
                        currentInputBox.m_text = newText;
                    }
                }
                foreach (Keys key in Enum.GetValues(typeof(Keys)))
                {
                    if (InputSystem.KeyPressed(key))
                    {
                        currentInputBox.m_text += InputSystem.ConvertKeyEnumToChar(key, InputSystem.IsKeyDown(Keys.LeftShift));
                        Debug.WriteLine($"CURRENT INPUT BOX TEXT: {currentInputBox.m_text}");
                    }
                }
            }
        }
        //Applies changed to target entities
        public override void EntityUpdate(Entity target)
        {
        }
        public override void OnNewEntity(Entity newEntity)
        {

        }

        public void CheckIfButtonClicked(MouseButton btn)
        {
            Vector2 mousePos = InputSystem.MousePosition();
            for (int i = 0; i < realArrayEntityCount; i++)
            {
                Entity currEntity = matchingEntities[i];
                GUIRect btnRect = currEntity.GetComponent<GUIRect>();
                if (btnRect != null)
                {
                    Vector2 btnPos = btnRect.GetPosition();
                    Vector2 btnSize = btnRect.GetSize();
                    if (btnPos.X < mousePos.X &&
                        btnPos.X + btnSize.X > mousePos.X &&
                        btnPos.Y < mousePos.Y &&
                        btnPos.Y + btnSize.Y > mousePos.Y)
                    {
                        GUIButton btnComponent = currEntity.GetComponent<GUIButton>();
                        if (btnComponent != null && btnComponent.activateButton == btn)
                        {
                            btnComponent.Clicked();
                            return;
                        }
                        GUIInputBox inputBox = currEntity.GetComponent<GUIInputBox>();
                        if (inputBox != null && btn == MouseButton.Left)
                        {
                            if(currentInputBox != null)
                                currentInputBox.owner.GetComponent<GUIRenderer>().m_color = Color.Black;
                            currEntity.GetComponent<GUIRenderer>().m_color = Color.Green;
                            inputBox.m_active = true;
                            activatedInputBox = true;
                            currentInputBox = inputBox;
                            return;
                        }
                        Debug.WriteLine($"{currEntity.EntityName}");
                    }
                }
            }
            if (activatedInputBox)
            {
                currentInputBox.owner.GetComponent<GUIRenderer>().m_color = Color.Black;
                activatedInputBox = false;
                currentInputBox.m_active = false;
                currentInputBox = null;
            }
        }
    }
}

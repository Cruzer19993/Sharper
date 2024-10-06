using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Sharper.Systems.Backend
{
    public class InputSystem
    {
        static List<Keys> lastFramePressedKeys = new List<Keys>(); //Let's hope it doesn't need to be thread safe
        static List<MouseButton> lastFramePressedButtons = new List<MouseButton>();
        public static event EventHandler<MouseButton> buttonPressedEvent;
        static int lastScrollValue = 0;
        public static float ScrollDelta
        {
            get
            {
                int currentMouseScrollValue = Mouse.GetState().ScrollWheelValue;
                float scrollDelta = currentMouseScrollValue > lastScrollValue ? 1 : -1;
                if (lastScrollValue == currentMouseScrollValue) scrollDelta = 0;
                lastScrollValue = currentMouseScrollValue;
                return scrollDelta;

            }
            private set { ScrollDelta = value; }
        }
        public InputSystem()
        {
            Instance = this;
        }
        public static InputSystem Instance;

        public static Vector2 MousePosition()
        {
            return Mouse.GetState().Position.ToVector2();
        }

        public static bool ButtonPressed(MouseButton button)
        {
            if (ButtonDown(button))
            {
                if (!lastFramePressedButtons.Contains(button))
                {
                    lastFramePressedButtons.Add(button);
                }
            }
            if (!ButtonDown(button) && lastFramePressedButtons.Contains(button))
            {
                lastFramePressedButtons.Remove(button);
                if (buttonPressedEvent != null) buttonPressedEvent.Invoke(new object(), button);
                return true;
            }
            else return false;
        }

        public static bool ButtonDown(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return Mouse.GetState().LeftButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return Mouse.GetState().RightButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return Mouse.GetState().MiddleButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        public bool AnyKeyPressed()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (IsKeyDown(key))
                {
                    if (!lastFramePressedKeys.Contains(key))
                        lastFramePressedKeys.Add(key);
                }
                if (!IsKeyDown(key) && lastFramePressedKeys.Contains(key))
                {
                    lastFramePressedKeys.Remove(key);
                    return true;
                }
                else return false;
            }
            return false;
        }

        public static bool KeyPressed(Keys key)
        {
            if (IsKeyDown(key))
            {
                if (!lastFramePressedKeys.Contains(key))
                    lastFramePressedKeys.Add(key);
            }
            if (!IsKeyDown(key) && lastFramePressedKeys.Contains(key))
            {
                lastFramePressedKeys.Remove(key);
                return true;
            }
            else return false;
        }

        public static bool IsKeyDown(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }

        public static bool IsKeyUp(Keys key)
        {
            return Keyboard.GetState().IsKeyUp(key);
        }

        public static char ConvertKeyEnumToChar(Keys enumKey, bool shift = false)
        {
            char key;
            switch (enumKey)
            {
                //Alphabet keys
                case Keys.A: if (shift) { key = 'A'; } else { key = 'a'; } break;
                case Keys.B: if (shift) { key = 'B'; } else { key = 'b'; } break;
                case Keys.C: if (shift) { key = 'C'; } else { key = 'c'; } break;
                case Keys.D: if (shift) { key = 'D'; } else { key = 'd'; } break;
                case Keys.E: if (shift) { key = 'E'; } else { key = 'e'; } break;
                case Keys.F: if (shift) { key = 'F'; } else { key = 'f'; } break;
                case Keys.G: if (shift) { key = 'G'; } else { key = 'g'; } break;
                case Keys.H: if (shift) { key = 'H'; } else { key = 'h'; } break;
                case Keys.I: if (shift) { key = 'I'; } else { key = 'i'; } break;
                case Keys.J: if (shift) { key = 'J'; } else { key = 'j'; } break;
                case Keys.K: if (shift) { key = 'K'; } else { key = 'k'; } break;
                case Keys.L: if (shift) { key = 'L'; } else { key = 'l'; } break;
                case Keys.M: if (shift) { key = 'M'; } else { key = 'm'; } break;
                case Keys.N: if (shift) { key = 'N'; } else { key = 'n'; } break;
                case Keys.O: if (shift) { key = 'O'; } else { key = 'o'; } break;
                case Keys.P: if (shift) { key = 'P'; } else { key = 'p'; } break;
                case Keys.Q: if (shift) { key = 'Q'; } else { key = 'q'; } break;
                case Keys.R: if (shift) { key = 'R'; } else { key = 'r'; } break;
                case Keys.S: if (shift) { key = 'S'; } else { key = 's'; } break;
                case Keys.T: if (shift) { key = 'T'; } else { key = 't'; } break;
                case Keys.U: if (shift) { key = 'U'; } else { key = 'u'; } break;
                case Keys.V: if (shift) { key = 'V'; } else { key = 'v'; } break;
                case Keys.W: if (shift) { key = 'W'; } else { key = 'w'; } break;
                case Keys.X: if (shift) { key = 'X'; } else { key = 'x'; } break;
                case Keys.Y: if (shift) { key = 'Y'; } else { key = 'y'; } break;
                case Keys.Z: if (shift) { key = 'Z'; } else { key = 'z'; } break;

                //Decimal keys
                case Keys.D0: if (shift) { key = ')'; } else { key = '0'; } break;
                case Keys.D1: if (shift) { key = '!'; } else { key = '1'; } break;
                case Keys.D2: if (shift) { key = '@'; } else { key = '2'; } break;
                case Keys.D3: if (shift) { key = '#'; } else { key = '3'; } break;
                case Keys.D4: if (shift) { key = '$'; } else { key = '4'; } break;
                case Keys.D5: if (shift) { key = '%'; } else { key = '5'; } break;
                case Keys.D6: if (shift) { key = '^'; } else { key = '6'; } break;
                case Keys.D7: if (shift) { key = '&'; } else { key = '7'; } break;
                case Keys.D8: if (shift) { key = '*'; } else { key = '8'; } break;
                case Keys.D9: if (shift) { key = '('; } else { key = '9'; } break;

                //Decimal numpad keys
                case Keys.NumPad0: key = '0'; break;
                case Keys.NumPad1: key = '1'; break;
                case Keys.NumPad2: key = '2'; break;
                case Keys.NumPad3: key = '3'; break;
                case Keys.NumPad4: key = '4'; break;
                case Keys.NumPad5: key = '5'; break;
                case Keys.NumPad6: key = '6'; break;
                case Keys.NumPad7: key = '7'; break;
                case Keys.NumPad8: key = '8'; break;
                case Keys.NumPad9: key = '9'; break;

                //Special keys
                case Keys.OemTilde: if (shift) { key = '~'; } else { key = '`'; } break;
                case Keys.OemSemicolon: if (shift) { key = ':'; } else { key = ';'; } break; ;
                case Keys.OemQuotes: if (shift) { key = '"'; } else { key = '\''; } break; ;
                case Keys.OemQuestion: if (shift) { key = '?'; } else { key = '/'; } break; ;
                case Keys.OemPlus: if (shift) { key = '+'; } else { key = '='; } break;
                case Keys.OemPipe: if (shift) { key = '|'; } else { key = '\\'; } break; ;
                case Keys.OemPeriod: if (shift) { key = '>'; } else { key = '.'; } break;
                case Keys.OemOpenBrackets: if (shift) { key = '{'; } else { key = '['; } break;
                case Keys.OemCloseBrackets: if (shift) { key = '}'; } else { key = ']'; } break;
                case Keys.OemMinus: if (shift) { key = '_'; } else { key = '-'; }; break;
                case Keys.OemComma: if (shift) { key = '<'; } else { key = ','; }; break;
                case Keys.Space: key = ' '; break;
                default: key = 'X'; break;
            }
            return key;
        }
    }

    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }
}

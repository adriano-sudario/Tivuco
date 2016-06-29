using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class KeyboardAndMouseInputs : Inputs
    {
        // Keyboard states used to determine key presses
        public KeyboardState currentKeyboardState;
        public KeyboardState previousKeyboardState;

        //Mouse states used to track Mouse button press
        public MouseState currentMouseState;
        public MouseState previousMouseState;

        public void GetInputs()
        {
            // Save the previous state of the keyboard and mouse so we can determine single key/button presses
            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;

            // Read the current state of the keyboard and mouse and store it
            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();
        }

        public float MoveX()
        {
            if (currentKeyboardState.IsKeyDown(Keys.J))
            {
                return -1;
            }

            if (currentKeyboardState.IsKeyDown(Keys.L))
            {
                return 1;
            }

            return 0;
        }

        public float MoveY()
        {
            if (currentKeyboardState.IsKeyDown(Keys.I))
            {
                return -1;
            }

            if (currentKeyboardState.IsKeyDown(Keys.K))
            {
                return 1;
            }

            return 0;
        }

        public bool JumpHolding()
        {
            if (currentKeyboardState.IsKeyDown(Keys.I))
            {
                return true;
            }

            return false;
        }

        public bool JumpClicked()
        {
            if (currentKeyboardState.IsKeyDown(Keys.I) && previousKeyboardState.IsKeyUp(Keys.I))
            {
                return true;
            }

            return false;
        }

        public bool JumpReleased()
        {
            if (previousKeyboardState.IsKeyDown(Keys.I) && currentKeyboardState.IsKeyUp(Keys.I))
            {
                return true;
            }

            return false;
        }

        public bool FirstSkillHolding()
        {
            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                return true;
            }

            return false;
        }

        public bool FirstSkillClicked()
        {
            if (previousMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                return true;
            }

            return false;
        }

        public bool FirstSkillReleased()
        {
            if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
            {
                return true;
            }

            return false;
        }

        public bool SecondSkillHolding()
        {
            if (currentMouseState.MiddleButton == ButtonState.Pressed)
            {
                return true;
            }

            return false;
        }

        public bool SecondSkillClicked()
        {
            if (previousMouseState.MiddleButton == ButtonState.Released && currentMouseState.MiddleButton == ButtonState.Pressed)
            {
                return true;
            }

            return false;
        }

        public bool SecondSkillReleased()
        {
            if (currentMouseState.MiddleButton == ButtonState.Released && previousMouseState.MiddleButton == ButtonState.Pressed)
            {
                return true;
            }

            return false;
        }

        public bool ThirdSkillHolding()
        {
            if (currentMouseState.RightButton == ButtonState.Pressed)
            {
                return true;
            }

            return false;
        }

        public bool ThirdSkillClicked()
        {
            if (previousMouseState.RightButton == ButtonState.Released && currentMouseState.RightButton == ButtonState.Pressed)
            {
                return true;
            }

            return false;
        }

        public bool ThirdSkillReleased()
        {
            if (currentMouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed)
            {
                return true;
            }

            return false;
        }

        public bool Aim()
        {
            if (currentMouseState.ScrollWheelValue != previousMouseState.ScrollWheelValue)
            {
                return true;
            }

            return false;
        }

        public void RotateAim(Aim aim, Player.Side side)
        {
            if (currentMouseState.Position != previousMouseState.Position)
            {
                if (currentMouseState.Position.Y < aim.mouseAimArea.Y - (aim.mouseAimArea.Height / 2))
                {
                    aim.Rotate((int)1);
                }
                else if (currentMouseState.Position.Y > aim.mouseAimArea.Y - (aim.mouseAimArea.Height / 2))
                {
                    aim.Rotate((int)-1);
                }
            }

            aim.angle = aim.DegreeToRadian(aim.angleInDegrees);
        }

        public void SetDefaultAim(Aim aim)
        {
            if (currentKeyboardState.IsKeyDown(Keys.I) && currentKeyboardState.IsKeyDown(Keys.L))
            {
                aim.angleInDegrees = 45;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.K) && currentKeyboardState.IsKeyDown(Keys.L))
            {
                aim.angleInDegrees = -45;
            }
            if (currentKeyboardState.IsKeyDown(Keys.I) && currentKeyboardState.IsKeyDown(Keys.J))
            {
                aim.angleInDegrees = 45;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.K) && currentKeyboardState.IsKeyDown(Keys.J))
            {
                aim.angleInDegrees = -45;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.L))
            {
                aim.angleInDegrees = 0;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.I))
            {
                aim.angleInDegrees = 90;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.J))
            {
                aim.angleInDegrees = 0;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.K))
            {
                aim.angleInDegrees = -90;
            }
            else
            {
                aim.angleInDegrees = 0;
            }
        }

        public bool PauseGame()
        {
            if (currentKeyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter))
            {
                return true;
            }

            return false;
        }

        public int SelectX()
        {
            if (currentKeyboardState.IsKeyDown(Keys.J) && previousKeyboardState.IsKeyUp(Keys.J))
            {
                return -1;
            }

            if (currentKeyboardState.IsKeyDown(Keys.L) && previousKeyboardState.IsKeyUp(Keys.L))
            {
                return 1;
            }

            return 0;
        }

        public int SelectY()
        {
            if (currentKeyboardState.IsKeyDown(Keys.I) && previousKeyboardState.IsKeyUp(Keys.I))
            {
                return -1;
            }

            if (currentKeyboardState.IsKeyDown(Keys.K) && previousKeyboardState.IsKeyUp(Keys.K))
            {
                return 1;
            }

            return 0;
        }

        public bool Back()
        {
            if (previousMouseState.RightButton == ButtonState.Released && currentMouseState.RightButton == ButtonState.Pressed)
            {
                return true;
            }

            return false;
        }
    }
}

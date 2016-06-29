using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class KeyboardInputs : Inputs
    {
        // Keyboard states used to determine key presses
        public KeyboardState currentKeyboardState;
        public KeyboardState previousKeyboardState;

        Keys leftKey;
        Keys rightKey;
        Keys upKey;
        Keys downKey;

        Keys jumpKey;
        Keys backKey;
        Keys enterKey;

        Keys firstSkillKey;
        Keys secondSkillKey;
        Keys thirdSkillKey;
        Keys aimKey;

        public KeyboardIndex keyboardIndex;

        public enum KeyboardIndex { One, Two }

        public KeyboardInputs(KeyboardIndex keyboardIndex)
        {
            this.keyboardIndex = keyboardIndex;

            SetDefaultKeysByIndex(keyboardIndex);
        }

        public void SetDefaultKeysByIndex(KeyboardIndex index)
        {
            switch (index)
            {
                case KeyboardIndex.One:
                    leftKey = Keys.Left;
                    rightKey = Keys.Right;
                    upKey = Keys.Up;
                    downKey = Keys.Down;

                    jumpKey = Keys.Space;
                    backKey = Keys.F1;
                    enterKey = Keys.Enter;

                    firstSkillKey = Keys.Q;
                    secondSkillKey = Keys.W;
                    thirdSkillKey = Keys.E;
                    aimKey = Keys.R;
                    break;

                case KeyboardIndex.Two:
                    leftKey = Keys.NumPad4;
                    rightKey = Keys.NumPad6;
                    upKey = Keys.NumPad8;
                    downKey = Keys.NumPad5;

                    jumpKey = Keys.RightControl;
                    backKey = Keys.Back;
                    enterKey = Keys.RightAlt;

                    firstSkillKey = Keys.U;
                    secondSkillKey = Keys.I;
                    thirdSkillKey = Keys.O;
                    aimKey = Keys.P;
                    break; 
            }
        }

        public void GetInputs()
        {
            // Save the previous state of the keyboard so we can determine single key/button presses
            previousKeyboardState = currentKeyboardState;

            // Read the current state of the keyboard and store it
            currentKeyboardState = Keyboard.GetState();
        }

        public float MoveX()
        {
            if (currentKeyboardState.IsKeyDown(leftKey))
            {
                return -1;
            }

            if (currentKeyboardState.IsKeyDown(rightKey))
            {
                return 1;
            }

            return 0;
        }

        public float MoveY()
        {
            if (currentKeyboardState.IsKeyDown(upKey))
            {
                return -1;
            }

            if (currentKeyboardState.IsKeyDown(downKey))
            {
                return 1;
            }

            return 0;
        }

        public bool JumpHolding()
        {
            if (currentKeyboardState.IsKeyDown(jumpKey))
            {
                return true;
            }

            return false;
        }

        public bool JumpClicked()
        {
            if (currentKeyboardState.IsKeyDown(jumpKey) && previousKeyboardState.IsKeyUp(jumpKey))
            {
                return true;
            }

            return false;
        }

        public bool JumpReleased()
        {
            if (previousKeyboardState.IsKeyDown(jumpKey) && currentKeyboardState.IsKeyUp(jumpKey))
            {
                return true;
            }

            return false;
        }

        public bool FirstSkillHolding()
        {
            if (currentKeyboardState.IsKeyDown(firstSkillKey))
            {
                return true;
            }

            return false;
        }

        public bool FirstSkillClicked()
        {
            if (previousKeyboardState.IsKeyUp(firstSkillKey) && currentKeyboardState.IsKeyDown(firstSkillKey))
            {
                return true;
            }

            return false;
        }

        public bool FirstSkillReleased()
        {
            if (currentKeyboardState.IsKeyUp(firstSkillKey) && previousKeyboardState.IsKeyDown(firstSkillKey))
            {
                return true;
            }

            return false;
        }

        public bool SecondSkillHolding()
        {
            if (currentKeyboardState.IsKeyDown(secondSkillKey))
            {
                return true;
            }

            return false;
        }

        public bool SecondSkillClicked()
        {
            if (previousKeyboardState.IsKeyUp(secondSkillKey) && currentKeyboardState.IsKeyDown(secondSkillKey))
            {
                return true;
            }

            return false;
        }

        public bool SecondSkillReleased()
        {
            if (currentKeyboardState.IsKeyUp(secondSkillKey) && previousKeyboardState.IsKeyDown(secondSkillKey))
            {
                return true;
            }

            return false;
        }

        public bool ThirdSkillHolding()
        {
            if (currentKeyboardState.IsKeyDown(thirdSkillKey))
            {
                return true;
            }

            return false;
        }

        public bool ThirdSkillClicked()
        {
            if (previousKeyboardState.IsKeyUp(thirdSkillKey) && currentKeyboardState.IsKeyDown(thirdSkillKey))
            {
                return true;
            }

            return false;
        }

        public bool ThirdSkillReleased()
        {
            if (currentKeyboardState.IsKeyUp(thirdSkillKey) && previousKeyboardState.IsKeyDown(thirdSkillKey))
            {
                return true;
            }

            return false;
        }

        public bool Aim()
        {
            if (currentKeyboardState.IsKeyDown(aimKey) && previousKeyboardState.IsKeyUp(aimKey))
            {
                return true;
            }

            return false;
        }

        public void RotateAim(Aim aim, Player.Side side)
        {
            if (currentKeyboardState.IsKeyDown(upKey))
            {
                aim.Rotate((int)1);
            }
            else if (currentKeyboardState.IsKeyDown(downKey))
            {
                aim.Rotate((int)-1);
            }

            aim.angle = aim.DegreeToRadian(aim.angleInDegrees);
        }

        public void SetDefaultAim(Aim aim)
        {
            if (currentKeyboardState.IsKeyDown(upKey) && currentKeyboardState.IsKeyDown(rightKey))
            {
                aim.angleInDegrees = 45;
            }
            else if (currentKeyboardState.IsKeyDown(downKey) && currentKeyboardState.IsKeyDown(rightKey))
            {
                aim.angleInDegrees = -45;
            }
            if (currentKeyboardState.IsKeyDown(upKey) && currentKeyboardState.IsKeyDown(leftKey))
            {
                aim.angleInDegrees = 45;
            }
            else if (currentKeyboardState.IsKeyDown(downKey) && currentKeyboardState.IsKeyDown(leftKey))
            {
                aim.angleInDegrees = -45;
            }
            else if (currentKeyboardState.IsKeyDown(rightKey))
            {
                aim.angleInDegrees = 0;
            }
            else if (currentKeyboardState.IsKeyDown(upKey))
            {
                aim.angleInDegrees = 90;
            }
            else if (currentKeyboardState.IsKeyDown(leftKey))
            {
                aim.angleInDegrees = 0;
            }
            else if (currentKeyboardState.IsKeyDown(downKey))
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
            if (currentKeyboardState.IsKeyDown(enterKey) && previousKeyboardState.IsKeyUp(enterKey))
            {
                return true;
            }

            return false;
        }

        public int SelectX()
        {
            if (currentKeyboardState.IsKeyDown(leftKey) && previousKeyboardState.IsKeyUp(leftKey))
            {
                return -1;
            }

            if (currentKeyboardState.IsKeyDown(rightKey) && previousKeyboardState.IsKeyUp(rightKey))
            {
                return 1;
            }

            return 0;
        }

        public int SelectY()
        {
            if (currentKeyboardState.IsKeyDown(upKey) && previousKeyboardState.IsKeyUp(upKey))
            {
                return -1;
            }

            if (currentKeyboardState.IsKeyDown(downKey) && previousKeyboardState.IsKeyUp(downKey))
            {
                return 1;
            }

            return 0;
        }

        public bool Back()
        {
            if (currentKeyboardState.IsKeyDown(backKey) && previousKeyboardState.IsKeyUp(backKey))
            {
                return true;
            }

            return false;
        }
    }
}

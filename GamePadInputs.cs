using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class GamePadInputs : Inputs
    {
        // Gamepad states used to determine button presses
        public GamePadState currentGamePadState;
        public GamePadState previousGamePadState;

        // Player's index of gamepad
        public PlayerIndex playerIndex;

        public GamePadInputs(PlayerIndex index)
        {
            playerIndex = index;
        }

        public void GetInputs()
        {
            // Save the previous state of the game pad so we can determine single key/button presses
            previousGamePadState = currentGamePadState;

            // Read the current state of the gamepad and store it
            currentGamePadState = GamePad.GetState(playerIndex);
        }

        public float MoveX()
        {
            if (currentGamePadState.DPad.Left == ButtonState.Pressed ||
                currentGamePadState.IsButtonDown(Buttons.LeftThumbstickLeft))
            {
                return -1;
            }

            if (currentGamePadState.DPad.Right == ButtonState.Pressed ||
                currentGamePadState.IsButtonDown(Buttons.LeftThumbstickRight))
            {
                return 1;
            }

            return 0;
        }

        public float MoveY()
        {
            if (currentGamePadState.DPad.Up == ButtonState.Pressed || 
                currentGamePadState.IsButtonDown(Buttons.LeftThumbstickUp))
            {
                return -1;
            }

            if (currentGamePadState.DPad.Down == ButtonState.Pressed ||
                currentGamePadState.IsButtonDown(Buttons.LeftThumbstickDown))
            {
                return 1;
            }

            return 0;
        }

        public bool JumpHolding()
        {
            if (currentGamePadState.Buttons.A == ButtonState.Pressed)
            {
                return true;
            }

            return false;
        }

        public bool JumpClicked()
        {
            if (currentGamePadState.Buttons.A == ButtonState.Pressed && previousGamePadState.Buttons.A == ButtonState.Released)
            {
                return true;
            }

            return false;
        }

        public bool JumpReleased()
        {
            if (previousGamePadState.Buttons.A == ButtonState.Pressed && currentGamePadState.Buttons.A == ButtonState.Released)
            {
                return true;
            }

            return false;
        }

        public bool FirstSkillHolding()
        {
            if (currentGamePadState.IsButtonDown(Buttons.RightShoulder))
            {
                return true;
            }

            return false;
        }

        public bool FirstSkillClicked()
        {
            if (previousGamePadState.IsButtonUp(Buttons.RightShoulder) && currentGamePadState.IsButtonDown(Buttons.RightShoulder))
            {
                return true;
            }

            return false;
        }

        public bool FirstSkillReleased()
        {
            if (currentGamePadState.IsButtonUp(Buttons.RightShoulder) && previousGamePadState.IsButtonDown(Buttons.RightShoulder))
            {
                return true;
            }

            return false;
        }

        public bool SecondSkillHolding()
        {
            if (currentGamePadState.IsButtonDown(Buttons.RightTrigger))
            {
                return true;
            }

            return false;
        }

        public bool SecondSkillClicked()
        {
            if (previousGamePadState.IsButtonUp(Buttons.RightTrigger) && currentGamePadState.IsButtonDown(Buttons.RightTrigger))
            {
                return true;
            }

            return false;
        }

        public bool SecondSkillReleased()
        {
            if (currentGamePadState.IsButtonUp(Buttons.RightTrigger) && previousGamePadState.IsButtonDown(Buttons.RightTrigger))
            {
                return true;
            }

            return false;
        }

        public bool ThirdSkillHolding()
        {
            if (currentGamePadState.IsButtonDown(Buttons.LeftShoulder))
            {
                return true;
            }

            return false;
        }

        public bool ThirdSkillClicked()
        {
            if (previousGamePadState.IsButtonUp(Buttons.LeftShoulder) && currentGamePadState.IsButtonDown(Buttons.LeftShoulder))
            {
                return true;
            }

            return false;
        }

        public bool ThirdSkillReleased()
        {
            if (currentGamePadState.IsButtonUp(Buttons.LeftShoulder) && previousGamePadState.IsButtonDown(Buttons.LeftShoulder))
            {
                return true;
            }

            return false;
        }

        public bool Aim()
        {
            if (currentGamePadState.IsButtonDown(Buttons.RightStick) && previousGamePadState.IsButtonUp(Buttons.RightStick))
            {
                return true;
            }

            return false;
        }

        public void RotateAim(Aim aim, Player.Side side)
        {
            float rightThumbstickY = currentGamePadState.ThumbSticks.Right.Y;
            float rightThumbstickX = currentGamePadState.ThumbSticks.Right.X;

            if ((rightThumbstickX != 0 || rightThumbstickY != 0))
            {
                float thumbstickAngle = 0;

                float thumbstickAngleAngleInDegrees = 0;

                thumbstickAngle = (float)Math.Atan2(rightThumbstickY, rightThumbstickX);

                thumbstickAngleAngleInDegrees = (float)aim.RadianToDegree(thumbstickAngle);

                if (side == Player.Side.Left)
                {
                    if (thumbstickAngleAngleInDegrees >= 0)
                        thumbstickAngleAngleInDegrees = 90 - (thumbstickAngleAngleInDegrees - 90);
                    else
                        thumbstickAngleAngleInDegrees = -90 + ((thumbstickAngleAngleInDegrees + 90) * -1);
                }

                aim.Rotate(thumbstickAngleAngleInDegrees);
            }

            aim.angle = aim.DegreeToRadian(aim.angleInDegrees);
        }

        public void SetDefaultAim(Aim aim)
        {
            float leftThumbstickY = currentGamePadState.ThumbSticks.Left.Y;
            float leftThumbstickX = currentGamePadState.ThumbSticks.Left.X;

            if ((leftThumbstickX != 0 || leftThumbstickY != 0))
            {
                float thumbstickAngle = 0;

                float thumbstickAngleAngleInDegrees = 0;

                thumbstickAngle = (float)Math.Atan2(leftThumbstickY, leftThumbstickX);

                thumbstickAngleAngleInDegrees = (float)aim.RadianToDegree(thumbstickAngle);

                if (thumbstickAngleAngleInDegrees > 22.5 && thumbstickAngleAngleInDegrees <= 67.5)
                {
                    aim.angleInDegrees = 45;
                }
                else if (thumbstickAngleAngleInDegrees > 67.5 && thumbstickAngleAngleInDegrees <= 112.5)
                {
                    aim.angleInDegrees = 90;
                }
                else if (thumbstickAngleAngleInDegrees > 112.5 && thumbstickAngleAngleInDegrees <= 157.5)
                {
                    aim.angleInDegrees = 45;
                }
                else if ((thumbstickAngleAngleInDegrees > 157.5 && thumbstickAngleAngleInDegrees <= 180) ||
                            (thumbstickAngleAngleInDegrees >= -180 && thumbstickAngleAngleInDegrees <= -157.5))
                {
                    aim.angleInDegrees = 0;
                }
                else if (thumbstickAngleAngleInDegrees > -157.5 && thumbstickAngleAngleInDegrees <= -112.5)
                {
                    aim.angleInDegrees = -45;
                }
                else if (thumbstickAngleAngleInDegrees > -112.5 && thumbstickAngleAngleInDegrees <= -67.5)
                {
                    aim.angleInDegrees = -90;
                }
                else if (thumbstickAngleAngleInDegrees > -67.5 && thumbstickAngleAngleInDegrees <= -22.5)
                {
                    aim.angleInDegrees = -45;
                }
                else if ((thumbstickAngleAngleInDegrees > -22.5 && thumbstickAngleAngleInDegrees <= 0) ||
                            (thumbstickAngleAngleInDegrees >= 0 && thumbstickAngleAngleInDegrees <= 22.5))
                {
                    aim.angleInDegrees = 0;
                }
            }
            else
            {
                aim.angleInDegrees = 0;
            }
        }

        public bool PauseGame()
        {
            if (currentGamePadState.IsButtonDown(Buttons.Start) && previousGamePadState.IsButtonUp(Buttons.Start))
            {
                return true;
            }

            return false;
        }

        public int SelectX()
        {
            if ((currentGamePadState.IsButtonDown(Buttons.LeftThumbstickLeft) && previousGamePadState.IsButtonUp(Buttons.LeftThumbstickLeft)) ||
                    (currentGamePadState.DPad.Left == ButtonState.Pressed && currentGamePadState.DPad.Left == ButtonState.Released))
            {
                return -1;
            }

            if ((currentGamePadState.IsButtonDown(Buttons.LeftThumbstickRight) && previousGamePadState.IsButtonUp(Buttons.LeftThumbstickRight)) ||
                    (currentGamePadState.DPad.Right == ButtonState.Pressed && currentGamePadState.DPad.Right == ButtonState.Released))
            {
                return 1;
            }

            return 0;
        }

        public int SelectY()
        {
            if ((currentGamePadState.IsButtonDown(Buttons.LeftThumbstickUp) && previousGamePadState.IsButtonUp(Buttons.LeftThumbstickUp)) ||
                    (currentGamePadState.DPad.Up == ButtonState.Pressed && currentGamePadState.DPad.Up == ButtonState.Released))
            {
                return -1;
            }

            if ((currentGamePadState.IsButtonDown(Buttons.LeftThumbstickDown) && previousGamePadState.IsButtonUp(Buttons.LeftThumbstickDown)) ||
                    (currentGamePadState.DPad.Down == ButtonState.Pressed && currentGamePadState.DPad.Down == ButtonState.Released))
            {
                return 1;
            }

            return 0;
        }

        public bool Back()
        {
            if (currentGamePadState.Buttons.B == ButtonState.Pressed && previousGamePadState.Buttons.B == ButtonState.Released)
            {
                return true;
            }

            return false;
        }
    }
}

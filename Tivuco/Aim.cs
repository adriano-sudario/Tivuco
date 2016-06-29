using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Aim
    {
        public Texture2D spritesheet;
        public Vector2 position;
        public Vector2 origin;
        public Rectangle rectangle;
        public Rectangle mouseAimArea;
        public int frameWidth;
        public int elapsedTime;
        public int currentFrame;
        public int lastFrame;
        public float angle;
        public int angleInDegrees;
        public int aimMovementSpeed;
        public int millisecondsToChangeFrame;
        public bool angleJustTurnedRight;
        SpriteEffects effect;

        public Aim(Texture2D spritesheet, int frameWidth, int lastAnimationFrame, int millisecondsToChangeFrame)
        {
            this.spritesheet = spritesheet;
            angle = 0;
            angleInDegrees = 0;
            aimMovementSpeed = 5;
            elapsedTime = 0;
            this.millisecondsToChangeFrame = millisecondsToChangeFrame;
            rectangle = new Rectangle(0, 0, frameWidth, spritesheet.Height);
            mouseAimArea = new Rectangle((Game1.screenWidth / 2) - 20,
                                        (Game1.screenHeight / 2) - 20,
                                         40,
                                         40);
            //position = new Vector2(0, 0);
            origin = new Vector2(0, spritesheet.Height / 2);
            currentFrame = 0;
            this.frameWidth = frameWidth;
            lastFrame = lastAnimationFrame;
            angleJustTurnedRight = false;
        }

        public void Update(GameTime gameTime, Player.Side side)
        {
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime > millisecondsToChangeFrame)
            {
                currentFrame++;

                if (currentFrame > lastFrame)
                {
                    currentFrame = 0;
                }

                rectangle.X = currentFrame * frameWidth;

                elapsedTime = 0;
            }

            if (side == Player.Side.Right)
            {
                angle = angle * -1f;
                origin.X = 0;
                effect = SpriteEffects.None;
            }
            else if (side == Player.Side.Left)
            {
                origin.X = frameWidth;
                effect = SpriteEffects.FlipHorizontally;
            }
        }

        public void Rotate(int multiplier)
        {
            angleInDegrees += aimMovementSpeed * multiplier;

            if (angleInDegrees > 90)
                angleInDegrees = 90;
            else if (angleInDegrees < -90)
                angleInDegrees = -90;
        }

        public void Rotate(float thumbstickAngleInDegrees)
        {
            int multiplier = 0;

            if (thumbstickAngleInDegrees > angleInDegrees)
            {
                if (angleInDegrees + aimMovementSpeed > thumbstickAngleInDegrees)
                    return;

                multiplier = 1;
            }
            else if (thumbstickAngleInDegrees < angleInDegrees)
            {
                if (angleInDegrees - aimMovementSpeed < thumbstickAngleInDegrees)
                    return;

                multiplier = -1;
            }
            else
            {
                return;
            }

            angleInDegrees += aimMovementSpeed * multiplier;

            if (angleInDegrees > 90)
                angleInDegrees = 90;
            else if (angleInDegrees < -90)
                angleInDegrees = -90;
        }

        public float RadianToDegree(float angle)
        {
            return (float)(angle * (180.0 / Math.PI));
        }

        public float DegreeToRadian(float angle)
        {
            if (angleInDegrees < 0)
                angle = 360 + angleInDegrees;

            return (float)(Math.PI * angle / 180.0);
        }

        public void Draw(SpriteBatch spriteBatch, Player.Side side)
        {
            spriteBatch.Draw(spritesheet, position, rectangle, Color.White, angle, origin, 1.0f, effect, 1);
        }
    }
}

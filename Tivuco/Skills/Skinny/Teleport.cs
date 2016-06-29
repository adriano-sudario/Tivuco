using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Teleport
    {
        Animation disappearance;

        float disappearancePositionX
        {
            get { return disappearance.destinationRect.X; }
            set
            {
                disappearance.position.X = value;
                disappearance.destinationRect.X = (int)value;
            }
        }

        float disappearancePositionY
        {
            get { return disappearance.destinationRect.Y; }
            set
            {
                disappearance.position.Y = value;
                disappearance.destinationRect.Y = (int)value;
            }
        }

        Animation appearanceSmoke;

        float appearanceSmokePositionX
        {
            get { return appearanceSmoke.destinationRect.X; }
            set
            {
                appearanceSmoke.position.X = value;
                appearanceSmoke.destinationRect.X = (int)value;
            }
        }

        float appearanceSmokePositionY
        {
            get { return appearanceSmoke.destinationRect.Y; }
            set
            {
                appearanceSmoke.position.Y = value;
                appearanceSmoke.destinationRect.Y = (int)value;
            }
        }

        float distance;

        public bool skinnyTeleporting;
        bool drawDisappearance;
        bool drawAppearanceSmoke;

        public Teleport(Animation disappearance, Animation appearanceSmoke, float distance)
        {
            this.disappearance = disappearance;
            this.appearanceSmoke = appearanceSmoke;

            skinnyTeleporting = false;
            drawDisappearance = true;
            drawAppearanceSmoke = true;
            this.distance = distance;
        }

        public void SetTeleport(Skinny skinny, float angle, Player.Side direction)
        {
            ChangePosition(skinny, angle, direction);

            skinnyTeleporting = true;
            drawDisappearance = true;
            drawAppearanceSmoke = true;
            disappearance.currentFrame = 0;
            appearanceSmoke.currentFrame = 0;
        }

        public void ChangePosition(Skinny skinny, float angle, Player.Side direction)
        {
            disappearancePositionX = (int)skinny.positionX;
            disappearancePositionY = (int)skinny.positionY;

            float initialPositionX = (int)skinny.positionX;
            float initialPositionY = (int)skinny.positionY;

            if (direction == Player.Side.Right)
            {
                skinny.positionX += (float)((distance) * Math.Cos(angle * -1));
            }
            else
            {
                skinny.positionX += -((float)((distance) * Math.Cos(angle * -1)));
            }

            skinny.positionY += (float)((distance) * Math.Sin(angle * -1));

            skinny.positionX = MathHelper.Clamp(skinny.positionX, 0, Game1.screenWidth - (skinny.width) - 1);
            skinny.positionY = MathHelper.Clamp(skinny.positionY, 0, Game1.screenHeight - (skinny.height) - 1);

            skinny.AdjustPositionInCaseOfTilesIntersection(initialPositionX, initialPositionY);

            appearanceSmokePositionX = (int)skinny.positionX - ((appearanceSmoke.frameWidth - skinny.width) / 2);
            appearanceSmokePositionY = (int)skinny.positionY - ((appearanceSmoke.frameHeight - skinny.height) / 2);
        }

        public void Update(GameTime gameTime)
        {
            if (!skinnyTeleporting)
                return;

            if (!drawDisappearance && !drawAppearanceSmoke)
            {
                skinnyTeleporting = false;
            }
            else
            {
                if (drawDisappearance)
                {
                    disappearance.Update(gameTime);

                    if (disappearance.LastFrame())
                    {
                        drawDisappearance = false;
                    }
                }

                if (drawAppearanceSmoke)
                {
                    appearanceSmoke.Update(gameTime);

                    if (appearanceSmoke.LastFrame())
                    {
                        drawAppearanceSmoke = false;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, bool isRight)
        {
            if (!skinnyTeleporting)
                return;

            if (drawDisappearance)
                disappearance.Draw(spriteBatch, isRight);

            if (drawAppearanceSmoke)
                appearanceSmoke.Draw(spriteBatch, isRight);
        }
    }
}

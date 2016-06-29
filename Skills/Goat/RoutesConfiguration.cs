using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class RoutesConfiguration
    {
        public List<Animation> appearanceSmokes;
        Texture2D appearanceSmokeTexture;

        string text;
        string dots;
        int dotsAddMillisecondsTime;
        int dotsAddElapsedTime;

        Vector2 textSize;
        Vector2 dotsSize;
        Vector2 dotsPosition;
        Vector2 textPosition;

        public float manaCost;

        int configurationMillisecondsTime;
        int configurationElapsedTime;

        public RoutesConfiguration(Texture2D appearanceSmokeTexture)
        {
            text = "changing routes";
            dots = ".";
            configurationMillisecondsTime = 5000;
            configurationElapsedTime = 0;
            dotsAddMillisecondsTime = 100;
            dotsAddElapsedTime = 0;
            manaCost = 50;
            textSize = Game1.font18.MeasureString(text);
            dotsSize = Game1.font18.MeasureString(dots);
            appearanceSmokes = new List<Animation>();
            this.appearanceSmokeTexture = appearanceSmokeTexture;
        }

        public void Update(GameTime gameTime, Goat goat)
        {
            dotsPosition = new Vector2(goat.positionX + (goat.width / 2), goat.TopOfFirstMeasureBar() - dotsSize.Y - 5);
            textPosition = new Vector2(goat.positionX + (goat.width / 2), dotsPosition.Y - 15);

            dotsAddElapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (dotsAddElapsedTime > dotsAddMillisecondsTime)
            {
                dots += ".";
                if (dots.Length > 5)
                    dots = ".";
                dotsSize = Game1.font18.MeasureString(dots);
                dotsAddElapsedTime = 0;
            }

            configurationElapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (configurationElapsedTime > configurationMillisecondsTime)
            {
                FinishConfiguration(goat);
            }
        }

        public void FinishConfiguration(Goat goat)
        {
            Random random = new Random();

            foreach (Player p in Game1.players)
            {
                if (p != goat && !p.playerIsDead)
                {
                    float initialPositionX = (int)p.positionX;
                    float initialPositionY = (int)p.positionY;

                    float newPositionX = random.Next(0, Game1.screenWidth - (p.width) - 1);
                    float newPositionY = random.Next(0, Game1.screenHeight - (p.height) - 1);

                    p.positionX = newPositionX;
                    p.positionY = newPositionY;

                    p.AdjustPositionInCaseOfTilesIntersection(initialPositionX, initialPositionY);

                    Animation appearanceSmokeAnimation = new Animation();
                    appearanceSmokeAnimation.Initialize(appearanceSmokeTexture,
                                                    new Vector2(p.positionX - ((57 - p.width) / 2),
                                                    p.positionY - ((57 - p.height) / 2)), 57, 57, 0, 8, 40, Color.White, 1f, true);

                    appearanceSmokes.Add(appearanceSmokeAnimation);
                }
            }

            goat.sp -= manaCost;
            goat.configuringRoutes = false;

            configurationElapsedTime = 0;

            dots = ".";
        }

        public void DrawTexts(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Game1.font18, dots, dotsPosition, Color.White, 0, new Vector2(dotsSize.X / 2, 0), 1f, SpriteEffects.None, 0);
            spriteBatch.DrawString(Game1.font18, text, textPosition, Color.White, 0, new Vector2(textSize.X / 2, 0), 1f, SpriteEffects.None, 0);
        }

        public void DrawAnimations(SpriteBatch spriteBatch)
        {
            foreach (Animation a in appearanceSmokes)
            {
                a.Draw(spriteBatch);
            }
        }
    }
}

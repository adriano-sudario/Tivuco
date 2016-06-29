using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Round
    {
        Texture2D roundBoxTexture;
        Texture2D roundCharactersTexture;
        Rectangle roundBoxRectangle;
        Rectangle roundCharactersDestinationRectangle;
        Rectangle roundCharactersSourceRectangle;
        RoundCount[] roundCounts;
        RoundCount roundCountBlinking;
        RoundCount lastRoundWon;

        int roundsWon;

        int roundCharactersWidth;

        Selection.Character character;
        public string playerID;
        Vector2 playerIDSize;
        Vector2 playerIDPosition;

        public Round(Texture2D roundBox, Texture2D roundCount, Texture2D roundCharacters, 
                     int roundCountWidth, int roundCharactersWidth, Vector2 roundBoxPosition, string playerID,
                     Selection.Character character)
        {
            this.character = character;
            this.roundBoxTexture = roundBox;
            this.roundCharactersTexture = roundCharacters;
            this.playerID = playerID;
            this.roundCharactersWidth = roundCharactersWidth;
            playerIDSize = Game1.font36.MeasureString(playerID);
            roundsWon = 0;
            SetRoundBoxPosition(roundBoxPosition);
            SetRoundCharactersRectangle();
            SetPlayerTextPosition();
            roundCounts = new RoundCount[Result.maxWins];
            SetRoundsPosition(roundCount, roundCountWidth);
        }

        public void SetRoundBoxPosition(Vector2 roundBoxPosition)
        {
            roundBoxRectangle = new Rectangle((int)roundBoxPosition.X, (int)roundBoxPosition.Y, roundBoxTexture.Width, roundBoxTexture.Height);
        }

        public void SetRoundCharactersRectangle()
        {
            roundCharactersDestinationRectangle = new Rectangle((int)roundBoxRectangle.X + 35,
                                                                (int)roundBoxRectangle.Y + ((int)roundBoxRectangle.Height / 2) - (roundCharactersTexture.Height / 2),
                                                                roundCharactersWidth, roundCharactersTexture.Height);

            roundCharactersSourceRectangle = new Rectangle(roundCharactersWidth * (int)character, 0,
                                                                roundCharactersWidth, roundCharactersTexture.Height);
        }

        public void SetPlayerTextPosition()
        {
            playerIDPosition = new Vector2((float)roundCharactersDestinationRectangle.X + roundCharactersWidth + 15, roundCharactersDestinationRectangle.Y + (roundCharactersTexture.Height / 2));
        }

        public void SetRoundsPosition(Texture2D roundCountTexture, int roundCountWidth)
        {
            int startingX = (int)playerIDPosition.X + (int)playerIDSize.X + 20;

            for (int i = 0; i < Result.maxWins; i++)
            {
                Rectangle destination = new Rectangle(startingX,
                                                      roundCharactersDestinationRectangle.Y + (roundCharactersDestinationRectangle.Height / 2) - (roundCountTexture.Height / 2),
                                                      roundCountWidth, roundCountTexture.Height);

                roundCounts[i] = new RoundCount(roundCountTexture, roundCountWidth, destination);

                startingX += roundCountWidth + 10;
            }
        }

        public void SetPlayerAsWinner()
        {
            if (Result.roundWon != null)
            {
                if (Result.roundWon.lastRoundWon != null)
                {
                    Result.roundWon.lastRoundWon.MarkAsWon();
                }
            }

            roundsWon++;

            roundCountBlinking = roundCounts[roundsWon - 1];

            roundCountBlinking.Blinking = true;
            Result.roundWon = this;
            Result.roundWon.lastRoundWon = roundCountBlinking;

            if (roundsWon == Result.maxWins)
                Result.SetWinner(playerID, character);
        }

        public void Update(GameTime gameTime)
        {
            roundCountBlinking.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(roundBoxTexture, roundBoxRectangle, Color.White);
            spriteBatch.DrawString(Game1.font36, playerID + ":", playerIDPosition, Color.White, 0, new Vector2(0, playerIDSize.Y / 2), 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(roundCharactersTexture, roundCharactersDestinationRectangle, roundCharactersSourceRectangle, Color.White);

            foreach (RoundCount rc in roundCounts)
            {
                rc.Draw(spriteBatch);
            }
        }

        public class RoundCount
        {
            Texture2D roundCountTexture;
            public Rectangle roundCountDestinationRectangle;
            Rectangle roundCountSourceRectangle;

            int roundCountWidth;

            bool blinked;

            int elapsedTime;
            int blinkMillisecondTime;

            public bool Blinking
            {
                set
                {
                    blinked = true;
                    roundCountSourceRectangle.X = roundCountWidth;
                }
            }

            public RoundCount(Texture2D roundCountTexture, int roundCountWidth, Rectangle roundCountDestinationRectangle)
            {
                this.roundCountTexture = roundCountTexture;
                this.roundCountDestinationRectangle = roundCountDestinationRectangle;
                roundCountSourceRectangle = new Rectangle(0, 0, roundCountWidth, roundCountTexture.Height);
                this.roundCountWidth = roundCountWidth;
                blinked = false;
                elapsedTime = 0;
                blinkMillisecondTime = 250;
            }

            public void MarkAsWon()
            {
                roundCountSourceRectangle.X = roundCountWidth;
            }

            public void Update(GameTime gameTime)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;

                if (elapsedTime > blinkMillisecondTime)
                {
                    if (blinked)
                    {
                        roundCountSourceRectangle.X = roundCountWidth;
                        blinked = false;
                    }
                    else
                    {
                        roundCountSourceRectangle.X = 0;
                        blinked = true;
                    }

                    elapsedTime = 0;
                }
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(roundCountTexture, roundCountDestinationRectangle, roundCountSourceRectangle, Color.White);
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Result
    {
        Texture2D victory;
        Rectangle victoryDestinationRectangle;
        static Rectangle victorySourceRectangle;

        public List<Round> rounds;
        public static Round roundWon;
        public static Round lastPlayerRoundWon;

        int victoryFrameWidth;

        public static bool matchEnded;
        public static string winnerID;
        public static int maxWins;

        static Vector2 winnerIDSize;

        public Result(Texture2D victory, int victoryFrameWidth, int maxWins)
        {
            rounds = new List<Round>();
            this.victory = victory;
            Result.maxWins = maxWins;
            matchEnded = false;
            this.victoryFrameWidth = victoryFrameWidth;
            victoryDestinationRectangle = new Rectangle((Game1.screenWidth / 2) - (victoryFrameWidth / 2),
                                                        (Game1.screenHeight / 2) - (victory.Height / 2),
                                                        victoryFrameWidth, victory.Height);

            victorySourceRectangle = new Rectangle(0, 0, victoryFrameWidth, victory.Height);
        }

        public void AddRound(Texture2D roundBox, Texture2D roundCount, Texture2D roundCharacters, 
                     int roundCountWidth, int roundCharactersWidth, Vector2 roundBoxPosition, string playerID,
                     Selection.Character character)
        {
            Round round = new Round(roundBox, roundCount, roundCharacters, roundCountWidth, roundCharactersWidth,
                                    roundBoxPosition, playerID, character);

            rounds.Add(round);
        }

        public static void SetWinner(string winner, Selection.Character type)
        {
            Result.victorySourceRectangle.X += victorySourceRectangle.Width * (int)type;
            winnerID = winner + " wins!";
            winnerIDSize = Game1.font36.MeasureString(winnerID);
            matchEnded = true;
        }

        public void SetRoundWinner(string winner)
        {
            foreach (Round r in rounds)
            {
                if (r.playerID == winner)
                {
                    r.SetPlayerAsWinner();
                    break;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            roundWon.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Game1.screen == Game1.Screen.MatchStatistics)
            {
                foreach (Round r in rounds)
                {
                    r.Draw(spriteBatch);
                }
            }
            else if (Game1.screen == Game1.Screen.Result)
            {
                spriteBatch.Draw(victory, victoryDestinationRectangle, victorySourceRectangle, Color.White);
                spriteBatch.DrawString(Game1.font36, winnerID, new Vector2(victoryDestinationRectangle.X + (victoryDestinationRectangle.Width / 2) - (winnerIDSize.X / 2),
                                                                           victoryDestinationRectangle.Y + victoryDestinationRectangle.Height - 50), Color.White);
            }
        }
    }
}

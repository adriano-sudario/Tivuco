using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Goat : Player
    {
        Texture2D disconnectionTexture;
        Love love;
        RoutesConfiguration routesConfiguration;
        List<Projectile> disconnections;

        float disconnectionManaCost;

        public bool configuringRoutes;

        public GoatActions goatActions;

        public enum GoatActions { None, InLove, ReleasingDisillusionRays, ThrowingDisconnection }

        public Goat(Animation animation, Animation deadAnimation, Aim aim, Texture2D measureBar,
                      string id, int maxHp, int maxSp, float spRegen, float move, float maximumFallingSpeed,
                      Vector2 distanceToHand, Side side, Inputs inputs,
                      Texture2D disconnectionTexture, Love love, RoutesConfiguration routesConfiguration)
            : base(animation, deadAnimation, aim, measureBar, id, maxHp, maxSp, spRegen, move, maximumFallingSpeed,
                   distanceToHand, side, inputs)
        {
            this.disconnectionTexture = disconnectionTexture;
            this.love = love;
            this.routesConfiguration = routesConfiguration;
            disconnections = new List<Projectile>();
            goatActions = GoatActions.None;
            disconnectionManaCost = 25;
            configuringRoutes = false;
        }

        public override void ResetPlayer()
        {
            base.ResetPlayer();
            goatActions = GoatActions.None;
            configuringRoutes = false;
        }

        public override void Update(GameTime gameTime)
        {
            inputs.GetInputs();
            RespawnPlayer();

            if (playerIsDead && stopUpdating)
            {
                return;
            }
            else if (playerIsDead)
            {
                disconnections.Clear();

                UpdateDeadPlayer(gameTime, 4, 4, 6);

                return;
            }

            if (goatActions != GoatActions.ReleasingDisillusionRays)
            {
                CheckInputs(gameTime);

                if (goatActions != GoatActions.InLove && !configuringRoutes)
                {
                    CheckGoatInputs();
                }
            }

            if (routesConfiguration.appearanceSmokes.Count > 0)
            {
                for (int i = routesConfiguration.appearanceSmokes.Count - 1; i > -1; i--)
                {
                    if (routesConfiguration.appearanceSmokes[i].LastFrame())
                    {
                        routesConfiguration.appearanceSmokes.Remove(routesConfiguration.appearanceSmokes[i]);
                        continue;
                    }

                    routesConfiguration.appearanceSmokes[i].Update(gameTime);
                }
            }

            love.Update(gameTime, this);

            if (configuringRoutes)
                routesConfiguration.Update(gameTime, this);

            if (goatActions == GoatActions.ThrowingDisconnection && playerAnimation.LastFrame())
            {
                ThrowDisconnection();
            }

            if (disconnections.Count > 0)
            {
                UpdateDisconnections(gameTime);
            }

            base.Update(gameTime);

            if (aiming)
                aim.Update(gameTime, side);
        }

        public void ThrowDisconnection()
        {
            Animation disconnectionAnimation = new Animation();
            disconnectionAnimation.Initialize(disconnectionTexture, new Vector2(0, 0), 16, 14, 0, 0, 100, Color.White, 1f, true);

            Projectile disconnection = new Projectile(disconnectionAnimation, 0f, 20, 8);

            if (!aiming)
                inputs.SetDefaultAim(aim);

            float xValue = 0;

            if (side == Side.Right)
            {
                xValue = positionX + width - distanceToHand.X;
            }
            else
            {
                xValue = positionX + distanceToHand.X;
            }

            float yValue = positionY + distanceToHand.Y - (disconnection.height / 2);

            disconnection.PrepareToThrow(xValue, yValue, aim.DegreeToRadian(aim.angleInDegrees), side);

            disconnections.Add(disconnection);

            ChangeGoatAction(GoatActions.None);
        }

        public void CheckForXBlocks(Tile tile, Projectile proj)
        {
            Rectangle nextPositionXRectangle = new Rectangle((int)positionX + (int)proj.IncrementX,
                                                    (int)positionY, width, height);

            if (nextPositionXRectangle.Intersects(tile.rectangle))
            {
                if (proj.directionX == Projectile.DirectionX.Left)
                {
                    proj.positionX = tile.rectangle.Right;
                }
                else if (proj.directionX == Projectile.DirectionX.Right)
                {
                    proj.positionX = tile.rectangle.Left - width;
                }

                proj.IncrementX *= -1;

                proj.timesHitTiles++;

                if (proj.timesHitTiles > 8)
                {
                    disconnections.Remove(proj);
                }
            }
        }

        public void CheckForYBlocks(Tile tile, Projectile proj)
        {
            Rectangle nextPositionYRectangle = new Rectangle((int)positionX,
                                                    (int)positionY + (int)proj.IncrementX, width, height);

            if (nextPositionYRectangle.Intersects(tile.rectangle))
            {
                if (proj.directionY == Projectile.DirectionY.Down)
                {
                    proj.positionY = tile.rectangle.Top - height;
                }
                else if (proj.directionY == Projectile.DirectionY.Up)
                {
                    proj.positionY = tile.rectangle.Bottom;
                }

                proj.IncrementY *= -1;

                proj.timesHitTiles++;

                if (proj.timesHitTiles > 8)
                {
                    disconnections.Remove(proj);
                }
            }
        }

        public void UpdateDisconnections(GameTime gameTime)
        {
            for (int i = disconnections.Count - 1; i > -1; i--)
            {
                foreach (Tile t in Game1.tiles)
                {
                    disconnections[i].CheckForBlocks(t);
                }

                if (disconnections[i].leftBlock != null || disconnections[i].rightBlock != null ||
                    disconnections[i].downBlock != null || disconnections[i].upBlock != null)
                {
                    if (disconnections[i].downBlock != null || disconnections[i].upBlock != null)
                    {
                        disconnections[i].IncrementY *= -1;

                        disconnections[i].AdjustPositionAfterBlock(gameTime);

                        disconnections[i].timesHitTiles++;

                        if (disconnections[i].timesHitTiles > 8)
                        {
                            disconnections.Remove(disconnections[i]);
                            continue;
                        }
                    }

                    if (disconnections[i].leftBlock != null || disconnections[i].rightBlock != null)
                    {
                        disconnections[i].IncrementX *= -1;

                        disconnections[i].AdjustPositionAfterBlock(gameTime);

                        disconnections[i].timesHitTiles++;

                        if (disconnections[i].timesHitTiles > 8)
                        {
                            disconnections.Remove(disconnections[i]);
                            continue;
                        }
                    }
                }
                else
                {
                    disconnections[i].Update(gameTime);
                }

                bool playerHit = false;

                foreach (Player p in Game1.players)
                {
                    if (p != this)
                    {
                        if (disconnections[i].rectangle.Intersects(p.body) && !p.playerIsDead)
                        {
                            p.hp -= disconnections[i].damage;
                            disconnections.Remove(disconnections[i]);
                            playerHit = true;
                            break;
                        }
                    }
                }

                if (playerHit)
                    continue;

                if (disconnections[i].positionX < 0 || disconnections[i].positionX > Game1.screenWidth ||
                    disconnections[i].positionY < 0 || disconnections[i].positionY > Game1.screenHeight)
                {
                    disconnections.Remove(disconnections[i]);
                }
            }
        }

        public void CheckGoatInputs()
        {
            if (inputs.FirstSkillClicked() && sp > disconnectionManaCost)
            {
                //SoundsLibrary.PlaySound(SoundsLibrary.goatThrowingDisconnectionSound);

                ChangeGoatAction(GoatActions.ThrowingDisconnection);
                sp -= disconnectionManaCost;
            }

            if (inputs.SecondSkillClicked() && sp > love.manaCost)
            {
                if (!love.affectedByLoveInvisibility)
                {
                    ChangeGoatAction(GoatActions.InLove);
                }
                else
                {
                    love.ReleaseDisillusionRays(this);
                }
            }

            if (inputs.SecondSkillClicked())
            {
                if (love.affectedByLoveInvisibility)
                {
                    //SoundsLibrary.PlaySound(SoundsLibrary.goatThrowingDisconnectionSound);
                    love.ReleaseDisillusionRays(this);
                }
            }

            if (inputs.ThirdSkillClicked() && sp > routesConfiguration.manaCost)
            {
                //SoundsLibrary.PlaySound(SoundsLibrary.goatConfiguringRouteSound);
                configuringRoutes = true;
            }
        }

        public void ChangeGoatAction(GoatActions action)
        {
            if (goatActions != action)
            {
                goatActions = action;
                changeAnimation = true;
            }
        }

        public override void ChangeAnimation()
        {
            if (!changeAnimation)
                return;

            switch (goatActions)
            {
                case GoatActions.ThrowingDisconnection:
                    playerAnimation.firstFrame = 4;
                    playerAnimation.lastFrame = 5;
                    break;

                case GoatActions.ReleasingDisillusionRays:
                    playerAnimation.firstFrame = 3;
                    playerAnimation.lastFrame = 3;
                    break;

                default:
                    base.ChangeAnimation();
                    break;
            }

            playerAnimation.currentFrame = playerAnimation.firstFrame;
            changeAnimation = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!love.affectedByLoveInvisibility || (love.affectedByLoveInvisibility && playerIsDead))
                base.Draw(spriteBatch);

            if (configuringRoutes)
                routesConfiguration.DrawTexts(spriteBatch);
        }

        public override void DrawSkills(SpriteBatch spriteBatch)
        {
            love.Draw(spriteBatch, this);

            if (disconnections.Count > 0)
            {
                foreach (Projectile disconnect in disconnections)
                {
                    disconnect.Draw(spriteBatch);
                }
            }

            if (routesConfiguration.appearanceSmokes.Count > 0)
            {
                routesConfiguration.DrawAnimations(spriteBatch);
            }
        }
    }
}

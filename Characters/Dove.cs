using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Dove : Player
    {
        Animation sandwich;
        Animation stinkAnimation;
        List<Poo> poos;
        Texture2D pooTexture;
        Projectile fork;
        bool forkThrown;
        bool forkStopped;
        bool forkLeftPlayer;
        float lastPooHeight;
        public float doveLastPositionY;

        public float sandwichPositionX
        {
            get
            {
                return sandwich.position.X;
            }
            set
            {
                if (side == Side.Right)
                {
                    sandwich.position.X = value + width - distanceToHand.X - 1;
                    sandwich.destinationRect.X = (int)value + width - (int)distanceToHand.X - 1;
                }
                else
                {
                    sandwich.position.X = value + distanceToHand.X - sandwich.frameWidth + 1;
                    sandwich.destinationRect.X = (int)value + (int)distanceToHand.X - sandwich.frameWidth + 1;
                }
            }
        }

        public float sandwichPositionY
        {
            get
            {
                return sandwich.position.Y;
            }
            set
            {
                sandwich.position.Y = value + distanceToHand.Y - sandwich.frameHeight - 3;
                sandwich.destinationRect.Y = (int)value + (int)distanceToHand.Y - sandwich.frameHeight - 3;
            }
        }

        DoveActions doveActions;

        public enum DoveActions { None, Eating, Pooing, ThrowingFork }

        public Dove(Animation animation, Animation deadAnimation, Aim aim, Texture2D measureBar,
                    string id, int maxHp, int maxSp, float spRegen, float move, float maximumFallingSpeed,
                    Vector2 distanceToHand, Side side, Inputs inputs,
                    Animation sandwich, Texture2D pooTexture, int maxNumberOfPoos, Animation stinkAnimation, Projectile fork)
            : base(animation, deadAnimation, aim, measureBar, id, maxHp, maxSp, spRegen, move, maximumFallingSpeed,
                   distanceToHand, side, inputs)
        {
            doveActions = DoveActions.None;
            this.sandwich = sandwich;
            poos = new List<Poo>();
            this.fork = fork;
            ResetFork();
            this.pooTexture = pooTexture;
            maxSkillValue = maxNumberOfPoos;
            this.stinkAnimation = stinkAnimation;
        }

        public override void ResetPlayer()
        {
            base.ResetPlayer();
            doveActions = DoveActions.None;
            poos.Clear();
            ResetFork();
        }

        public void ResetFork()
        {
            forkThrown = false;
            forkStopped = false;
            forkLeftPlayer = false;
        }

        public void ThrowFork()
        {
            if (!aiming)
                inputs.SetDefaultAim(aim);

            float xValue = 0;

            if (side == Side.Right)
            {
                xValue = positionX + width - distanceToHand.X - 4;
            }
            else
            {
                xValue = positionX + distanceToHand.X + 4;
            }

            float yValue = positionY + distanceToHand.Y - (fork.height / 2);

            fork.PrepareToThrow(xValue, yValue, aim.DegreeToRadian(aim.angleInDegrees), side);

            forkThrown = true;
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
                UpdateDeadPlayer(gameTime, 4, 6, 12);

                return;
            }

            if (doveActions != DoveActions.Pooing)
            {
                CheckInputs(gameTime);
                if (doveActions != DoveActions.Eating)
                    CheckDoveInputs();

                base.Update(gameTime);
            }
            else
            {
                AdjustMeasureBarRectangles();

                if (positionY + height > lastPooHeight)
                {
                    positionY -= 3f;
                }
                else
                {
                    positionY = doveLastPositionY;
                    ChangeDoveAction(DoveActions.None);
                }
            }

            if (doveActions == DoveActions.Eating || doveActions == DoveActions.Pooing)
            {
                aiming = false;
            }

            if (doveActions == DoveActions.ThrowingFork && playerAnimation.LastFrame())
            {
                ThrowFork();
                ChangeDoveAction(DoveActions.None);
            }

            if (forkThrown)
                UpdateFork(gameTime);

            if (doveActions == DoveActions.Eating)
            {
                sandwichPositionX = positionX;
                sandwichPositionY = positionY;

                if (sandwich.LastFrame())
                {
                    skillMeasureBarVisible = true;

                    if (skillValue < maxSkillValue)
                        skillValue += 1;

                    spRegen = defaultSpRegen;

                    ChangeDoveAction(DoveActions.None);

                    sandwich.currentFrame = sandwich.firstFrame;

                    hp += 5;

                    if (hp > maxHp)
                    {
                        hp = maxHp;
                    }
                }
                else
                {
                    sandwich.Update(gameTime);
                }
            }

            if (aiming)
                aim.Update(gameTime, side);

            if (poos.Count > 0)
                UpdatePoos(gameTime);
        }

        public void UpdateFork(GameTime gameTime)
        {
            if (forkStopped)
            {
                if (fork.rectangle.Intersects(body))
                {
                    ResetFork();
                }

                return;
            }

            foreach (Tile t in Game1.tiles)
            {
                fork.CheckForBlocks(t);
            }

            if (fork.downBlock != null)
            {
                forkStopped = true;

                fork.AdjustPositionAfterBlock(gameTime);
            }
            else if (fork.upBlock != null || fork.leftBlock != null || fork.rightBlock != null)
            {
                fork.IncrementX = 0;
                fork.IncrementY = fork.force;

                fork.AdjustPositionAfterBlock(gameTime);
            }
            else
            {
                fork.Update(gameTime);
            }

            foreach (Player p in Game1.players)
            {
                if (p != this)
                {
                    if (fork.rectangle.Intersects(p.body) && !forkStopped && !p.playerIsDead)
                    {
                        p.hp -= fork.damage;
                        ResetFork();
                        break;
                    }
                }
                else
                {
                    if (!fork.rectangle.Intersects(p.body) && !forkLeftPlayer)
                    {
                        forkLeftPlayer = true;
                    }
                    else if (fork.rectangle.Intersects(p.body) && forkLeftPlayer)
                    {
                        ResetFork();
                    }
                }
            }

            if (fork.positionX < 0 || fork.positionX > Game1.screenWidth ||
                fork.positionY < 0 || fork.positionY > Game1.screenHeight)
            {
                ResetFork();
            }
        }

        public void UpdatePoos(GameTime gameTime)
        {
            for (int i = poos.Count - 1; i > -1; i--)
            {
                if (poos[i].infectedPlayer == null)
                {
                    foreach (Player p in Game1.players)
                    {
                        bool alreadyInfected = false;

                        foreach (Poo infected in poos)
                        {
                            if (infected.infectedPlayer == p)
                            {
                                alreadyInfected = true;
                                break;
                            }
                        }

                        if (poos[i].pooRectangle.Intersects(p.body) && p != this && !p.playerIsDead &&
                            !alreadyInfected)
                        {
                            poos[i].stinkPositionX = p.positionX;
                            poos[i].stinkPositionY = p.positionY - 5;

                            poos[i].infectedPlayer = p;

                            break;
                        }
                    }
                }
                else
                {
                    poos[i].stinkPositionX = poos[i].infectedPlayer.positionX;
                    poos[i].stinkPositionY = poos[i].infectedPlayer.positionY - 5;
                    poos[i].stinkAnimation.Update(gameTime);

                    poos[i].damageApplied += poos[i].damageDecrement;

                    if (poos[i].damageApplied > poos[i].damage)
                    {
                        poos[i].infectedPlayer.hp -= poos[i].damage - (poos[i].damageApplied - poos[i].damageDecrement);
                        poos[i].infectedPlayer.hp -= poos[i].damageDecrement;
                        poos.Remove(poos[i]);
                        continue;
                    }

                    poos[i].infectedPlayer.hp -= poos[i].damageDecrement;
                }
            }
        }

        public void CheckDoveInputs()
        {
            if (inputs.FirstSkillClicked())
            {
                if (!forkThrown)
                {
                    ChangeDoveAction(DoveActions.ThrowingFork);
                }
                else if (sp > 80)
                {
                    ResetFork();

                    sp -= 80;
                }
            }

            if (inputs.SecondSkillClicked())
            {
                if (doveActions == DoveActions.None)
                {
                    sandwichPositionX = positionX;
                    sandwichPositionY = positionY;

                    sp -= 25;
                    spRegen = 0;

                    ChangeDoveAction(DoveActions.Eating);
                }
            }

            if (inputs.ThirdSkillClicked())
            {
                if (doveActions == DoveActions.None && onGround != null && skillValue > 0 && poos.Count < 6 && sp > 50)
                {
                    Poo poo = new Poo(pooTexture, 60, 0.3f, stinkAnimation);

                    poo.pooRectangle = new Rectangle(0, 0, poo.poo.Width, poo.poo.Height);

                    if (side == Side.Right)
                    {
                        poo.pooPositionX = (int)positionX + (width / 2) - (poo.poo.Width / 2) + 4;
                    }
                    else
                    {
                        poo.pooPositionX = (int)positionX + 6 + (poo.poo.Width / 2);
                    }
                    
                    poo.pooPositionY = (int)positionY + height - poo.poo.Height;

                    doveLastPositionY = positionY;

                    lastPooHeight = poo.pooPositionY;

                    poos.Add(poo);

                    skillValue -= 1;

                    sp -= 50;

                    if (skillValue < 1)
                        skillMeasureBarVisible = false;

                    ChangeDoveAction(DoveActions.Pooing);
                }
            }
        }

        public void ChangeDoveAction(DoveActions action)
        {
            if (doveActions != action)
            {
                doveActions = action;
                changeAnimation = true;
            }
        }

        public override void ChangeAnimation()
        {
            if (!changeAnimation)
                return;

            switch (doveActions)
            {
                case DoveActions.None:
                    base.ChangeAnimation();
                    break;

                case DoveActions.Eating:
                    switch (action)
                    {
                        case PlayerAction.Idle:
                            playerAnimation.firstFrame = 6;
                            playerAnimation.lastFrame = 6;
                            break;

                        case PlayerAction.Walking:
                            playerAnimation.firstFrame = 7;
                            playerAnimation.lastFrame = 8;
                            break;

                        case PlayerAction.Jumping:
                            playerAnimation.firstFrame = 9;
                            playerAnimation.lastFrame = 9;
                            break;
                    }
                    break;

                case DoveActions.ThrowingFork:
                    playerAnimation.firstFrame = 4;
                    playerAnimation.lastFrame = 5;
                    break;

                case DoveActions.Pooing:
                    playerAnimation.firstFrame = 10;
                    playerAnimation.lastFrame = 10;
                    break;
            }

            playerAnimation.currentFrame = playerAnimation.firstFrame;
            changeAnimation = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            bool isRight;

            if (side == Player.Side.Right)
                isRight = true;
            else
                isRight = false;

            if (poos.Count > 0)
            {
                foreach (Poo p in poos)
                {
                    if (p.infectedPlayer == null)
                        p.Draw(spriteBatch, isRight);
                }
            }

            base.Draw(spriteBatch);

            if (doveActions == DoveActions.Eating)
            {
                sandwich.Draw(spriteBatch, isRight);
            }
        }

        public override void DrawSkills(SpriteBatch spriteBatch)
        {
            bool isRight;

            if (side == Player.Side.Right)
                isRight = true;
            else
                isRight = false;

            if (poos.Count > 0)
            {
                foreach (Poo p in poos)
                {
                    if (p.infectedPlayer != null)
                        p.Draw(spriteBatch, isRight);
                }
            }

            if (forkThrown)
                fork.Draw(spriteBatch);
        }
    }
}

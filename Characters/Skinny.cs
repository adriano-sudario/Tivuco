using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Skinny : Player
    {
        Animation spaceshipAnimation;
        Texture2D spaceshipShootTexture;
        Texture2D crazyRayTexture;
        List<SpaceshipShoot> spaceshipShoots;
        List<Projectile> crazyRays;

        Teleport teleport;

        float spaceshipManaDecrement;

        public SkinnyActions skinnyActions;

        public enum SkinnyActions { None, Teleporting, OnSpaceship, ThrowingCrazyRay }

        public Skinny(Animation animation, Animation deadAnimation, Aim aim, Texture2D measureBar,
                      string id, int maxHp, int maxSp, float spRegen, float move, float maximumFallingSpeed,
                      Vector2 distanceToHand, Side side, Inputs inputs,
                      Animation spaceshipAnimation, Texture2D spaceshipShootTexture, Texture2D crazyRayTexture,
                      Teleport teleport, float spaceshipManaDecrement)
            : base(animation, deadAnimation, aim, measureBar, id, maxHp, maxSp, spRegen, move, maximumFallingSpeed,
                   distanceToHand, side, inputs)
        {
            skinnyActions = SkinnyActions.None;
            this.spaceshipAnimation = spaceshipAnimation;
            this.spaceshipShootTexture = spaceshipShootTexture;
            this.crazyRayTexture = crazyRayTexture;
            this.teleport = teleport;
            this.spaceshipManaDecrement = spaceshipManaDecrement;
            spaceshipShoots = new List<SpaceshipShoot>();
            crazyRays = new List<Projectile>();
        }

        public override void ResetPlayer()
        {
            base.ResetPlayer();
            skinnyActions = SkinnyActions.None;
            spaceshipShoots.Clear();
            crazyRays.Clear();
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
                UpdateDeadPlayer(gameTime, 4, 4, 12);

                return;
            }

            if (skinnyActions != SkinnyActions.OnSpaceship)
            {
                CheckInputs(gameTime);
                CheckSkinnyInputs();

                teleport.Update(gameTime);

                base.Update(gameTime);

                if (aiming)
                    aim.Update(gameTime, side);

                if (skinnyActions == SkinnyActions.ThrowingCrazyRay && playerAnimation.LastFrame())
                {
                    Animation crazyRayAnimation = new Animation();
                    crazyRayAnimation.Initialize(crazyRayTexture, new Vector2(0, 0), 9, 9, 0, 4, 100, Color.White, 1f, true);

                    Projectile crazyRay = new Projectile(crazyRayAnimation, 0f, 25, 8);

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

                    float yValue = positionY + distanceToHand.Y - (crazyRay.height / 2);

                    crazyRay.PrepareToThrow(xValue, yValue, aim.DegreeToRadian(aim.angleInDegrees), side);

                    crazyRays.Add(crazyRay);

                    ChangeSkinnyAction(SkinnyActions.None);
                }
            }
            else
            {
                positionX = MathHelper.Clamp(positionX, 0, Game1.screenWidth - (width) - 1);
                positionY = MathHelper.Clamp(positionY, 0, Game1.screenHeight - (height) - 1);

                CheckIfDead();

                if (playerIsDead)
                {
                    return;
                }

                CheckSpaceShipInputs();

                if (sp <= 0)
                {
                    RestoreDefaultAnimation(PlayerAction.Idle);
                    ChangeSkinnyAction(SkinnyActions.None);
                }

                if (skinnyActions == SkinnyActions.OnSpaceship)
                {
                    CheckForSpaceshipTilesIntersections();

                    AdjustMeasureBarRectangles();

                    playerAnimation.Update(gameTime);
                }
                else
                {
                    base.Update(gameTime);
                }
            }

            if (crazyRays.Count > 0)
                UpdateCrazyRays(gameTime);

            if (spaceshipShoots.Count > 0)
            {
                for (int i = spaceshipShoots.Count - 1; i > -1; i--)
                {
                    spaceshipShoots[i].Update();

                    Player playerHit = spaceshipShoots[i].CheckPlayersCollision(this);

                    if (playerHit != null)
                    {
                        playerHit.hp -= spaceshipShoots[i].shootDamage;

                        spaceshipShoots.Remove(spaceshipShoots[i]);

                        break;
                    }

                    if (spaceshipShoots[i].CollidedTile())
                    {
                        spaceshipShoots.Remove(spaceshipShoots[i]);
                    }
                }
            }
        }

        public void UpdateCrazyRays(GameTime gameTime)
        {
            for (int i = crazyRays.Count - 1; i > -1; i--)
            {
                crazyRays[i].Update(gameTime);

                if (crazyRays[i].positionX < 0 || crazyRays[i].positionX > Game1.screenWidth)
                {
                    crazyRays.Remove(crazyRays[i]);
                    continue;
                }
                else if (crazyRays[i].positionY < 0 || crazyRays[i].positionY > Game1.screenHeight)
                {
                    crazyRays.Remove(crazyRays[i]);
                    continue;
                }

                foreach (Player p in Game1.players)
                {
                    if (p != this)
                    {
                        if (crazyRays[i].rectangle.Intersects(p.body) && !p.playerIsDead)
                        {
                            p.hp -= crazyRays[i].damage;
                            crazyRays.Remove(crazyRays[i]);
                            break;
                        }
                    }
                }
            }
        }

        public void CheckForSpaceshipTilesIntersections()
        {
            foreach (Tile t in Game1.tiles)
            {
                if (t.rectangle.Intersects(down))
                {
                    positionY = t.rectangle.Y - height;
                }
                else if (t.rectangle.Intersects(up))
                {
                    positionY = t.rectangle.Y + t.rectangle.Height;
                }
                else if (t.rectangle.Intersects(left))
                {
                    positionX = t.rectangle.X + t.rectangle.Width;
                }
                else if (t.rectangle.Intersects(right))
                {
                    positionX = t.rectangle.X - width;
                }
            }
        }

        public void CheckSpaceShipInputs()
        {
            float moveXValue = inputs.MoveX();
            float moveYValue = inputs.MoveY();

            if (moveXValue != 0)
            {
                FloatX(moveXValue);
                sp -= spaceshipManaDecrement;
            }

            if (moveYValue != 0)
            {
                FloatY(moveYValue);
                sp -= spaceshipManaDecrement;
            }

            if (inputs.FirstSkillClicked() && sp > spaceshipManaDecrement * 10f)
            {
                SpaceshipShoot spaceShoot = new SpaceshipShoot(spaceshipShootTexture, this);

                spaceshipShoots.Add(spaceShoot);

                sp -= spaceshipManaDecrement * 10f;
            }

            if (inputs.ThirdSkillClicked())
            {
                RestoreDefaultAnimation(PlayerAction.Idle);
                ChangeSkinnyAction(SkinnyActions.None);
            }
        }

        public void FloatX(float multiplier)
        {
            positionX += (movement * multiplier);
        }

        public void FloatY(float multiplier)
        {
            positionY += (movement * multiplier);
        }

        public void CheckSkinnyInputs()
        {
            if (inputs.FirstSkillClicked() && sp >= 20)
            {
                ChangeSkinnyAction(SkinnyActions.ThrowingCrazyRay);
                sp -= 20;
            }

            if (inputs.SecondSkillClicked() && sp >= 75)
            {
                ChangeSkinnyAction(SkinnyActions.Teleporting);
                sp -= 75;
            }

            if (inputs.ThirdSkillClicked() && skinnyActions != SkinnyActions.OnSpaceship)
            {
                SetNewAnimation(playerAnimation);
                ChangeSkinnyAction(SkinnyActions.OnSpaceship);
            }
        }

        //public void GetOutOfSpaceship()
        //{
        //    float positionXBackup = positionX;
        //    float positionYBackup = positionY;

        //    playerAnimation = defaultAnimation;

        //    positionX = positionXBackup;
        //    positionY = positionYBackup;

        //    ChangeSkinnyAction(SkinnyActions.None);
        //}

        public void ChangeSkinnyAction(SkinnyActions action)
        {
            if (skinnyActions != action)
            {
                skinnyActions = action;
                changeAnimation = true;
            }
        }

        public override void ChangeAnimation()
        {
            if (!changeAnimation)
                return;

            switch (skinnyActions)
            {
                case SkinnyActions.None:
                    base.ChangeAnimation();
                    break;

                case SkinnyActions.ThrowingCrazyRay:
                    playerAnimation.firstFrame = 4;
                    playerAnimation.lastFrame = 5;
                    break;

                case SkinnyActions.Teleporting:
                    if (!aiming)
                        inputs.SetDefaultAim(aim);

                    teleport.SetTeleport(this, aim.DegreeToRadian(aim.angleInDegrees), side);
                    break;

                case SkinnyActions.OnSpaceship:
                    SetNewAnimation(spaceshipAnimation);
                    onGround = null;
                    side = Side.Right;
                    force = 0;
                    break;
            }

            if (skinnyActions != SkinnyActions.Teleporting)
            {
                playerAnimation.currentFrame = playerAnimation.firstFrame;
                changeAnimation = false;
            }
            else
            {
                ChangeSkinnyAction(Skinny.SkinnyActions.None);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            bool isRight;

            if (side == Player.Side.Right)
                isRight = true;
            else
                isRight = false;

            if (spaceshipShoots.Count > 0)
            {
                foreach (SpaceshipShoot ss in spaceshipShoots)
                {
                    ss.Draw(spriteBatch);
                }
            }

            base.Draw(spriteBatch);

            teleport.Draw(spriteBatch, isRight);
        }

        public override void DrawSkills(SpriteBatch spriteBatch)
        {
            if (crazyRays.Count > 0)
            {
                foreach (Projectile cr in crazyRays)
                {
                    cr.Draw(spriteBatch);
                }
            }
        }
    }
}

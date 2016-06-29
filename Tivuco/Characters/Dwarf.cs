using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Dwarf : Player
    {
        FireSpitting fireSpitting;
        Texture2D spinningBoozeTexture;
        List<Projectile> boozes;
        float smokingSpRegen;
        float spRegenered;

        public float fireSpittingPositionX
        {
            get 
            {
                switch (dwarfActions)
                {
                    case DwarfActions.HoldingBottle:
                        return fireSpitting.boozePositionX;

                    case DwarfActions.HoldingLighter:
                        return fireSpitting.lighterPositionX;

                    case DwarfActions.SpittingFire:
                        return fireSpitting.spittingPositionX;

                    default:
                        return 0;
                }
            }
            set
            {
                switch (dwarfActions)
                {
                    case DwarfActions.HoldingBottle:
                        fireSpitting.boozePositionX = value;
                        break;

                    case DwarfActions.HoldingLighter:
                        fireSpitting.lighterPositionX = value;
                        break;

                    case DwarfActions.SpittingFire:
                        fireSpitting.spittingPositionX = value;
                        break;
                }
            }
        }

        public float fireSpittingPositionY
        {
            get
            {
                switch (dwarfActions)
                {
                    case DwarfActions.HoldingBottle:
                        return fireSpitting.boozePositionY;

                    case DwarfActions.HoldingLighter:
                        return fireSpitting.lighterPositionY;

                    case DwarfActions.SpittingFire:
                        return fireSpitting.spittingPositionY;

                    default:
                        return 0;
                }
            }
            set
            {
                switch (dwarfActions)
                {
                    case DwarfActions.HoldingBottle:
                        fireSpitting.boozePositionY = value;
                        break;

                    case DwarfActions.HoldingLighter:
                        fireSpitting.lighterPositionY = value;
                        break;

                    case DwarfActions.SpittingFire:
                        fireSpitting.spittingPositionY = value;
                        break;
                }
            }
        }

        public bool alcoholOnMouth;
        public bool drinking;

        DwarfActions dwarfActions;

        public enum DwarfActions { None, HoldingBottle, HoldingLighter, SpittingFire, Aiming, ThrowingBottle, Smoking }

        public Dwarf(Animation animation, Animation deadAnimation, Aim aim, Texture2D measureBar,
                      string id, int maxHp, int maxSp, float spRegen, float move, float maximumFallingSpeed,
                      Vector2 distanceToHand, Side side, Inputs inputs,
                      FireSpitting fireSpitting, Texture2D spinningBoozeTexture)
            : base(animation, deadAnimation, aim, measureBar, id, maxHp, maxSp, spRegen, move, maximumFallingSpeed, 
                   distanceToHand, side, inputs)
        {
            dwarfActions = DwarfActions.None;
            maxSkillValue = fireSpitting.maximumAmount;
            alcoholOnMouth = false;
            drinking = false;
            this.fireSpitting = fireSpitting;
            this.spinningBoozeTexture = spinningBoozeTexture;
            smokingSpRegen = 25f;
            spRegenered = 0;
            boozes = new List<Projectile>();
        }

        public override void ResetPlayer()
        {
            base.ResetPlayer();
            dwarfActions = DwarfActions.None;
            alcoholOnMouth = false;
            drinking = false;
            spRegenered = 0;
            boozes.Clear();
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
                UpdateDeadPlayer(gameTime, 4, 4, 9);

                return;
            }

            if (dwarfActions != DwarfActions.Smoking)
            {
                CheckInputs(gameTime);
                CheckDwarfInputs();
            }
            else
            {
                if (playerAnimation.LastFrame())
                {
                    if (spRegenered < smokingSpRegen)
                    {
                        sp += smokingSpRegen - spRegenered;
                    }

                    spRegenered = 0;
                    playerAnimation.frameTime = 100;

                    ChangeDwarfAction(DwarfActions.None);
                }
                else
                {
                    if (playerAnimation.changedFrame)
                    {
                        spRegenered += smokingSpRegen / (playerAnimation.lastFrame - playerAnimation.firstFrame);
                        sp += smokingSpRegen / (playerAnimation.lastFrame - playerAnimation.firstFrame);
                        if (sp > 100)
                        {
                            sp = 100;
                        }
                    }
                }
            }

            if (sp >= 75)
            {
                movement = 4;
            }
            else if (sp >= 50 && sp < 75)
            {
                movement = 5;
            }
            else if (sp >= 25 && sp < 50)
            {
                movement = 6;
            }
            else if (sp >= 0 && sp < 25)
            {
                movement = 7;
            }

            base.Update(gameTime);

            if (dwarfActions == DwarfActions.HoldingBottle || dwarfActions == DwarfActions.HoldingLighter ||
                     dwarfActions == DwarfActions.SpittingFire || dwarfActions == DwarfActions.Smoking)
            {
                aiming = false;
            }

            if (aiming)
                aim.Update(gameTime, side);

            if (dwarfActions == DwarfActions.ThrowingBottle && playerAnimation.LastFrame())
            {
                Animation boozeSpinAnimation = new Animation();
                boozeSpinAnimation.Initialize(spinningBoozeTexture, new Vector2(0, 0), 18, 18, 0, 7, 50, Color.White, 1f, true);

                Projectile spinningBooze = new Projectile(boozeSpinAnimation, 0.3f, 40, 10);

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

                float yValue = positionY + distanceToHand.Y - (spinningBooze.height / 2);

                spinningBooze.PrepareToThrow(xValue, yValue, aim.DegreeToRadian(aim.angleInDegrees), side);

                boozes.Add(spinningBooze);

                ChangeDwarfAction(DwarfActions.None);
            }

            if (boozes.Count > 0)
                UpdateBoozes(gameTime);

            AdjustSpittingFireAnimations(gameTime);
        }

        public void UpdateBoozes(GameTime gameTime)
        {
            for (int i = boozes.Count - 1; i > -1; i--)
            {
                boozes[i].Update(gameTime);

                if (boozes[i].positionX < 0 || boozes[i].positionX > Game1.screenWidth)
                {
                    boozes.Remove(boozes[i]);
                    continue;
                }
                else if (boozes[i].positionY < 0 || boozes[i].positionY > Game1.screenHeight)
                {
                    boozes.Remove(boozes[i]);
                    continue;
                }

                bool boozeRemoved = false;

                foreach (Player p in Game1.players)
                {
                    if (p != this)
                    {
                        if (boozes[i].rectangle.Intersects(p.body) && !p.playerIsDead)
                        {
                            p.hp -= boozes[i].damage;
                            boozes.Remove(boozes[i]);
                            boozeRemoved = true;
                            break;
                        }
                    }
                }

                if (boozeRemoved)
                    continue;

                foreach (Tile t in Game1.tiles)
                {
                    if (boozes[i].rectangle.Intersects(t.rectangle))
                    {
                        boozes.Remove(boozes[i]);
                        break;
                    }
                }
            }
        }

        public void AdjustSpittingFireAnimations(GameTime gameTime)
        {
            switch (dwarfActions)
            {
                case DwarfActions.HoldingBottle:
                    if (side == Side.Right)
                        fireSpittingPositionX = (positionX + width) - 8;
                    else
                        fireSpittingPositionX = (positionX - fireSpitting.boozeWidth) + 8;

                    fireSpittingPositionY = positionY + 24;

                    if (skillValue == maxSkillValue)
                    {
                        fireSpitting.boozeAnimation.currentFrame = 4;
                    }
                    else if (skillValue > GetValueFromPercentage(maxSkillValue, 74))
                    {
                        fireSpitting.boozeAnimation.currentFrame = 3;
                    }
                    else if (skillValue > GetValueFromPercentage(maxSkillValue, 49))
                    {
                        fireSpitting.boozeAnimation.currentFrame = 2;
                    }
                    else if (skillValue > GetValueFromPercentage(maxSkillValue, 24))
                    {
                        fireSpitting.boozeAnimation.currentFrame = 1;
                    }

                    fireSpitting.boozeAnimation.SetAnimationRectangles();

                    //fireSpitting.boozeAnimation.Update(gameTime);
                    break;

                case DwarfActions.HoldingLighter:
                    if (side == Side.Right)
                        fireSpittingPositionX = (positionX + width) - 7;
                    else
                        fireSpittingPositionX = (positionX - fireSpitting.lighterWidth) + 7;

                    fireSpittingPositionY = positionY + 23;

                    fireSpitting.lighterAnimation.Update(gameTime);
                    break;

                case DwarfActions.SpittingFire:
                    if (side == Side.Right)
                        fireSpittingPositionX = (positionX + width) - 7;
                    else
                        fireSpittingPositionX = (positionX - fireSpitting.spittingWidth) + 7;

                    fireSpittingPositionY = positionY + 8;

                    fireSpitting.spittingAnimation.Update(gameTime);
                    break;
            }
        }

        public void CheckDwarfInputs()
        {
            if (inputs.ThirdSkillHolding() && sp > fireSpitting.spDecrement)
            {
                if (!alcoholOnMouth)
                {
                    skillMeasureBarVisible = true;
                    skillValue += fireSpitting.skillValueIncrement;
                    if (skillValue > maxSkillValue)
                    {
                        skillValue = maxSkillValue;
                    }
                    else
                    {
                        sp -= fireSpitting.spDecrement;
                    }

                    drinking = true;

                    ChangeDwarfAction(DwarfActions.HoldingBottle);
                }
            }

            if (inputs.ThirdSkillReleased())
            {
                if (drinking)
                {
                    alcoholOnMouth = true;
                    drinking = false;
                    ChangeDwarfAction(DwarfActions.HoldingLighter);
                }
                else if (alcoholOnMouth)
                {
                    skillMeasureBarVisible = false;
                    skillValue = 0;
                    alcoholOnMouth = false;
                    ChangeDwarfAction(DwarfActions.None);
                }
            }

            if (inputs.FirstSkillClicked())
            {
                if (dwarfActions != DwarfActions.HoldingBottle && dwarfActions != DwarfActions.HoldingLighter &&
                     dwarfActions != DwarfActions.SpittingFire)
                {
                    if (sp >= 25)
                    {
                        sp -= 25;
                        ChangeDwarfAction(DwarfActions.ThrowingBottle);
                    }
                }
            }

            if (inputs.FirstSkillHolding())
            {
                if (alcoholOnMouth)
                {
                    skillValue -= 1f;
                    if (skillValue <= 0)
                    {
                        skillMeasureBarVisible = false;
                        skillValue = 0;
                        alcoholOnMouth = false;
                        ChangeDwarfAction(DwarfActions.None);
                    }
                    else
                    {
                        foreach (Player p in Game1.players)
                        {
                            if (p != this)
                            {
                                if (p.playerAnimation.destinationRect.Intersects(fireSpitting.spittingAnimation.destinationRect) && !p.playerIsDead)
                                {
                                    p.hp -= fireSpitting.damage;
                                }
                            }
                        }

                        ChangeDwarfAction(DwarfActions.SpittingFire);
                    }
                }
            }

            if (inputs.FirstSkillReleased())
            {
                if (alcoholOnMouth)
                {
                    ChangeDwarfAction(DwarfActions.HoldingLighter);
                }
            }

            if (inputs.SecondSkillClicked())
            {
                if (dwarfActions != DwarfActions.Smoking && onGround != null &&
                    dwarfActions != DwarfActions.HoldingBottle && dwarfActions != DwarfActions.HoldingLighter && 
                     dwarfActions != DwarfActions.SpittingFire)
                {
                    playerAnimation.frameTime = 50;
                    ChangeDwarfAction(DwarfActions.Smoking);
                }
            }
        }

        public void ChangeDwarfAction(DwarfActions action)
        {
            if (dwarfActions != action)
            {
                dwarfActions = action;
                changeAnimation = true;
            }
        }

        public override void ChangeAnimation()
        {
            if (!changeAnimation)
                return;

            switch (dwarfActions)
            {
                case DwarfActions.None:
                    base.ChangeAnimation();
                    break;

                case DwarfActions.HoldingBottle:
                case DwarfActions.HoldingLighter:
                case DwarfActions.SpittingFire:
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
                            playerAnimation.firstFrame = 3;
                            playerAnimation.lastFrame = 3;
                            break;
                    }
                    break;

                case DwarfActions.Smoking:
                    playerAnimation.firstFrame = 9;
                    playerAnimation.lastFrame = 33;
                    break;

                case DwarfActions.ThrowingBottle:
                    playerAnimation.firstFrame = 4;
                    playerAnimation.lastFrame = 5;
                    break;
            }

            playerAnimation.currentFrame = playerAnimation.firstFrame;
            changeAnimation = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void DrawSkills(SpriteBatch spriteBatch)
        {
            bool isRight;

            if (side == Side.Right)
                isRight = true;
            else
                isRight = false;

            if (boozes.Count > 0)
            {
                foreach (Projectile sb in boozes)
                {
                    sb.Draw(spriteBatch);
                }
            }

            switch (dwarfActions)
            {
                case DwarfActions.HoldingBottle:
                    fireSpitting.boozeAnimation.Draw(spriteBatch, isRight);
                    break;

                case DwarfActions.HoldingLighter:
                    fireSpitting.lighterAnimation.Draw(spriteBatch, isRight);
                    break;

                case DwarfActions.SpittingFire:
                    fireSpitting.spittingAnimation.Draw(spriteBatch, isRight);
                    break;
            }
        }
    }
}

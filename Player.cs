using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Player
    {
        //public Texture2D PlayerTexture;
        public Animation playerAnimation;
        public Animation defaultAnimation;
        public Animation deadAnimation;

        // Position of the Player relative to the upper left side of the screen
        //public Vector2 position;
        public float positionX
        {
            get { return playerAnimation.position.X; }
            set
            {
                playerAnimation.position.X = value;
                playerAnimation.destinationRect.X = (int)value;
                AdjustCollisionRectangles();

                if (aiming)
                {
                    if (side == Side.Right)
                        aim.position.X = (value + width) - distanceToHand.X;
                    else if (side == Side.Left)
                        aim.position.X = value + distanceToHand.X;
                }
            }
        }

        public float positionY
        {
            get { return playerAnimation.position.Y; }
            set
            {
                playerAnimation.position.Y = value;
                playerAnimation.destinationRect.Y = (int)value;
                AdjustCollisionRectangles();

                if (aiming)
                {
                    aim.position.Y = value + distanceToHand.Y - 2;
                }
            }
        }

        public Selection.Character Type
        {
            get 
            {
                return (Selection.Character)Enum.Parse(typeof(Selection.Character), this.GetType().Name);
            }
        }

        public Vector2 inicialPosition;
        public Vector2 distanceToHand;

        public Aim aim;
        public bool aiming;

        Texture2D measureBar;
        Rectangle measureBarSourceRectangle;
        Rectangle measure_hpRectangle;
        Rectangle hpRectangle;
        Rectangle measure_spRectangle;
        Rectangle spRectangle;
        Rectangle measure_skillRectangle;
        Rectangle skillRectangle;
        Rectangle hp_spSourceRectangle;
        public bool skillMeasureBarVisible;

        // Rectangles representing up, down, left or right sides of the player
        public Rectangle up;
        public Rectangle down;
        public Rectangle left;
        public Rectangle right;

        //SpriteFont playerName;

        // Player ID
        public string id { get; set; }

        // State of the player
        //public bool active;

        // Set if player is on ground
        public Tile onGround { get; set; }

        // Set if the animation is to be changed
        public bool changeAnimation { get; set; }

        // Set the force of jump applied
        public float force { get; set; }
        // Set the default force of jump
        public float jumpForce { get; set; }
        public int jumpElapsedTime;

        // Set gravity
        public float gravity;

        public float maximumFallingSpeed { get; set; }

        public float maxHp;
        public float maxSp;
        public float hp;
        public float sp;
        public float spRegen;
        public float defaultSpRegen;
        public float skillValue;
        public float maxSkillValue;

        public bool playerIsDead;
        public bool stopUpdating;
        public bool deadPlayerOnGround;

        //Movement speed of the player
        public float movement { get; set; }
        float initialMovement;

        // Get the width of the player
        public int width
        {
            get { return (int)(playerAnimation.frameWidth * playerAnimation.scale); }
        }

        // Get the height of the player
        public int height
        {
            get { return (int)(playerAnimation.frameHeight * playerAnimation.scale); }
        }

        // Get the rectangle representing the whole body of the player
        public Rectangle body
        {
            get { return playerAnimation.destinationRect; }
        }

        // Side the player
        public Side side { get; set; }
        public Side startingSide;

        public Inputs inputs;

        // Get or set what the player is doing
        public PlayerAction action { get; set; }

        public enum Side { Left, Right }

        public enum PlayerAction { Idle, Walking, Jumping }

        public Player(Animation animation, Animation deadAnimation, Aim aim, Texture2D measureBar, 
                      string id, int maxHp, int maxSp, float spRegen, float move, float maximumFallingSpeed, 
                      Vector2 distanceToHand, Side side, Inputs inputs)
        {
            playerAnimation = animation;
            defaultAnimation = animation;
            this.deadAnimation = deadAnimation;
            changeAnimation = false;

            this.side = side;
            startingSide = side;

            this.aim = aim;
            aiming = false;
            this.distanceToHand = distanceToHand;

            this.inputs = inputs;

            //playerName = font;

            this.id = id;

            // Set the starting position of the player around the middle of the screen and to the back
            //this.position = position;
            //positionX = (int)position.X;
            //positionY = (int)position.Y;
            inicialPosition = playerAnimation.position;

            // Set the player to be active
            //active = true;

            playerIsDead = false;
            stopUpdating = false;
            deadPlayerOnGround = false;

            // Set the player movemente speed
            movement = move;
            initialMovement = movement;

            this.maxHp = maxHp;
            this.maxSp = maxSp;
            this.hp = maxSp;
            this.sp = maxSp;
            this.spRegen = spRegen;
            defaultSpRegen = spRegen;
            //spRegen = 0.5f;

            skillValue = 0;

            onGround = null;

            this.maximumFallingSpeed = maximumFallingSpeed;

            force = 0;
            jumpForce = 8f;

            gravity = 0.3f;

            AdjustCollisionRectangles();

            this.measureBar = measureBar;
            measureBarSourceRectangle = new Rectangle(0, 0, (measureBar.Width / 2), measureBar.Height);
            measure_spRectangle = new Rectangle((int)positionX, (int)positionY - measureBar.Height - 3, width, measureBar.Height);
            spRectangle = new Rectangle((int)positionX, (int)positionY - measureBar.Height - 3, (int)GetValueFromPercentage(width, GetPercentage(maxSp, sp)), measureBar.Height);
            measure_hpRectangle = new Rectangle((int)positionX, spRectangle.Y - measureBar.Height, width, measureBar.Height);
            hpRectangle = new Rectangle((int)positionX, spRectangle.Y - measureBar.Height, (int)GetValueFromPercentage(width, GetPercentage(maxHp, hp)), measureBar.Height);
            measure_skillRectangle = new Rectangle((int)positionX, hpRectangle.Y - measureBar.Height - 3, width, measureBar.Height);
            skillRectangle = new Rectangle((int)positionX, hpRectangle.Y - measureBar.Height, (int)GetValueFromPercentage(width, GetPercentage(maxSkillValue, skillValue)), measureBar.Height);
            hp_spSourceRectangle = new Rectangle(measureBar.Width / 2, 0, (measureBar.Width / 2), measureBar.Height);

            skillMeasureBarVisible = false;

            action = PlayerAction.Idle;
        }

        public virtual void ResetPlayer()
        {
            RestoreDefaultAnimation(PlayerAction.Idle);
            side = startingSide;
            aiming = false;
            positionX = inicialPosition.X;
            positionY = inicialPosition.Y;
            //active = true;
            playerIsDead = false;
            stopUpdating = false;
            deadPlayerOnGround = false;
            movement = initialMovement;
            hp = maxHp;
            sp = maxSp;
            spRegen = defaultSpRegen;
            skillValue = 0;
            onGround = null;
            force = 0;
            jumpForce = 8f;
            gravity = 0.3f;
            skillMeasureBarVisible = false;
        }

        public int GetPercentage(float max, float value)
        {
            return (int)((value * 100) / max);
        }

        public float GetValueFromPercentage(float max, float percent)
        {
            return (max * percent) / 100;
        }

        public virtual void Update(GameTime gameTime)
        {
            //GetPlayerInputs();
            //RespawnPlayer();

            //// Do not update the game if we are not active
            //if (!active) return;

            //CheckBasicMovementInputs(gameTime);
            //playerAnimation.position = position;

            // Make sure that the player does not go out of bounds
            positionX = MathHelper.Clamp(positionX, 0, Game1.screenWidth - (width) - 1);
            positionY = MathHelper.Clamp(positionY, 0, Game1.screenHeight - (height) - 1);

            CheckIfDead();

            if (playerIsDead)
            {
                return;
            }

            RegenSP();

            ApplyGravity();
            //playerAnimation.position = position;

            ChangeAnimation();

            CheckForTilesIntersections();
            //playerAnimation.position = position;

            AdjustMeasureBarRectangles();

            playerAnimation.Update(gameTime);
        }

        public float TopOfFirstMeasureBar()
        {
            if (skillMeasureBarVisible)
                return measure_skillRectangle.Top;
            else
                return measure_hpRectangle.Top;
        }

        public void AdjustPositionInCaseOfTilesIntersection(float initialPositionX, float initialPositionY)
        {
            foreach (Tile t in Game1.tiles)
            {
                if (body.Intersects(t.rectangle))
                {
                    if (t.rectangle.Width > t.rectangle.Height)
                    {
                        float backUpPositionY = positionY;

                        if (initialPositionY < positionY)
                        {
                            positionY = t.rectangle.Y - height;
                        }
                        else
                        {
                            positionY = t.rectangle.Y + t.rectangle.Height;
                            force = 0;
                        }

                        if (IntersectAnotherTile())
                        {
                            positionY = backUpPositionY;

                            if (initialPositionX < positionX)
                            {
                                positionX = t.rectangle.X - width;
                            }
                            else
                            {
                                positionX = t.rectangle.X + t.rectangle.Width;
                            }
                        }
                    }
                    else
                    {
                        float backUpPositionX = positionX;

                        if (initialPositionX < positionX)
                        {
                            positionX = t.rectangle.X - width;
                        }
                        else
                        {
                            positionX = t.rectangle.X + t.rectangle.Width;
                        }

                        if (IntersectAnotherTile())
                        {
                            positionX = backUpPositionX;

                            if (initialPositionY < positionY)
                            {
                                positionY = t.rectangle.Y - height;
                            }
                            else
                            {
                                positionY = t.rectangle.Y + t.rectangle.Height;
                                force = 0;
                            }
                        }
                    }

                    break;
                }
            }
        }

        public bool IntersectAnotherTile()
        {
            foreach (Tile t in Game1.tiles)
            {
                if (body.Intersects(t.rectangle))
                {
                    return true;
                }
            }

            return false;
        }

        public void SetNewAnimation(Animation animation)
        {
            float backUpPositionX = positionX;
            float backUpPositionY = positionY;

            playerAnimation = animation;
            playerAnimation.currentFrame = animation.firstFrame;

            positionX = backUpPositionX + ((width - animation.frameWidth) / 2);
            positionY = backUpPositionY + ((height - animation.frameHeight) / 2);
        }

        public void RestoreDefaultAnimation(PlayerAction act)
        {
            float backUpPositionX = positionX;
            float backUpPositionY = positionY;

            playerAnimation = defaultAnimation;

            positionX = backUpPositionX + ((width - defaultAnimation.frameWidth) / 2);
            positionY = backUpPositionY + ((height - defaultAnimation.frameHeight) / 2);

            ChangeAction(act);
        }

        public void CheckIfDead()
        {
            if (hp <= 0)
            {
                //active = false;
                aiming = false;
                playerIsDead = true;
            }
        }

        public void UpdateDeadPlayer(GameTime gameTime, int firstFrameAfterHitGround, int lastFrameAfterHitGround,
                                     int distanceToGround)
        {
            if (onGround != null)
            {
                playerAnimation.Update(gameTime);

                if (playerAnimation != deadAnimation)
                    SetNewAnimation(deadAnimation);

                if (deadPlayerOnGround)
                {
                    if (playerAnimation.LastFrame())
                    {
                        stopUpdating = true;
                        Game1.deadPlayers++;
                    }
                }
                else if (playerAnimation.currentFrame == playerAnimation.lastFrame || playerAnimation.currentFrame == playerAnimation.lastFrame - 1)
                {
                    playerAnimation.firstFrame = firstFrameAfterHitGround;
                    playerAnimation.lastFrame = lastFrameAfterHitGround;
                    playerAnimation.currentFrame = firstFrameAfterHitGround;
                    deadPlayerOnGround = true;
                    positionY = onGround.rectangle.Top - height; ;
                    positionY += distanceToGround;
                }
            }
            else
            {
                playerAnimation.Update(gameTime);

                if (playerAnimation != deadAnimation)
                    SetNewAnimation(deadAnimation);

                positionY -= (int)(force);
                force -= gravity;

                if (force <= 0)
                {
                    gravity = 0.3f;
                }

                foreach (Tile t in Game1.tiles)
                {
                    if (t.rectangle.Intersects(down))
                    {
                        positionY = t.rectangle.Y - height;
                        onGround = t;
                    }
                }
            }
        }

        public void RegenSP()
        {
            if (sp < 100)
            {
                sp += spRegen;
            }
            else
            {
                sp = 100;
            }
        }

        public void ApplyGravity()
        {
            if (onGround == null)
            {
                positionY -= (int)(force);
                force -= gravity;

                if (force <= 0)
                {
                    gravity = 0.3f;
                }

                ChangeAction(PlayerAction.Jumping);
            }
        }

        public void CheckForTilesIntersections()
        {
            foreach (Tile t in Game1.tiles)
            {
                if (t.rectangle.Intersects(down))
                {
                    if (onGround == null)
                    {
                        positionY = t.rectangle.Y - height;
                        onGround = t;
                        force = 0;
                        jumpElapsedTime = 0;
                        gravity = 0.3f;
                    }
                }
                else if (onGround == t)
                {
                    onGround = null;
                    force = 0;
                }

                if (t.rectangle.Intersects(up))
                {
                    positionY = t.rectangle.Y + t.rectangle.Height;
                    force = 0;
                }
                else if (t.rectangle.Intersects(left) && side == Side.Left)
                {
                    positionX = t.rectangle.X + t.rectangle.Width;
                }
                else if (t.rectangle.Intersects(right) && side == Side.Right)
                {
                    positionX = t.rectangle.X - width;
                }
            }
        }

        public void RespawnPlayer()
        {
            //if (inputs.PauseGame())
            //{
            //    positionX = inicialPosition.X;
            //    positionY = inicialPosition.Y;
            //    hp = 100;
            //    sp = 100;
            //    force = 0;
            //    jumpElapsedTime = 0;
            //    gravity = 0.3f;
            //    onGround = null;
            //    //active = true;
            //}
        }

        public void CheckInputs(GameTime gameTime)
        {
            float walkValue = inputs.MoveX();

            if (walkValue != 0)
            {
                Walk(walkValue);
            }
            else if (onGround != null)
            {
                ChangeAction(PlayerAction.Idle);
            }

            if (inputs.JumpHolding())
            {
                jumpElapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                Jump();
            }

            if (inputs.JumpReleased())
            {
                if (jumpElapsedTime < 200)
                {
                    gravity = 0.7f;
                }
            }

            if (inputs.Aim())
            {
                if (!aiming)
                {
                    if (side == Side.Right)
                        this.aim.position = new Vector2((positionX + width) - distanceToHand.X, positionY + distanceToHand.Y - 2);
                    else
                        this.aim.position = new Vector2(positionX, positionY + distanceToHand.Y - 2);

                    aiming = true;
                }
                else if (aiming)
                {
                    aim.angle = 0;
                    aim.angleInDegrees = 0;
                    aim.currentFrame = 0;
                    aiming = false;
                }
            }

            if (aiming)
            {
                inputs.RotateAim(aim, side);
            }
        }

        public void Jump()
        {
            if (onGround == null)
                return;

            onGround = null;
            force = jumpForce;
            //gravity = 0.3f;

            ChangeAction(PlayerAction.Jumping);
        }

        public void AdjustMeasureBarRectangles()
        {
            spRectangle.X = (int)positionX;
            spRectangle.Y = (int)positionY - measureBar.Height - 3;
            spRectangle.Width = (int)GetValueFromPercentage(width, GetPercentage(maxSp, sp));
            measure_spRectangle.X = (int)positionX;
            measure_spRectangle.Y = (int)positionY - measureBar.Height - 3;
            measure_spRectangle.Width = width;

            hpRectangle.X = (int)positionX;
            hpRectangle.Y = spRectangle.Y - measureBar.Height;
            hpRectangle.Width = (int)GetValueFromPercentage(width, GetPercentage(maxHp, hp));
            measure_hpRectangle.X = hpRectangle.X;
            measure_hpRectangle.Y = hpRectangle.Y;
            measure_hpRectangle.Width = width;

            if (!skillMeasureBarVisible)
                return;

            skillRectangle.X = (int)positionX;
            skillRectangle.Y = hpRectangle.Y - measureBar.Height;
            skillRectangle.Width = (int)GetValueFromPercentage(width, GetPercentage(maxSkillValue, skillValue));
            measure_skillRectangle.X = skillRectangle.X;
            measure_skillRectangle.Y = skillRectangle.Y;
            measure_skillRectangle.Width = width;
        }

        public void Walk(float multiplier)
        {
            if (onGround != null)
            {
                ChangeAction(PlayerAction.Walking);
            }

            if (multiplier > 0)
                side = Side.Right;
            else if (multiplier < 0)
                side = Side.Left;

            positionX += (movement * multiplier);
        }

        public void AdjustCollisionRectangles()
        {
            up = new Rectangle((int)positionX + ((int)movement + 1), (int)positionY, width - (((int)movement + 1) * 2), height / 2);
            down = new Rectangle((int)positionX + ((int)movement + 1), (int)positionY + height - (height / 2), width - (((int)movement + 1) * 2), (height / 2) + 1);
            left = new Rectangle((int)positionX - 1, (int)positionY, ((int)movement + 1) + 1, height - 2);
            right = new Rectangle((int)positionX + width - ((int)movement + 1), (int)positionY, ((int)movement + 1) + 1, height - 2);
        }

        public void ChangeAction(PlayerAction act)
        {
            if (action != act)
            {
                action = act;
                changeAnimation = true;
            }
        }

        public virtual void ChangeAnimation()
        {
            if (!changeAnimation)
                return;

            switch (action)
            {
                case PlayerAction.Idle:
                    playerAnimation.firstFrame = 0;
                    playerAnimation.lastFrame = 0;
                    break;

                case PlayerAction.Walking:
                    playerAnimation.firstFrame = 1;
                    playerAnimation.lastFrame = 2;
                    break;

                case PlayerAction.Jumping:
                    playerAnimation.firstFrame = 3;
                    playerAnimation.lastFrame = 3;
                    break;
            }

            playerAnimation.currentFrame = playerAnimation.firstFrame;
            changeAnimation = false;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!playerIsDead)
            {
                if (skillMeasureBarVisible)
                {
                    spriteBatch.Draw(measureBar, skillRectangle, hp_spSourceRectangle, Color.Yellow);
                    spriteBatch.Draw(measureBar, measure_skillRectangle, measureBarSourceRectangle, Color.White);
                }
                spriteBatch.Draw(measureBar, hpRectangle, hp_spSourceRectangle, Color.Red);
                spriteBatch.Draw(measureBar, spRectangle, hp_spSourceRectangle, Color.Blue);
                spriteBatch.Draw(measureBar, measure_hpRectangle, measureBarSourceRectangle, Color.White);
                spriteBatch.Draw(measureBar, measure_spRectangle, measureBarSourceRectangle, Color.White);
            }

            if (side == Side.Right) playerAnimation.Draw(spriteBatch, true);
            else playerAnimation.Draw(spriteBatch, false);

            if (aiming)
                aim.Draw(spriteBatch, side);

            //if (active)
            //{
            //    if (!playerIsDead)
            //    {
            //        if (skillMeasureBarVisible)
            //        {
            //            spriteBatch.Draw(measureBar, skillRectangle, hp_spSourceRectangle, Color.Yellow);
            //            spriteBatch.Draw(measureBar, measure_skillRectangle, measureBarSourceRectangle, Color.White);
            //        }
            //        spriteBatch.Draw(measureBar, hpRectangle, hp_spSourceRectangle, Color.Red);
            //        spriteBatch.Draw(measureBar, spRectangle, hp_spSourceRectangle, Color.Blue);
            //        spriteBatch.Draw(measureBar, measure_hpRectangle, measureBarSourceRectangle, Color.White);
            //        spriteBatch.Draw(measureBar, measure_spRectangle, measureBarSourceRectangle, Color.White);
            //    }

            //    if (side == Side.Right) playerAnimation.Draw(spriteBatch, true);
            //    else playerAnimation.Draw(spriteBatch, false);

            //    if (aiming)
            //        aim.Draw(spriteBatch, side);

            //    //spriteBatch.DrawString(playerName, id, new Vector2(playerAnimation.position.X, playerAnimation.position.Y - 36), Color.Black);
            //}
        }

        public virtual void DrawSkills(SpriteBatch spriteBatch)
        {

        }
    }
}

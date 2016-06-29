using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Projectile
    {
        public Animation animation;
        public float gravity;
        public float gravityIncrement;
        public float damage;
        public float force;
        public float angle;
        float incrementX;
        float incrementY;

        public int timesHitTiles;

        public float IncrementX
        {
            get { return incrementX; }
            set
            {
                incrementX = value;
                SetDirectionX();
            }
        }

        public float IncrementY
        {
            get { return incrementY; }
            set
            {
                incrementY = value;
                SetDirectionY();
            }
        }

        public Tile leftBlock;
        public Tile rightBlock;
        public Tile upBlock;
        public Tile downBlock;

        public DirectionX directionX;
        public DirectionY directionY;

        public enum DirectionX { Left, Right, Neutral }
        public enum DirectionY { Up, Down, Neutral }

        public int width
        {
            get { return animation.frameWidth; }
        }

        public int height
        {
            get { return animation.frameHeight; }
        }

        public Rectangle rectangle
        {
            get { return animation.destinationRect; }
        }

        public float positionX
        {
            get { return animation.position.X; }
            set
            {
                animation.position.X = value;
                animation.destinationRect.X = (int)value;
            }
        }

        public float positionY
        {
            get { return animation.position.Y; }
            set
            {
                animation.position.Y = value;
                animation.destinationRect.Y = (int)value;
            }
        }

        public Projectile(Animation animation, float gravityIncrement, float damage, float force)
        {
            this.animation = animation;
            this.gravityIncrement = gravityIncrement;
            this.gravity = 0;
            this.damage = damage;
            this.force = force;
            leftBlock = null;
            rightBlock = null;
            upBlock = null;
            downBlock = null;
            timesHitTiles = 0;
        }

        public void Update(GameTime gameTime)
        {
            animation.Update(gameTime);

            positionX += incrementX;

            positionY += incrementY;

            positionY += gravity;
            gravity += gravityIncrement;
        }

        public void AdjustPositionYAfterTile(Tile tile)
        {
            if (directionY == DirectionY.Down)
            {
                positionY = tile.rectangle.Top - height;

                tile = null;
            }
            else if (directionY == DirectionY.Up)
            {
                positionY = tile.rectangle.Bottom;

                tile = null;
            }
        }

        public void AdjustPositionXAfterTile(Tile tile)
        {
            if (directionX == DirectionX.Left)
            {
                positionX = tile.rectangle.Right;

                tile = null;
            }
            else if (directionX == DirectionX.Right)
            {
                positionX = tile.rectangle.Left - width;

                tile = null;
            }
        }

        public void AdjustPositionAfterBlock(GameTime gameTime)
        {
            animation.Update(gameTime);

            if (upBlock != null)
            {
                positionY = upBlock.rectangle.Bottom;

                upBlock = null;
            }
            else if (downBlock != null)
            {
                positionY = downBlock.rectangle.Top - height;

                downBlock = null;
            }
            
            if (leftBlock != null)
            {
                positionX = leftBlock.rectangle.Right;

                leftBlock = null;
            }
            else if (rightBlock != null)
            {
                positionX = rightBlock.rectangle.Left - width;

                rightBlock = null;
            }
        }

        public void CheckForBlocks(Tile tile)
        {
            CheckForYBlocks(tile);

            CheckForXBlocks(tile);
        }

        public void CheckForXBlocks(Tile tile)
        {
            int extraIncrement;

            if (directionX == DirectionX.Right || directionX == DirectionX.Neutral)
            {
                extraIncrement = 1;
            }
            else
            {
                extraIncrement = -1;
            }

            Rectangle nextPositionXRectangle = new Rectangle((int)positionX + ((int)IncrementX + extraIncrement),
                                                    (int)positionY, width, height);

            if (nextPositionXRectangle.Intersects(tile.rectangle))
            {
                if (directionX == DirectionX.Left)
                {
                    leftBlock = tile;
                }
                else if (directionX == DirectionX.Right)
                {
                    rightBlock = tile;
                }
            }
        }

        public void CheckForYBlocks(Tile tile)
        {
            int extraIncrement;

            if (directionY == DirectionY.Down || directionY == DirectionY.Neutral)
            {
                extraIncrement = 1;
            }
            else
            {
                extraIncrement = -1;
            }

            Rectangle nextPositionYRectangle = new Rectangle((int)positionX,
                                                    (int)positionY + ((int)IncrementY + extraIncrement), width, height);

            if (nextPositionYRectangle.Intersects(tile.rectangle))
            {
                if (directionY == DirectionY.Down)
                {
                    downBlock = tile;
                }
                else if (directionY == DirectionY.Up)
                {
                    upBlock = tile;
                }
            }
        }

        public void PrepareToThrow(float positionX, float positionY, float angle, Player.Side direction)
        {
            this.positionX = positionX;

            this.positionY = positionY;

            this.angle = angle;

            SetIncrementValues(direction);
        }

        public void SetIncrementValues(Player.Side direction)
        {
            if (direction == Player.Side.Right)
            {
                IncrementX = (float)((force) * Math.Cos(angle * -1));
            }
            else
            {
                IncrementX = -((float)((force) * Math.Cos(angle * -1)));
            }

            IncrementY = (float)((force) * Math.Sin(angle * -1));
        }

        public void SetDirectionX()
        {
            if (incrementX > 0)
            {
                directionX = DirectionX.Right;
            }
            else if (incrementX < 0)
            {
                directionX = DirectionX.Left;
            }
            else
            {
                directionX = DirectionX.Neutral;
            }
        }

        public void SetDirectionY()
        {
            if (incrementY < 0)
            {
                directionY = DirectionY.Up;
            }
            else if (incrementY > 0)
            {
                directionY = DirectionY.Down;
            }
            else
            {
                directionY = DirectionY.Neutral;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            bool isRight;

            if (directionX == DirectionX.Right)
            {
                isRight = true;
            }
            else
            {
                isRight = false;
            }

            animation.Draw(spriteBatch, isRight);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Poo
    {
        public Texture2D poo;
        public Rectangle pooRectangle;
        public Animation stinkAnimation;
        public Player infectedPlayer;

        public float damage;
        public float damageApplied;
        public float damageDecrement;

        public float stinkPositionX
        {
            get { return stinkAnimation.position.X; }
            set
            {
                stinkAnimation.position.X = value;
                stinkAnimation.destinationRect.X = (int)value;
            }
        }

        public float stinkPositionY
        {
            get { return stinkAnimation.position.Y; }
            set
            {
                stinkAnimation.position.Y = value;
                stinkAnimation.destinationRect.Y = (int)value;
            }
        }

        public int pooPositionX
        {
            get { return pooRectangle.X; }
            set
            {
                pooRectangle.X = value;
            }
        }

        public int pooPositionY
        {
            get { return pooRectangle.Y; }
            set
            {
                pooRectangle.Y = value;
            }
        }

        public int width
        {
            get { return poo.Width; }
        }

        public int height
        {
            get { return poo.Height; }
        }

        public Poo(Texture2D poo, float damage, float damageDecrement, Animation stinkAnimation)
        {
            this.poo = poo;
            this.damage = damage;
            this.damageDecrement = damageDecrement;
            this.stinkAnimation = stinkAnimation;
            damageApplied = 0;
            infectedPlayer = null;
        }

        public void Draw(SpriteBatch spriteBatch, bool isRight)
        {
            if (infectedPlayer == null)
                spriteBatch.Draw(poo, pooRectangle, Color.White);
            else if (!infectedPlayer.playerIsDead)
                stinkAnimation.Draw(spriteBatch, isRight);
        }
    }
}

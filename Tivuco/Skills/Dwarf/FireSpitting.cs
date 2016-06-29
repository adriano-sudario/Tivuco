using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class FireSpitting
    {
        public Animation boozeAnimation;
        public Animation lighterAnimation;
        public Animation spittingAnimation;

        public float damage;
        public float maximumAmount;
        public float skillValueIncrement;
        public float spDecrement;

        public int boozeWidth
        {
            get { return (int)(boozeAnimation.frameWidth * boozeAnimation.scale); }
        }

        public int boozeHeight
        {
            get { return (int)(boozeAnimation.frameHeight * boozeAnimation.scale); }
        }

        public int lighterWidth
        {
            get { return (int)(lighterAnimation.frameWidth * boozeAnimation.scale); }
        }

        public int lighterHeight
        {
            get { return (int)(lighterAnimation.frameHeight * boozeAnimation.scale); }
        }

        public int spittingWidth
        {
            get { return (int)(spittingAnimation.frameWidth * boozeAnimation.scale); }
        }

        public int spittingHeight
        {
            get { return (int)(spittingAnimation.frameHeight * boozeAnimation.scale); }
        }

        public float lighterPositionX
        {
            get { return lighterAnimation.position.X; }
            set
            {
                lighterAnimation.position.X = value;
                lighterAnimation.destinationRect.X = (int)value;
            }
        }

        public float lighterPositionY
        {
            get { return lighterAnimation.position.Y; }
            set
            {
                lighterAnimation.position.Y = value;
                lighterAnimation.destinationRect.Y = (int)value;
            }
        }

        public float boozePositionX
        {
            get { return boozeAnimation.position.X; }
            set
            {
                boozeAnimation.position.X = value;
                boozeAnimation.destinationRect.X = (int)value;
            }
        }

        public float boozePositionY
        {
            get { return boozeAnimation.position.Y; }
            set
            {
                boozeAnimation.position.Y = value;
                boozeAnimation.destinationRect.Y = (int)value;
            }
        }

        public float spittingPositionX
        {
            get { return spittingAnimation.position.X; }
            set
            {
                spittingAnimation.position.X = value;
                spittingAnimation.destinationRect.X = (int)value;
            }
        }

        public float spittingPositionY
        {
            get { return spittingAnimation.position.Y; }
            set
            {
                spittingAnimation.position.Y = value;
                spittingAnimation.destinationRect.Y = (int)value;
            }
        }

        public FireSpitting(Animation boozeAnimation, Animation lighterAnimation, Animation spittingAnimation,
                            float damage, float maximumAmount, float skillValueIncrement)
        {
            this.boozeAnimation = boozeAnimation;
            this.lighterAnimation = lighterAnimation;
            this.spittingAnimation = spittingAnimation;

            this.skillValueIncrement = skillValueIncrement;
            spDecrement = skillValueIncrement / 4;
            this.damage = damage;
            this.maximumAmount = maximumAmount;
        }
    }
}

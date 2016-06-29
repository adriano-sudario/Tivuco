using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Happy : Player
    {
        HappyActions happyActions;

        public enum HappyActions { None, OnAcid, Spinning, ThrowingWhatever }

        public Happy(Animation animation, Animation deadAnimation, Aim aim, Texture2D measureBar,
                     string id, int maxHp, int maxSp, float spRegen, float move, float maximumFallingSpeed,
                     Vector2 distanceToHand, Side side, Inputs inputs)
            : base(animation, deadAnimation, aim, measureBar, id, maxHp, maxSp, spRegen, move, maximumFallingSpeed,
                   distanceToHand, side, inputs)
        {
            happyActions = HappyActions.None;
        }

        public override void ResetPlayer()
        {
            base.ResetPlayer();
            happyActions = HappyActions.None;
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

                return;
            }

            CheckInputs(gameTime);
            CheckHappyInputs();

            base.Update(gameTime);

            if (aiming)
                aim.Update(gameTime, side);
        }

        public void CheckHappyInputs()
        {
            
        }

        public void ChangeHappyAction(HappyActions action)
        {
            if (happyActions != action)
            {
                happyActions = action;
                changeAnimation = true;
            }
        }

        public override void ChangeAnimation()
        {
            if (!changeAnimation)
                return;

            switch (happyActions)
            {
                case HappyActions.None:
                    base.ChangeAnimation();
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
            
        }
    }
}

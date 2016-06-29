using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Love
    {
        Texture2D heartTexture;
        Rectangle heartRectangle;
        Animation disillusionRaysAnimation;

        int timesHeartBlinked;
        int maxHeartBlinks;
        float heartMillisecondsTime;
        float heartElapsedTime;
        bool showHeart;

        float disillusionRaysDamage;
        List<Player> playersHitByDisillusionRays;

        public float disillusionRaysPositionX
        {
            get { return disillusionRaysAnimation.position.X; }
            set
            {
                disillusionRaysAnimation.position.X = value;
                disillusionRaysAnimation.destinationRect.X = (int)value;
            }
        }

        public float disillusionRaysPositionY
        {
            get { return disillusionRaysAnimation.position.Y; }
            set
            {
                disillusionRaysAnimation.position.Y = value;
                disillusionRaysAnimation.destinationRect.Y = (int)value;
            }
        }

        float invisibilityMillisecondsTime;
        float invisibilityElapsedTime;
        public bool affectedByLoveInvisibility;

        public float manaCost;

        public Love(Texture2D heartTexture, Animation disillusionRaysAnimation)
        {
            this.disillusionRaysAnimation = disillusionRaysAnimation;
            this.heartTexture = heartTexture;
            timesHeartBlinked = 0;
            maxHeartBlinks = 3;
            heartMillisecondsTime = 250;
            heartElapsedTime = 0;
            showHeart = false;

            disillusionRaysDamage = 40;
            playersHitByDisillusionRays = new List<Player>();

            invisibilityMillisecondsTime = 4000;
            invisibilityElapsedTime = 0;
            affectedByLoveInvisibility = false;

            manaCost = 50;
        }

        public void UpdateHeartPosition(Goat goat)
        {
            heartRectangle = new Rectangle((int)goat.positionX + (goat.width / 2) - (heartTexture.Width / 2),
                                            (int)goat.TopOfFirstMeasureBar() - heartTexture.Height - 5,
                                            heartTexture.Width, heartTexture.Height);
        }

        public void UpdateDisillusionRaysPosition(Goat goat)
        {
            disillusionRaysPositionX = goat.positionX;
            disillusionRaysPositionY = goat.positionY;
        }

        public void ReleaseDisillusionRays(Goat goat)
        {
            //SoundsLibrary.PlaySound(SoundsLibrary.goatReleasingDisillusionRaysSound);
            UpdateDisillusionRaysPosition(goat);
            affectedByLoveInvisibility = false;
            goat.ChangeGoatAction(Goat.GoatActions.ReleasingDisillusionRays);
            disillusionRaysAnimation.currentFrame = 0;
            invisibilityElapsedTime = 0;
        }

        public void Update(GameTime gameTime, Goat goat)
        {
            if (affectedByLoveInvisibility)
            {
                invisibilityElapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (invisibilityElapsedTime > invisibilityMillisecondsTime)
                {
                    ReleaseDisillusionRays(goat);
                }

                return;
            }

            switch (goat.goatActions)
            {
                case Goat.GoatActions.InLove:
                    heartElapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (heartElapsedTime > heartMillisecondsTime)
                    {
                        if (!showHeart)
                        {
                            showHeart = true;
                            timesHeartBlinked++;
                        }
                        else
                        {
                            showHeart = false;

                            if (timesHeartBlinked >= maxHeartBlinks)
                            {
                                affectedByLoveInvisibility = true;
                                goat.ChangeGoatAction(Goat.GoatActions.None);
                                timesHeartBlinked = 0;
                                goat.sp -= manaCost;
                            }
                        }

                        heartElapsedTime = 0;
                    }

                    if (showHeart)
                        UpdateHeartPosition(goat);

                    break;

                case Goat.GoatActions.ReleasingDisillusionRays:
                    UpdateDisillusionRaysPosition(goat);

                    foreach (Player p in Game1.players)
                    {
                        if (p != goat)
                        {
                            bool alreadyHit = false;

                            if (playersHitByDisillusionRays.Count > 0)
                            {
                                foreach (Player ph in playersHitByDisillusionRays)
                                {
                                    if (ph == p)
                                        alreadyHit = true;
                                }
                            }

                            if (!alreadyHit)
                            {
                                if (disillusionRaysAnimation.destinationRect.Intersects(p.body) && !p.playerIsDead)
                                {
                                    p.hp -= disillusionRaysDamage;
                                    playersHitByDisillusionRays.Add(p);
                                }
                            }
                        }
                    }

                    if (disillusionRaysAnimation.LastFrame())
                    {
                        goat.ChangeGoatAction(Goat.GoatActions.None);
                        playersHitByDisillusionRays.Clear();

                        return;
                    }

                    disillusionRaysAnimation.Update(gameTime);
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Goat goat)
        {
            switch (goat.goatActions)
            {
                case Goat.GoatActions.InLove:
                    if (showHeart)
                    {
                        spriteBatch.Draw(heartTexture, heartRectangle, Color.White);
                    }
                    break;

                case Goat.GoatActions.ReleasingDisillusionRays:
                    bool isRight;

                    if (goat.side == Player.Side.Right)
                        isRight = true;
                    else
                        isRight = false;

                    disillusionRaysAnimation.Draw(spriteBatch, isRight);
                    break;
            }
        }
    }
}

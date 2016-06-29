using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    class Animation
    {
        // The image representing the collection of images used for animation
        Texture2D spriteStrip;
        // The scale used to display the sprite strip
        public float scale { get; set; }
        // The time since we last updated the frame
        int elapsedTime;
        // The time we display a frame until the next one
        public int frameTime { get; set; }
        // The number of frames that the animation contains
        public int lastFrame { get; set; }
        // The index of the current frame we are displaying
        public int currentFrame { get; set; }
        // The index of the beggining of the frame
        public int firstFrame { get; set; }
        // Tells if frame has changed
        public bool changedFrame { get; set; }
        // The color of the frame we will be displaying
        Color color;
        // The area of the image strip we want to display
        Rectangle sourceRect = new Rectangle();
        // The area where we want to display the image strip in the game
        public Rectangle destinationRect;
        // Width of a given frame
        public int frameWidth;
        // Height of a given frame
        public int frameHeight;
        // Control side of player
        public bool isRightSide { get; set; }
        // The state of the Animation
        public bool Active;
        // Determines if the animation will keep playing or deactivate after one run
        public bool Looping;

        // Width of a given frame
        public Vector2 position;

        public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int firstFrame, int lastFrame, int frametime, Color color, float scale, bool looping)
        {
            // Keep a local copy of the values passed in
            this.color = color;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.lastFrame = lastFrame;
            this.frameTime = frametime;
            this.firstFrame = firstFrame;
            this.scale = scale;

            Looping = looping;
            this.position = position;
            spriteStrip = texture;

            destinationRect = new Rectangle((int)position.X, (int)position.Y, (int)(frameWidth * scale), (int)(frameHeight * scale));

            // Set the time to zero
            elapsedTime = 0;
            currentFrame = firstFrame;

            isRightSide = true;

            // Set the Animation to active by default
            Active = true;

            SetAnimationRectangles();
            destinationRect = new Rectangle((int)position.X, (int)position.Y, (int)(frameWidth * scale), (int)(frameHeight * scale));
        }

        public void Update(GameTime gameTime)
        {
            // Do not update the game if we are not active
            if (Active == false) return;

            SetAnimationRectangles();

            // Update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            changedFrame = false;

            // If the elapsed time is larger than the frame time
            // we need to switch frames
            if (elapsedTime > frameTime)
            {
                changedFrame = true;
                // Move to the next frame
                currentFrame++;

                // If the currentFrame is equal to frameCount reset currentFrame to zero
                if (currentFrame > lastFrame)
                {
                    // If we are not looping, the last frame will always be selected
                    if (Looping == false)
                        currentFrame = lastFrame;
                    else
                        currentFrame = firstFrame;
                }

                // Reset the elapsed time to zero
                elapsedTime = 0;
            }
        }

        public void SetAnimationRectangles()
        {
            // Grab the correct frame in the image strip by multiplying the currentFrame index by the Frame width
            sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width
            //destinationRect = new Rectangle((int)position.X, (int)position.Y, (int)(frameWidth * scale), (int)(frameHeight * scale));
        }

        public bool LastFrame()
        {
            if (currentFrame == lastFrame)
                return true;
            else
                return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Only draw the animation when we are active
            if (Active)
            {
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
            }
        }

        public void Draw(SpriteBatch spriteBatch, bool isRight)
        {
            // Only draw the animation when we are active
            if (Active)
            {
                if (isRight)
                {
                    spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
                }
                else
                {
                    spriteBatch.Draw(spriteStrip, position, sourceRect, Color.White, 0, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 0);
                }
            }
        }
    }
}

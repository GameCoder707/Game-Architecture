using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework_180016989
{
    // It's mostly set to Center
    // for most objects
    // but Bottom is more accurate
    // for ground based collisions
    enum OriginLocation
    {
        Center = 0,
        Bottom = 1
    }

    class Animator
    {
        // The animation to play through
        public Animation Animation { get; set; }

        // The current frame currently rendering
        public int FrameIndex { get; set; }

        // Time taken to stay in a frame
        private float time;

        // To pause the animation,
        // then resume when needed
        private bool paused;
        public bool Paused
        {
            get { return paused; }
            set { paused = value; }
        }
        
        private OriginLocation originLocation;

        // Based on where the origin is positioned,
        // it returns the appropriate vector
        public Vector2 Origin
        {
            get
            {
                // Dividing just the width, to get the middle of the bottom
                if (originLocation == OriginLocation.Bottom)
                {
                    return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight);
                }
                // Dividing width and height to obtain 
                // the center of the frame
                else if (originLocation == OriginLocation.Center)
                {
                    return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight / 2.0f);
                }

                return Vector2.Zero;
            }
        }
        
        // Initialising the origin location
        public Animator(OriginLocation location)
        {
            originLocation = location;
        }

        // Function is called to play the animation
        // Optional parameters if you wanted to start from
        // a certain frame and whether to pause it
        public void PlayAnimation(Animation animation, int startIndex = 0,
            bool pause = false)
        {
            // If it's the same, then return to save time
            if (Animation == animation)
                return;

            Animation = animation;
            FrameIndex = startIndex;
            time = 0.0f;
            paused = pause;

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, float rotation, float scale, SpriteEffects spriteEffects)
        {
            // Null check
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // If we're not paused, then continue
            if(paused == false)
            {
                time += elapsed;

                // If the timer is greater than the
                // time needed to stay in a frame,
                // the proceed
                if (time > Animation.FrameTime)
                {
                    time = 0;

                    // If it's looping,
                    // increase the index but stay
                    // within the range
                    if (Animation.IsLooping)
                        FrameIndex = (FrameIndex + 1) % Animation.FrameCount;
                    
                    // Increase the index and
                    // reach the last index
                    else
                        FrameIndex = Math.Min(FrameIndex + 1, Animation.FrameCount - 1);

                }

            }

            
            // The new rectangle after moving on to the next frame
            Rectangle source;

            // Using the frame index, we can access the frame by multiplying
            // it with the frame width
            source = new Rectangle(FrameIndex * Animation.FrameWidth, 0,
                    Animation.FrameWidth, Animation.FrameHeight);
            
            // Draw the frame
            spriteBatch.Draw(Animation.Texture, position, source, Color.White,
                rotation, Origin, scale, spriteEffects, 0.0f);

        }

    }
}

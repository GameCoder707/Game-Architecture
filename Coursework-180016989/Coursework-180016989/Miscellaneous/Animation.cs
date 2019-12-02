using Microsoft.Xna.Framework.Graphics;

namespace Coursework_180016989
{
    class Animation
    {
        // Used in getting the frame width
        // by manually dividing the texture
        // by the number
        private int slicing_number;
        public int SlicingNumber { get { return slicing_number; } }

        // The complete sprite sheet
        public Texture2D Texture { get; set; }

        // Time taken to stay in a frame
        public float FrameTime { get; set; }

        // If the animation should be looped
        public bool IsLooping { get; set; }

        // The number of frames in the sprite sheet
        public int FrameCount { get { return Texture.Width / FrameWidth; } }


        public int FrameWidth
        {
            get
            {
                // If the the width and height of the 
                // frame is equal then just return the height
                // of the texture
                if (slicing_number == 0)
                {
                    return Texture.Height;
                }
                // Else manually divide the
                // texture by the slicing number
                else
                {
                    return Texture.Width / slicing_number;
                }
            }
            
        }

        // The height of the frame
        public int FrameHeight { get { return Texture.Height; } }


        public Animation(Texture2D texture, float frameTime, bool isLooping, int s_num)
        {
            Texture = texture;
            FrameTime = frameTime;
            IsLooping = isLooping;
            slicing_number = s_num;
        }

    }
}
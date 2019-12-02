using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework_180016989
{
    enum PlatformType
    {
        Rigid = 0,
        Float = 1
    }

    class Platform : Collidable
    {
        Texture2D texture2D;
        PlatformType p_type;
        Vector2 p_position;

        private ContentManager content;

        Rectangle bounds;

        public Platform(IServiceProvider services)
        {
            content = new ContentManager(services) { RootDirectory = "Content" };
        }

        public void Initialise(PlatformType type, Vector2 position)
        {
            if(type == PlatformType.Rigid)
                texture2D = content.Load<Texture2D>("Graphics/World/Ground");
            

            p_type = type;
            p_position = position;

            bounds = new Rectangle((int)p_position.X, (int)p_position.Y, texture2D.Width, texture2D.Height);

        }

        public void UnloadContent()
        {
            content.Unload();
        }

        public override Vector2 GetPosition() { return p_position; }

        public override Rectangle GetRectangle() { return bounds; }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture2D, p_position, GetRectangle(), Color.White, 0.0f,
                Vector2.Zero, new Vector2(2.0f, 1.5f), SpriteEffects.None, 0.0f);
        }

    }
}

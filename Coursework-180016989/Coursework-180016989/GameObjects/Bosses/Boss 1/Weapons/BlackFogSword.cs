using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Coursework_180016989
{

    class BlackFogSword : Collidable
    {
        // Image of the weapon
        private Texture2D texture2D;

        private Vector2 position;
        public override Vector2 GetPosition() { return position; }

        private float rotation;
        private float Rotation { get { return rotation; } }

        // Origin is set to the bottom of
        // the texture to make it rotate around that point/axis
        private Vector2 Origin { get { return new Vector2(texture2D.Width / 2.0f, texture2D.Height); } }

        private int damage;
        public int Damage { get { return damage; } }

        // If the weapon has already attacked the player
        public bool Attacked;

        // If the sword is currently swinging
        public bool Swinging;

        private Rectangle bounds;
        public override Rectangle GetRectangle() { return bounds; }

        public void Initialise(ContentManager content, Vector2 bf_position)
        {
            texture2D = content.Load<Texture2D>("Graphics/Boss/Weapons/SmallSword");

            damage = 10;

            bounds = new Rectangle(texture2D.Bounds.X, texture2D.Bounds.Y, texture2D.Width, texture2D.Height);

            position = bf_position;
            rotation = 0.0f;

            Attacked = false;
            Swinging = false;

        }

        public void Update(float elapsed, Vector2 bf_position)
        {

            position = bf_position;

            // The sword will start rotating
            if (Swinging)
            {
                rotation += (8.0f * elapsed);

                // If it has rotated a full 90 degrees,
                // then stop swinging
                if (MathHelper.ToDegrees(rotation) > 90.0f)
                {
                    Swinging = false;
                    Attacked = false;
                    rotation = 0;
                }
            }
        }

        public override bool CollisionTest(Collidable obj)
        {
            // Circle Collision Test with the player
            if (obj is Player player)
                return Vector2.DistanceSquared(GetPosition(), player.GetPosition()) <= (175.0f * 175.0f);

            return false;

        }

        public override void OnCollision(Collidable obj)
        {
            // To hit the player once
            if (obj is Player player)
            {
                if (Swinging == true &&
                    Attacked == false)
                    Attacked = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, float dir)
        {
            // Draw only when it's swinging
            if (Swinging)
            {
                // Based on the direction the boss is facing,
                // the sword texture is flipped appropriately
                if (dir == 1)
                    spriteBatch.Draw(texture2D, GetPosition(), GetRectangle(), Color.White, rotation, Origin, 1.5f, SpriteEffects.None, 0.0f);
                else
                    spriteBatch.Draw(texture2D, GetPosition(), GetRectangle(), Color.White, -rotation, Origin, 1.5f, SpriteEffects.FlipHorizontally, 0.0f);
            }

        }        

    }
}

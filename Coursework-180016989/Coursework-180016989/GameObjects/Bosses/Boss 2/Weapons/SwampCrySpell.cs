using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Coursework_180016989
{
    class SwampCrySpell : Collidable
    {
        private Vector2 position;
        public override Vector2 GetPosition() { return position; }

        // Image of the spell
        private Texture2D texture;

        // Direction it will move 
        int direction;

        private float speed;

        private int damage;
        public int Damage { get { return damage; } }

        // Origin is kept at the center
        private Vector2 Origin
        {
            get { return new Vector2(texture.Width / 2, texture.Height / 2); }
        }

        public bool Thrown;
        public bool InitialPositionSet;

        // Dimensions of the texture
        private Rectangle bounds;

        public override Rectangle GetRectangle() { return bounds; }

        public void Initialise(ContentManager content)
        {
            texture = content.Load<Texture2D>("Graphics/Boss/Weapons/SwampCrySpell");

            position = Vector2.Zero;

            damage = 20;

            speed = 1100;
            
            bounds = new Rectangle(texture.Bounds.X, texture.Bounds.Y, texture.Width, texture.Height);

            InitialPositionSet = false;

        }

        public void Update(float elapsed, Vector2 sc_position,
            int dir, GraphicsDevice graphicsDevice)
        {

            // If it's thrown by the boss
            if (Thrown == true)
            {
                // Set the initial position
                if (InitialPositionSet == false)
                {
                    position = sc_position;
                    InitialPositionSet = true;
                    direction = dir;
                }

                // Move along the direction
                position.X += (direction * speed * elapsed);

                // If it has reached out of bounds,
                // then reset it
                if (position.X < graphicsDevice.Viewport.X - 50.0f ||
                    position.X > graphicsDevice.Viewport.Width + 50.0f)
                {
                    Thrown = false;
                    InitialPositionSet = false;
                }

            }
            else
            {
                position = sc_position;
            }

        }

        public override bool CollisionTest(Collidable obj)
        {
            // Circle collision test with the player
            if (obj is Player player)
                return Vector2.DistanceSquared(GetPosition(), player.GetPosition()) <= (100.0f * 100.0f);
            

            return false;

        }

        public override void OnCollision(Collidable obj)
        {
            // Reset it upon collision
            if (obj is Player player)
            {
                Thrown = false;
                InitialPositionSet = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw only when it's thrown
            if (Thrown == true)
            {
                spriteBatch.Draw(texture, GetPosition(), GetRectangle(), Color.White, 0.0f, Origin, 1.0f, SpriteEffects.None, 0.0f);
            }
        }

    }

}

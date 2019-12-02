using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Coursework_180016989
{
    class BlackFogShockwave : Collidable
    {
        // Image of the wave
        Texture2D texture2D;

        private Vector2 position;

        public override Vector2 GetPosition() { return position; }

        // Origin placed at the center
        private Vector2 Origin { get { return new Vector2(texture2D.Width / 2, texture2D.Height / 2); } }

        private int damage = 15;
        public int Damage { get { return damage; } }

        // The speed at which it moves
        float speed = 1000.0f;

        // If the wave is made to move
        public bool Pounded;

        // Starting position of the wave
        public bool InitialPositionSet;

        // Dimensions of the texture
        private Rectangle bounds;

        public override Rectangle GetRectangle() { return bounds; }

        public void Initialise(ContentManager content)
        {
            texture2D = content.Load<Texture2D>("Graphics/Boss/Weapons/SlashFX");

            position = Vector2.Zero;

            bounds = new Rectangle(texture2D.Bounds.X, texture2D.Bounds.Y,
                texture2D.Width, texture2D.Height);

            InitialPositionSet = false;

        }

        public void Update(float elapsed, Vector2 bf_position,
            float direction, GraphicsDevice graphicsDevice)
        {

            if (Pounded == true)
            {
                // To start from the boss's position
                if(InitialPositionSet == false)
                {
                    position = bf_position;
                    InitialPositionSet = true;
                }

                // Direction depends on the left/right wave
                position.X += (direction * speed * elapsed);

                // If it has gone out of bounds, then reset it
                if(position.X < graphicsDevice.Viewport.X - 50.0f ||
                    position.X > graphicsDevice.Viewport.Width + 50.0f)
                {
                    Pounded = false;
                    InitialPositionSet = false;
                }


            }
            else
            {
                position = bf_position;
            }

        }

        public override bool CollisionTest(Collidable obj)
        {
            // Circle Collision test with the player
            if (obj is Player player)
                return Vector2.DistanceSquared(GetPosition(), player.GetPosition()) <= (100.0f * 100.0f);

            return false;
        }

        public override void OnCollision(Collidable obj)
        {
            if (obj is Player player)
            {
                // If the wave is moving and it
                // has hit the player, then reset it
                if(Pounded == true)
                {
                    Pounded = false;
                    InitialPositionSet = false;
                }
                
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteEffects effects)
        {
            // Draw only when it's moving
            if(Pounded == true)
            {
                spriteBatch.Draw(texture2D, GetPosition(), GetRectangle(), Color.White, 0.0f, Origin, 0.3f, effects, 0.0f);
            }
            
        }

    }
}
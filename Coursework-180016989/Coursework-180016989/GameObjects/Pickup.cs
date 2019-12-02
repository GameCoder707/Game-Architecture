using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework_180016989
{
    // It would be either health or magic pickup
    enum PickupType
    {
        Health = 0,
        Magic = 1
    }

    class Pickup : Collidable
    {
        // The strength of refill
        enum RefillType
        {
            Mini = 0,
            Neutral = 1,
            Mega = 2
        }

        // To randomize the strength of refill
        Random rand = new Random();

        // Texture of the pickup
        Texture2D texture2D;

        // Local Content Manager to load textures
        private ContentManager content;

        // Used by player to refill the
        // appropriate category
        private PickupType p_type;
        public PickupType GetPickupType() { return p_type; }

        private RefillType r_type;

        // Points used to refill a particular bar
        int refill_points;
        public int GetRefillPoints() { return refill_points; }


        private Vector2 position;
        public void SetPosition(Vector2 value) { position = value; }
        public override Vector2 GetPosition() { return position; }

        Rectangle bounds;
        public override Rectangle GetRectangle() { return bounds; }

        private float fallSpeed;

        private float appearTimer;
        private float appearDelay;

        // A random X position to fall from
        private bool InitialPosSet;

        private float startingPosY;

        public Pickup(IServiceProvider services)
        {
            content = new ContentManager(services) { RootDirectory = "Content" };
        }
        

        public void Initalise(GraphicsDevice graphicsDevice, PickupType type)
        {
            switch (type)
            {
                case PickupType.Health:
                    texture2D = content.Load<Texture2D>("Graphics/World/health pickup");
                    appearDelay = 6.0f;

                    break;
                case PickupType.Magic:
                    texture2D = content.Load<Texture2D>("Graphics/World/mana pickup");
                    appearDelay = 5.0f;
                    break;

            }

            p_type = type;
            r_type = RefillType.Neutral;

            InitialPosSet = false;

            position = Vector2.Zero;

            // Placing it above the window
            startingPosY = graphicsDevice.Viewport.Y - 100;
            position.Y = startingPosY;

            bounds = new Rectangle(texture2D.Bounds.X, texture2D.Bounds.Y, texture2D.Width, texture2D.Height);

            appearTimer = 0.0f;

            fallSpeed = 200.0f;

        }

        public void Reset()
        {
            position.Y = startingPosY;
            appearTimer = 0.0f;
        }

        public void UnloadContent()
        {
            content.Unload();
        }

        public void Update(float elapsed, GraphicsDevice graphicsDevice)
        {

            // If the timer is up, then initiate the position
            // and make it fall down
            if (appearTimer >= appearDelay)
            {
                if (InitialPosSet == false)
                {
                    // To initiate a new value
                    // at each tick
                    rand = new Random();

                    // Get a random value within the window width
                    position.X = rand.Next(graphicsDevice.Viewport.X + 50,
                        graphicsDevice.Viewport.Width - 50);
                    position.Y = startingPosY;

                    // Get a random type
                    int type = rand.Next(0, 3);

                    // The amount to refill is adjusted 
                    // by the type
                    switch ((RefillType)type)
                    {
                        case RefillType.Mini:
                            refill_points = 5;
                            r_type = RefillType.Mini;
                            break;
                        case RefillType.Neutral:
                            refill_points = 10;
                            r_type = RefillType.Neutral;
                            break;
                        case RefillType.Mega:
                            refill_points = 15;
                            r_type = RefillType.Mega;
                            break;
                    }

                    // The position is now set
                    InitialPosSet = true;

                }

                // Make the pickup fall down
                position.Y += (fallSpeed * elapsed);

                // Reset it once it goes below the screen
                if (GetPosition().Y > graphicsDevice.Viewport.Height + 50.0f)
                {
                    position.Y = startingPosY;
                    appearTimer = 0.0f;
                    InitialPosSet = false;
                }
            }
            else
            {
                appearTimer += elapsed;
            }



        }

        public override bool CollisionTest(Collidable obj)
        {
            // Perform the collision test
            if (obj is Player player)
                return Vector2.DistanceSquared(GetPosition(), player.GetPosition()) <= (100.0f * 100.0f);
            
            return false;

        }

        public override void OnCollision(Collidable obj)
        {
            if (obj is Player player)
            {
                // Reset upon collision
                position.Y = startingPosY;
                appearTimer = 0.0f;
                InitialPosSet = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // The color is determined by its type
            switch (r_type)
            {
                case RefillType.Mega:
                    spriteBatch.Draw(texture2D, position, Color.White);
                    break;
                case RefillType.Neutral:
                    spriteBatch.Draw(texture2D, position, Color.LightGray);
                    break;
                case RefillType.Mini:
                    spriteBatch.Draw(texture2D, position, Color.Gray);
                    break;

            }

        }

    }
}

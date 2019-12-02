using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Coursework_180016989
{
    class Boss : Collidable
    {
        // To derive the viewport width and height
        protected GraphicsDevice graphicsDevice;

        // The state machine of the boss
        protected FSM fsm;

        // Animator object to play
        // the different animations
        protected Animator sprite;

        // Boss's Content Manager
        protected ContentManager content;

        // Speed at which the boss moves
        protected float movementSpeed;

        // Starting Position
        protected Vector2 startingPosition;

        // Player Position
        protected Vector2 playerPosition;

        // A random object to randomly
        // switch between the states
        protected Random rand;

        // Destination postion for moves
        protected Vector2 dest;

        // Velocity for all movement actions
        protected Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
        }

        protected Vector2 position;
        public override Vector2 GetPosition()
        {
            return position;
        }

        // Rectangle of the frame in sprite sheet
        protected Rectangle localBounds;
        public override Rectangle GetRectangle()
        {
            int left = (int)Math.Round(position.X - sprite.Origin.X) + localBounds.X;
            int top = (int)Math.Round(position.Y - (sprite.Origin.Y * 1.45)) + localBounds.Y;

            return new Rectangle(left, top, localBounds.Width, localBounds.Height);
        }

        // Time to switch between states
        protected float FSMWaitTime;
        protected float FSMWaitDelay;

        // Flip the frame texture based on
        // the direction the boss is moving
        protected SpriteEffects flip;
        protected int direction;

        protected int health;
        protected int maxHealth;

        // To check if the boss
        // is on the ground
        protected bool onGround;

        // Set and Get values for Health

        public void SetHealth(int value)
        {
            health = value;
        }

        public int GetHealth()
        {
            return health;
        }

        public int GetMaxHealth()
        {
            return maxHealth;
        }

        // Functions overrided by the bosses

        public virtual void Update(float elapsed, GraphicsDevice graphicsDevice, Vector2 p_Position) { }

        public virtual void Reset() { }

        public virtual void UnloadContent() { }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }

    }
}

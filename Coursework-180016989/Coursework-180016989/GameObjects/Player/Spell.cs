using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Coursework_180016989
{

    // The different spell types
    enum SpellType
    {
        Power = 0,
        Fire = 1,
        Ice = 2
    }

    // Inherits from Collidable for Collision Events
    class Spell : Collidable
    {
        // Delegate function for weapon accuracy event
        public delegate void ScoreActionBool(bool value);

        // Content Manager to load textures
        ContentManager content;

        // The position of the spell
        private Vector2 position;
        public override Vector2 GetPosition()
        {
            return position;
        }

        // A sound played when the spell is casted
        private SoundEffect castSound;

        // The direction the spell is moving towards
        private Vector2 direction;

        // A single animation is played
        private Animation spellAnimation;

        // Sprite sheet of the spell
        private Animator sprite;

        // Speed at which it travels
        private float movementSpeed;

        // Boolean to set the direction once
        private bool initiateDirection;
        public bool InitiateDirection
        {
            get { return initiateDirection; }
            set { initiateDirection = value; }
        }

        // If the spell is casted or not
        public bool SpellCasted;

        // Rotation value of the spell
        private float rotation;

        // Rectangle dimensions of the
        // frame in the sprite sheet
        private Rectangle localBounds;

        public override Rectangle GetRectangle()
        {
            int left = (int)Math.Round(GetPosition().X - sprite.Origin.X) + localBounds.X;
            int top = (int)Math.Round(GetPosition().Y - sprite.Origin.Y) + localBounds.Y;

            return new Rectangle(left, top, localBounds.Width, localBounds.Height);

        }

        // The amount of Magic needed 
        // to cast this spell
        private int mpCost;
        public int MPCost { get { return mpCost; } }

        // The amount of damage it
        // deals to the boss
        private int damage;
        public int GetDamage() { return damage; }

        // A delegate function to invoke the weapon
        // accuracy function in the Player class
        public ScoreActionBool WeaponAccuracy;

        public Spell(IServiceProvider services)
        {
            content = new ContentManager(services) { RootDirectory = "Content" };
        }

        public void Initialise(Vector2 position, SpellType type)
        {
            switch (type)
            {
                // The Power spell doesn't cost any Magic.
                // It's the basic attack
                case SpellType.Power:
                    spellAnimation = new Animation(content.Load<Texture2D>
                        ("Graphics/Player/Weapons/Powerball"), 0.1f, true, 6);
                    mpCost = 0;
                    damage = 5;
                    break;

                // Costs 1 Magic Point and deals
                // more damage
                case SpellType.Fire:
                    spellAnimation = new Animation(content.Load<Texture2D>
                        ("Graphics/Player/Weapons/Fireball"), 0.1f, true, 6);
                    mpCost = 1;
                    damage = 10;
                    break;

                // Costs 2 Magic Points and
                // deals the maximum damage
                case SpellType.Ice:
                    spellAnimation = new Animation(content.Load<Texture2D>
                        ("Graphics/Player/Weapons/Iceball"), 0.1f, true, 6);
                    mpCost = 2;
                    damage = 15;
                    break;
            }

            castSound = content.Load<SoundEffect>("Audio/spell_cast");

            direction = new Vector2();
            this.position = position;
            rotation = 0.0f;

            // Initialising the rectangle dimesions of the frame
            int width = (int)(spellAnimation.FrameWidth * 0.4);
            int left = (spellAnimation.FrameWidth - width) / 2;
            int height = (int)(spellAnimation.FrameWidth * 0.1);
            int top = spellAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            InitiateDirection = false;
            SpellCasted = false;

            movementSpeed = 1000.0f;

            sprite = new Animator(OriginLocation.Center);
            sprite.PlayAnimation(spellAnimation);

        }

        public void Reset()
        {
            // Reset values
            InitiateDirection = false;
            SpellCasted = false;
        }

        public void UnloadContent() { content.Unload(); }

        //**************************************************************
        //********************COLLISION EVENTS**************************
        //**************************************************************

        public override bool CollisionTest(Collidable obj)
        {
            if (obj is Boss boss)
            {
                if (SpellCasted == true)
                    return (Vector2.DistanceSquared(GetPosition(), boss.GetPosition()) <= (100.0f * 100.0f));
            }

            return false;

        }

        public override void OnCollision(Collidable obj)
        {
            if (obj is Boss boss)
            {
                Reset();

                // Passing in true as in this spell
                // has hit the boss and is a successful cast
                WeaponAccuracy?.Invoke(true);
            }

        }

        //**************************************************************
        //*****************END OF COLLISION EVENTS**********************
        //**************************************************************

        public void Update(float elapsed, MouseState mouse_state, Vector2 playerPosition, GraphicsDevice graphicsDevice)
        {

            if (SpellCasted == true)
            {
                // If the initial position isn't set,
                // then do it once
                if (InitiateDirection == false)
                {
                    // Obtaining the mouse position from the state
                    Vector2 mousePosition = new Vector2(mouse_state.X, mouse_state.Y);

                    position = playerPosition;

                    // To fire from above the player
                    position.Y -= 50.0f;

                    // Using the mouse postion to derive
                    // the direction vector
                    direction = mousePosition - position;

                    direction.Normalize();

                    // Using the arc tangent of the direction vector
                    // we can obtain the rotation value
                    rotation = (float)Math.Atan2(direction.Y, direction.X);

                    castSound.Play();

                    InitiateDirection = true;

                }

                // Moving along the direction by the speed
                position += (direction * movementSpeed * elapsed);
                sprite.PlayAnimation(spellAnimation);

                // If the spell goes out of bounds
                if (position.X > graphicsDevice.Viewport.Width + 50.0f ||
                    position.X < graphicsDevice.Viewport.X - 50.0f ||
                    position.Y > graphicsDevice.Viewport.Height + 50.0f ||
                    position.Y < graphicsDevice.Viewport.Y - 50.0f)
                {
                    Reset();

                    // The hit was missed, so passing in false
                    // for a unsuccessful cast
                    WeaponAccuracy?.Invoke(false);
                }

            }

        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (SpellCasted == true)
            {
                sprite.Draw(gameTime, spriteBatch, GetPosition(),
                    rotation, 0.3f, SpriteEffects.FlipHorizontally);
            }
        }



    }

}

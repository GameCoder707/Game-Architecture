using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Coursework_180016989
{
    // Player inherits from Collidable for Collisions
    class Player : Collidable
    {
        // Content Manager
        private ContentManager content;

        // Animation for Idle, Run and Jump actions
        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation jumpAnimation;
        private Animation dieAnimation;

        // The animation currently displaying
        private Animation currentAnimation;

        // Animator object to play the animation
        private Animator sprite;

        // To flip the image based on the direction
        // the player is moving
        private SpriteEffects flip;

        // The position of the player and
        // its previous position for animation
        // swap states
        private Vector2 position;
        private Vector2 previousPosition;

        // Used to reset the player's
        // position when resetting the level
        private Vector2 startingPosition;

        // Overriding the Get Position for Collision Tests
        public override Vector2 GetPosition() { return position; }

        public void SetPosition(Vector2 pos) { position = pos; }

        // The dimensions of the player's sprite
        private Rectangle localBounds;

        // Get the rectangle dimensions of the
        // frame in the sprite sheet
        public override Rectangle GetRectangle()
        {
            int left = (int)Math.Round(position.X - sprite.Origin.X) + localBounds.X;
            int top = (int)Math.Round(position.Y - sprite.Origin.Y * 1.45f) + localBounds.Y;

            return new Rectangle(left, top, localBounds.Width, localBounds.Height);
        }

        // Velocity affected by gravity, move acceleration, etc.
        Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        // Movement speed of the player
        private const float MoveAcceleration = 26000.0f;

        // Max Movement speed
        private const float MaxMoveSpeed = 5000.0f;

        // Drag factor to control the speed and slow
        // the player down when not moving
        private const float XDrag = 0.48f;

        // Jump speed of the player
        private const float JumpVelocity = -49000.0f;

        // Time the player will spend jumping
        private float jumpMaxTime = 0.27f;
        private float jumpTime = 0.0f;

        // Speed of the dash move
        private const float DashSpeed = 90000.0f;
        // Drag factor to control the speed
        private const float XDashDrag = 0.50f;
        private float dashTime;

        // Falling speed, max falling speed and air drag
        // to control the fall speed
        private const float GravityAcceleration = 3900.0f;
        private const float MaxFallSpeed = 900.0f;
        private const float AirDrag = 0.9f;

        // Movement direction
        private int movementDirection;
        private int dashDirection;

        // Various booleans
        private bool isJumping;
        private bool onGround;
        private bool isDashing;
        private bool canDash;

        // A boolean to indicate if a hit is taken
        // and time the player stays invulnerable
        private bool hitTaken;
        private float invulnerableTimer;
        private float invulnerableDuration;

        // The number of spells fired from the player's wand
        private float spellsFired;
        public float SpellsFired { get { return spellsFired; } }

        // The number of spells which hit the boss
        private float spellsHit;
        public float SpellsHit { get { return spellsHit; } }

        // Weapon accuracy calculated by dividing the
        // number of spells hit by the total number
        // of spells casted and multiplying by 100 to
        // obtain a percentage value and
        // Rounding it up to have
        // at least 1 decimal value
        public float WeaponAccuracy
        {
            get
            {
                // Returning 0 if no spells are casted
                // Else it will return NaN value
                if (spellsFired == 0)
                    return 0;
                else
                    return (float)Math.Round((SpellsHit / SpellsFired) * 100, 1);
            }
        }

        // To blink the player continuously
        private bool canRender;
        private float blinkTimer;
        private float blinkDelay;

        // The health points of the player
        private int health;
        public int Health
        {
            set { health = value; }
            get { return health; }
        }

        // Maximum health points of the player 
        private const int maxHealth = 100;
        public int MaxHealth { get { return maxHealth; } }

        // The magic points used for casting
        // spells
        private int magic;
        public int Magic
        {
            get { return magic; }
            set { magic = value; }
        }

        // Maximum magic points of the player
        private const int maxMagic = 50;
        public int MaxMagic { get { return maxMagic; } }

        public Player(IServiceProvider services)
        {
            content = new ContentManager(services) { RootDirectory = "Content" };
        }

        public void UnloadContent()
        {
            content.Unload();
        }

        public void Initialise(Vector2 position, GraphicsDevice graphics)
        {
            // Loading in player animation sprite sheets
            idleAnimation = new Animation(content.Load<Texture2D>("Graphics/Player/VioletWizardIdle"), 0.15f, true, 0);
            runAnimation = new Animation(content.Load<Texture2D>("Graphics/Player/VioletWizardRun"), 0.1f, true, 4);
            jumpAnimation = new Animation(content.Load<Texture2D>("Graphics/Player/VioletWizardJump"), 0.1f, false, 0);
            dieAnimation = new Animation(content.Load<Texture2D>("Graphics/Player/VioletWizardDying"), 0.1f, false, 4);

            // Initialising all the position values
            startingPosition = position;
            SetPosition(startingPosition);
            previousPosition = GetPosition();

            // Rectangle dimensions of the frame
            // in idleAnimation sprite sheet
            int width = (int)(idleAnimation.FrameWidth * 0.4f);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.1f);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            hitTaken = false;
            invulnerableDuration = 0.3f;
            invulnerableTimer = 0.0f;

            canRender = true;
            blinkDelay = 0.09f;
            blinkTimer = blinkDelay;

            // Set the health and magic
            // values from the XML file
            health = SaveInfo.Instance.PlayerHealth;
            magic = SaveInfo.Instance.PlayerMagic;
            spellsFired = SaveInfo.Instance.SpellsFired;
            spellsHit = SaveInfo.Instance.SpellsHit;

            dashTime = 0.0f;
            dashDirection = 1;

            // Because we start the level by falling down
            currentAnimation = jumpAnimation;

            // We set the origin to be at the center
            // of the frame
            sprite = new Animator(OriginLocation.Center);
            sprite.PlayAnimation(currentAnimation);

        }

        // Resetting all the values
        // when the level resets
        public void Reset()
        {
            Health = MaxHealth;
            Magic = MaxMagic;

            hitTaken = false;
            isDashing = false;
            isJumping = false;
            onGround = false;
            canRender = true;
            canDash = true;

            dashTime = 0.0f;

            dashDirection = 1;

            blinkTimer = blinkDelay;
            invulnerableTimer = 0.0f;

            spellsFired = 0;
            spellsHit = 0;

            currentAnimation = jumpAnimation;

            velocity = Vector2.Zero;

            SetPosition(startingPosition);

        }

        public void Update(float elapsed, KeyboardState key_state, GraphicsDevice graphicsDevice)
        {

            // If the player has taken a hit, then update
            // the invulnerable timer
            if (hitTaken == true)
            {
                if (invulnerableTimer >= invulnerableDuration)
                {
                    hitTaken = false;
                    canRender = true;
                    invulnerableTimer = 0.0f;
                    blinkTimer = blinkDelay;
                }
                else
                {
                    invulnerableTimer += elapsed;

                    // And blink the player by swapping
                    // the bool continuously
                    if (blinkTimer >= blinkDelay)
                    {
                        canRender = !canRender;
                        blinkTimer = 0.0f;
                    }
                    else
                    {
                        blinkTimer += elapsed;
                    }
                }
            }

            // Apply physics for all movement actions
            ApplyPhysics(elapsed);

            // Clamping the position between the width of the game window
            position.X = MathHelper.Clamp(position.X, localBounds.Width / 3,
                graphicsDevice.Viewport.Width - localBounds.Width / 3);

            // If we're not alive, then play the die animation
            if (Health > 0)
            {
                // Play jumping animation if the player is jumping
                if (isJumping == true)
                    currentAnimation = jumpAnimation;

                // Play the idle animaton if the player is on the ground 
                // and the position hasn't changed from the previous frame
                else if (previousPosition == GetPosition() && onGround)
                    currentAnimation = idleAnimation;

                // Play the run animation if the position has changed
                // from the previous frame
                else if (previousPosition != GetPosition() && onGround)
                    currentAnimation = runAnimation;
            }
            else
                currentAnimation = dieAnimation;

            sprite.PlayAnimation(currentAnimation);

            // Set the previous position with the current one
            previousPosition = GetPosition();

            // Resetting movement values
            movementDirection = 0;

        }
     

        //**************************************************************
        //***********************INPUT EVENTS***************************
        //**************************************************************

        // A delegate function for moving left
        public void MoveLeft(eButtonState buttonState, Vector2 amount)
        {
            // If the button is held down, and if the player is not
            // dashing, then set the direction values
            if (buttonState == eButtonState.DOWN && isDashing == false)
            {
                movementDirection = -1;
                dashDirection = -1;
            }

        }

        public void MoveRight(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.DOWN && isDashing == false)
            {
                movementDirection = 1;
                dashDirection = 1;
            }

        }

        // If the dash button is pressed once, check if the player
        // can dash and if he's already dashing. If all true,
        // then enable the player to dash
        public void Dash(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED &&
                canDash == true &&
                isDashing == false && Health > 0)
            {
                isDashing = true;
                canDash = false;
            }

        }

        // Make the player jump if the player is on the ground,
        // not dashing, not already jumping
        public void Jump(eButtonState buttonState, Vector2 amount)
        {
            if (buttonState == eButtonState.PRESSED &&
                onGround == true &&
                isDashing == false && Health > 0)
                isJumping = true;
        }

        //**************************************************************
        //*****************END OF INPUT EVENTS**************************
        //**************************************************************

        // A function subscribed to the event sent by the spells
        // If the spell is hit, passes in true value and updates the
        // spells hit variable. Total spells casted is calculated by default
        public void CalculateWeaponAccuracy(bool value)
        {
            if (value == true)
                spellsHit++;

            spellsFired++;
        }

        // Function that manages all the movement actions
        public void ApplyPhysics(float elapsed)
        {

            if (isDashing == true)
            {
                // Resetting Jump values
                isJumping = false;
                velocity.Y = 0;
                jumpTime = 0.0f;

                // If the dash timer is up,
                // then stop dashing
                if (dashTime >= 0.2f)
                {
                    isDashing = false;
                    dashTime = 0.0f;
                }
                else if (Health > 0)
                {
                    // Increasing the velocity by the dash speed
                    velocity.X += (dashDirection * DashSpeed * elapsed);

                    // And controlling it to avoid very fast movement
                    velocity.X *= XDashDrag;

                    dashTime += elapsed;
                }

            }
            // Else move the player normally by applying movement speed
            // and controlling it when alive
            else if (Health > 0)
            {
                velocity.X += (movementDirection * MoveAcceleration * elapsed);

                velocity.X *= XDrag;

                velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);
            }

            // Make the player move up if the player is not dashing
            if (isJumping == true && isDashing == false && Health > 0)
            {
                velocity.Y = (JumpVelocity * elapsed);

                jumpTime += elapsed;

                if (jumpTime >= jumpMaxTime)
                {
                    isJumping = false;
                    jumpTime = 0.0f;
                }

            }
            // And make the player move down when not dashing
            // and done with jumping
            else if ((onGround == false &&
                isJumping == false &&
                isDashing == false) || (Health <= 0 && onGround == false))
            {
                velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
            }


            // Set the position based on the velocity values
            SetPosition(GetPosition() + (velocity * elapsed));
            SetPosition(new Vector2((float)Math.Round(GetPosition().X), (float)Math.Round(GetPosition().Y)));

            // Reset the velocity if the player isn't
            // moving in any axis
            if (GetPosition().X == previousPosition.X || Health <= 0)
                velocity.X = 0;

            if (GetPosition().Y == previousPosition.Y)
                velocity.Y = 0;

        }

        //**************************************************************
        //********************COLLISION EVENTS**************************
        //**************************************************************

        // Function for collision tests with other collidable objects
        public override bool CollisionTest(Collidable obj)
        {

            if (obj is Platform platform)
            {
                // If player isn't colliding with the platform,
                // then the player is no longer on the ground
                if (GetRectangle().Intersects(platform.GetRectangle()) == false)
                {
                    onGround = false;
                    return false;
                }
                else
                    return true;
            }

            // Basic square distance-based Collision checks with rest of the collidables

            // The player shouldn't collide with any pick ups upon dying
            else if (obj is Pickup pickup && Health > 0)
                return Vector2.DistanceSquared(GetPosition(), pickup.GetPosition()) <= (100.0f * 100.0f);
            

            if (hitTaken == false && Health > 0)
            {
                if (obj is Boss boss)
                {
                    if(boss.GetHealth() > 0)
                    return Vector2.DistanceSquared(GetPosition(), boss.GetPosition()) <= (130.0f * 130.0f);
                }
                else if (obj is BlackFogSword sword)
                {
                    return Vector2.DistanceSquared(GetPosition(), sword.GetPosition()) <= (175.0f * 175.0f);
                }
                else if (obj is BlackFogShockwave wave)
                {
                    return Vector2.DistanceSquared(GetPosition(), wave.GetPosition()) <= (100.0f * 100.0f);
                }
                else if (obj is SwampCryClub club)
                {
                    return Vector2.DistanceSquared(GetPosition(), club.GetPosition()) <= (175.0f * 175.0f);
                }
                else if (obj is SwampCrySpell spell)
                {
                    return Vector2.DistanceSquared(GetPosition(), spell.GetPosition()) <= (100.0f * 100.0f);
                }

            }

            return false;

        }

        // Function for collision responses with other collidables
        public override void OnCollision(Collidable obj)
        {

            if (obj is Platform platform)
            {
                if (onGround == false)
                {
                    // Setting the appropriate values upon collision
                    onGround = true;

                    velocity.Y = 0;

                    // Calculating depth to resolve collisions
                    // Method used from Platformer Game from Microsoft XNA Community Game Platform
                    Vector2 depth = RectangleExtensions.GetIntersectionDepth(GetRectangle(), platform.GetRectangle());

                    // If there is some depth,
                    // then push the player appropriately in the Y axis
                    if (depth != Vector2.Zero)
                        position = new Vector2(position.X, position.Y + depth.Y);

                }

                // The dash ability resets when 
                // the player is on the ground
                canDash = true;

            }
            else if (obj is Pickup pickup)
            {
                switch (pickup.GetPickupType())
                {
                    // Refill Health or Magic based on the pickup type
                    case PickupType.Health:
                        if (Health + pickup.GetRefillPoints() <= MaxHealth)
                            Health += pickup.GetRefillPoints();
                        else
                            Health = MaxHealth;
                        break;
                    case PickupType.Magic:
                        if (Magic + pickup.GetRefillPoints() <= MaxMagic)
                            Magic += pickup.GetRefillPoints();
                        else
                            Magic = MaxMagic;
                        break;
                }
            }
            else if (obj is Boss boss)
            {
                Health -= 10;
                hitTaken = true;

            }
            else if (obj is BlackFogSword sword)
            {
                // Take damage only if the player
                // hasn't taken a hit and if the
                // sword is swinging
                if (sword.Attacked == false &&
                        sword.Swinging == true)
                {
                    Health -= sword.Damage;
                    hitTaken = true;
                }

            }
            else if (obj is BlackFogShockwave wave)
            {
                // Take damage only when 
                // the shockwave is moving
                if (wave.Pounded == true)
                {
                    Health -= wave.Damage;
                    hitTaken = true;
                }

            }

            // SWAMP CRY COLLISIONS

            else if (obj is SwampCryClub club)
            {
                // Same as the sword, take damage
                // when the club is swinging
                if (club.Attacked == false &&
                    club.Swinging == true)
                {
                    Health -= club.Damage;
                    hitTaken = true;
                }
            }
            else if (obj is SwampCrySpell spell)
            {
                // Take damage when the spell is thrown
                if (spell.Thrown == true)
                {
                    Health -= spell.Damage;
                    hitTaken = true;
                }
            }


        }

        //**************************************************************
        //*****************END OF COLLISION EVENTS**********************
        //**************************************************************

        // Drawing the player
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Flip the texture based on the direction
            // the player is moving 
            if (Velocity.X > 0)
                flip = SpriteEffects.None;
            else if (Velocity.X < 0)
                flip = SpriteEffects.FlipHorizontally;

            // Render only when the boolean is true
            // Used for blinking effect
            if (canRender == true)
                sprite.Draw(gameTime, spriteBatch, GetPosition(), 0.0f, 0.5f, flip);

        }

        

    }
}

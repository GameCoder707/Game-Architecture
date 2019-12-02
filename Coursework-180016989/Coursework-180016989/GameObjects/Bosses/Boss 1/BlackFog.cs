using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Coursework_180016989
{

    class BlackFog : Boss
    {
        // Different Move Sets
        // of Black Fog
        enum MoveSet
        {
            Rampage = 0,
            SwordAttack = 1,
            SlamAttack = 2
        }

        // Used for switching states
        private MoveSet currentMove;

        // Used for randomizing the states
        private int currentMoveNum;

        // All the sprite sheets for the boss
        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation fallAnimation;
        private Animation dieAnimation;

        // All the weapons used by Black Fog
        private BlackFogSword sword;
        private BlackFogShockwave left_wave;
        private BlackFogShockwave right_wave;

        // Sound played when the boss sends a shockwave
        private SoundEffect shockwaveSound;

        // Time taken to jump
        float jumpTime;

        // Gravity attributes for the boss
        private const float GravityAcceleration = 34000.0f;
        private const float MaxFallSpeed = 1200.0f;
        private const float JumpVelocity = -700.0f;

        private bool isJumping;
        private bool haltMovement;
        private bool castShockWave;
        private bool initialPositionSet;

        public BlackFog(IServiceProvider services, GraphicsDevice graphics, int hp = 0)
        {

            graphicsDevice = graphics;

            if (content == null)
                content = new ContentManager(services) { RootDirectory = "Content" };

            fsm = new FSM(this);

            rand = new Random();

            BF_RampageState rampage = new BF_RampageState();
            BF_SwordAttackState swordAttack = new BF_SwordAttackState();
            BF_SlamAttackState slamAttack = new BF_SlamAttackState();

            // Adding transitions for states to all other states
            // if it satisfies the enum condition
            rampage.AddTransition(new Transition(swordAttack, () => currentMove == MoveSet.SwordAttack));
            rampage.AddTransition(new Transition(slamAttack, () => currentMove == MoveSet.SlamAttack));

            swordAttack.AddTransition(new Transition(rampage, () => currentMove == MoveSet.Rampage));
            swordAttack.AddTransition(new Transition(slamAttack, () => currentMove == MoveSet.SlamAttack));

            slamAttack.AddTransition(new Transition(rampage, () => currentMove == MoveSet.Rampage));
            slamAttack.AddTransition(new Transition(swordAttack, () => currentMove == MoveSet.SwordAttack));

            // Add all the states to the state machine
            fsm.AddState(rampage);
            fsm.AddState(swordAttack);
            fsm.AddState(slamAttack);

            fsm.Initialise("Rampage");

            idleAnimation = new Animation(content.Load<Texture2D>("Graphics/Boss/BlackFogIdle"), 0.1f, true, 18);
            runAnimation = new Animation(content.Load<Texture2D>("Graphics/Boss/BlackFogRunning"), 0.06f, true, 12);
            fallAnimation = new Animation(content.Load<Texture2D>("Graphics/Boss/BlackFogFalling"), 0.3f, false, 6);
            dieAnimation = new Animation(content.Load<Texture2D>("Graphics/Boss/BlackFogDying"), 0.08f, false, 15);

            shockwaveSound = content.Load<SoundEffect>("Audio/shockwave_pound");

            movementSpeed = 500.0f;

            startingPosition = new Vector2(graphics.Viewport.Width / 2, 0.0f);
            position = startingPosition;
            dest = Vector2.Zero;

            direction = 1;

            maxHealth = 500;

            // If it's initialised with 0 from the script
            // then set it to the max health
            if (hp == 0)
                health = maxHealth;
            else
                health = hp;

            // Time to wait to switch 
            // to the next state
            FSMWaitDelay = 3.0f;
            FSMWaitTime = FSMWaitDelay;

            // Modified Rectangle dimensions 
            // of the frame for collisions
            int width = (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.1);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            sprite = new Animator(OriginLocation.Center);
            sprite.PlayAnimation(idleAnimation);

            sword = new BlackFogSword();
            left_wave = new BlackFogShockwave();
            right_wave = new BlackFogShockwave();

            sword.Initialise(content, position);
            left_wave.Initialise(content);
            right_wave.Initialise(content);

            haltMovement = false;
            castShockWave = false;
            initialPositionSet = false;

            currentMoveNum = 0;
        }

        public override void Reset()
        {

            position = startingPosition;

            currentMoveNum = 0;

            FSMWaitTime = FSMWaitDelay;

            castShockWave = false;
            sword.Attacked = false;
            sword.Swinging = false;
            left_wave.Pounded = false;
            right_wave.Pounded = false;

            SetHealth(GetMaxHealth());

            sprite.PlayAnimation(fallAnimation);

        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void Update(float elapsed, GraphicsDevice graphicsDevice, Vector2 p_Position)
        {

            // Obtain the player position
            // to be able to move towards him
            playerPosition = p_Position;

            ApplyPhysics(elapsed);

            // If the boss is alive
            if (GetHealth() > 0)
            {
                fsm.Update(elapsed);

                FSMWaitTime += elapsed;

                // If the timer is up,
                // then it's time to switch to
                // a random state
                if (FSMWaitTime >= FSMWaitDelay)
                {
                    currentMove = (MoveSet)currentMoveNum;

                    int previousMoveNum = currentMoveNum;
                    currentMoveNum = rand.Next(0, 3);

                    // To make sure it hasn't randomized
                    // to the same state
                    if (currentMoveNum == previousMoveNum)
                    {
                        currentMoveNum++;
                        currentMoveNum = currentMoveNum % 3;
                    }


                    FSMWaitTime = 0.0f;

                    // To initiate the direction again
                    // when switching to a new state
                    initialPositionSet = false;

                }

            }
            // Else play the die animation
            else
                sprite.PlayAnimation(dieAnimation);

            // To make sure the boss
            // doesn't go out bounds
            position.X = MathHelper.Clamp(position.X, localBounds.Width / 3,
                graphicsDevice.Viewport.Width - localBounds.Width / 3);

            // Update the boss weapons

            sword.Update(elapsed, new Vector2(GetPosition().X, GetPosition().Y + 50.0f));

            left_wave.Update(elapsed, new Vector2(GetPosition().X, GetPosition().Y + 50.0f), -1f, graphicsDevice);
            right_wave.Update(elapsed, new Vector2(GetPosition().X, GetPosition().Y + 50.0f), 1f, graphicsDevice);


        }

        // Managing all movement actions
        public void ApplyPhysics(float elapsed)
        {
            // Make the boss go upward
            // until the timer is up
            if (isJumping == true)
            {
                velocity.Y = (JumpVelocity * elapsed);
                position.Y += velocity.Y;

                jumpTime += elapsed;

                if (jumpTime >= 0.35f)
                {
                    haltMovement = true;
                    isJumping = false;
                    castShockWave = true;
                    jumpTime = 0.0f;
                }
            }
            // Else apply gravity to the boss
            else if (onGround == false && isJumping == false)
            {
                velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
                position.Y += (velocity.Y * elapsed);
            }

        }

        // Testing collision
        public override bool CollisionTest(Collidable obj)
        {

            if (obj is Spell spell)
            {
                // Perform a test if the boss is alive
                // and if the spell is active
                if (spell.SpellCasted == true &&
                    GetHealth() > 0)
                    return (Vector2.DistanceSquared(GetPosition(), spell.GetPosition()) <= (100.0f * 100.0f));
            }
            else if (obj is Platform platform)
            {
                if (GetRectangle().Intersects(platform.GetRectangle()) == false)
                {
                    onGround = false;
                    return false;
                }
                else
                    return true;
            }

            return false;
        }

        public override void OnCollision(Collidable obj)
        {

            if (obj is Spell spell)
            {
                // Decrease health by the damage value of the spell
                health -= spell.GetDamage();
            }
            else if (obj is Platform platform)
            {
                // If the boss is not on ground
                // he is now on ground level and reset the Y
                // velocity
                if (onGround == false)
                {
                    onGround = true;
                    velocity.Y = 0;

                    // Black Fog doesn't have resolving depth code because
                    // of the ground pound attack

                }
                // The boss is now enabled to send a shockwave
                else if (castShockWave == true)
                {
                    left_wave.Pounded = true;
                    left_wave.InitialPositionSet = false;

                    right_wave.Pounded = true;
                    right_wave.InitialPositionSet = false;

                    castShockWave = false;
                    haltMovement = false;
                    initialPositionSet = false;

                    shockwaveSound.Play();

                    sprite.PlayAnimation(idleAnimation);

                }
            }

        }

        // Draw the boss and its weapons
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (direction == 1)
                flip = SpriteEffects.None;
            else if (direction == -1)
                flip = SpriteEffects.FlipHorizontally;

            sword.Draw(spriteBatch, direction);

            sprite.Draw(gameTime, spriteBatch, GetPosition(), 0.0f, 0.5f, flip);

            left_wave.Draw(spriteBatch, SpriteEffects.FlipHorizontally);
            right_wave.Draw(spriteBatch, SpriteEffects.None);

        }

        // GET FUNCTIONS

        public BlackFogSword GetSword()
        {
            return sword;
        }

        public BlackFogShockwave GetLeftWave()
        {
            return left_wave;
        }

        public BlackFogShockwave GetRightWave()
        {
            return right_wave;
        }

        // BLACK FOG MOVE SETS

        public bool ExecuteRampage(float elapsed)
        {
            // Execute the state when on ground
            if (onGround == true)
            {
                // Keep moving in a direction
                position.X += (direction * movementSpeed * elapsed);

                // Swap direction when the boss reaches either side
                // of the viewport window
                if (position.X > graphicsDevice.Viewport.Width - 50.0f)
                {
                    direction = -1;
                    sprite.PlayAnimation(idleAnimation);
                    return true;

                }
                else if (position.X < graphicsDevice.Viewport.X + 50.0f)
                {
                    direction = 1;
                    sprite.PlayAnimation(idleAnimation);
                    return true;

                }
                else
                {
                    sprite.PlayAnimation(runAnimation);
                    return false;
                }
            }
            else
            {
                sprite.PlayAnimation(fallAnimation);
                return false;
            }


        }

        public bool ExecuteSwordAttack(float elapsed)
        {
            // Retrieving the player's
            // position once
            if (initialPositionSet == false)
            {

                dest = playerPosition;

                if (dest.X <= GetPosition().X)
                    direction = -1;
                else
                    direction = 1;

                initialPositionSet = true;

            }

            // Move towards the player
            // at increased speed
            position.X += (movementSpeed * 1.5f * direction * elapsed);

            sprite.PlayAnimation(runAnimation);

            // If you reach close to the player, 
            // initiate the sword attack
            if (Math.Abs(GetPosition().X - dest.X) <= 160.0f)
            {
                sword.Swinging = true;

                sprite.PlayAnimation(idleAnimation);

                initialPositionSet = false;

                return true;
            }
            else
            {
                sprite.PlayAnimation(runAnimation);
                return false;
            }

        }

        public bool ExecuteSlamAttack(float elapsed)
        {
            // Obtain the player's position once
            if (initialPositionSet == false)
            {

                dest = playerPosition;

                if (dest.X <= GetPosition().X)
                    direction = -1;
                else
                    direction = 1;

                initialPositionSet = true;

            }
            // Move at increased speed
            else if (haltMovement == false)
            {
                position.X += (direction * movementSpeed * 1.5f * elapsed);
                sprite.PlayAnimation(runAnimation);

            }
            else if (onGround == false)
            {
                sprite.PlayAnimation(fallAnimation);
            }
            else
            {
                sprite.PlayAnimation(idleAnimation);
            }

            // If we're close to the player,
            // then start jumping
            if (Math.Abs(GetPosition().X - dest.X) <= 200.0f)
            {

                if (isJumping == false && onGround == true)
                {
                    isJumping = true;
                }

            }

            // Returning this bool so
            // that there will be delay
            // after sending a shockwave
            return castShockWave;

        }

        // Reset the wait upon exit
        public void ResetWait()
        {
            FSMWaitDelay = 3.0f;
        }

        // Each state has its own duration to
        // stay active
        public void SetWait(float value)
        {
            FSMWaitDelay = value;
        }

    }

}

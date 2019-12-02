using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Coursework_180016989
{
    //*******************************************
    // REFER BlackFog.cs FOR UNCOMMENTED SECTIONS
    //*******************************************

    class SwampCry : Boss
    {
        enum MoveSet
        {
            Rampage = 0,
            ClubAttack = 1,
            ThrowAttack = 2
        }

        private MoveSet currentMove;
        private int currentMoveNum;

        private SwampCryClub club;
        private SwampCrySpell spell;

        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation throwAnimation;
        private Animation fallAnimation;
        private Animation dieAnimation;

        private const float GravityAcceleration = 34000.0f;
        private const float MaxFallSpeed = 1200.0f;

        private bool initialPositionSet;

        private bool PausedInitially;
        private float pauseDelay = 0.35f;
        private float pauseTimer;

        public SwampCry(IServiceProvider services, GraphicsDevice graphics, int hp = 0)
        {

            graphicsDevice = graphics;

            if (content == null)
                content = new ContentManager(services) { RootDirectory = "Content" };

            fsm = new FSM(this);

            rand = new Random();

            SC_RampageState idle = new SC_RampageState();
            SC_ClubAttackState clubAttack = new SC_ClubAttackState();
            SC_ThrowAttack throwAttack = new SC_ThrowAttack();

            idle.AddTransition(new Transition(clubAttack, () => currentMove == MoveSet.ClubAttack));
            idle.AddTransition(new Transition(throwAttack, () => currentMove == MoveSet.ThrowAttack));

            clubAttack.AddTransition(new Transition(idle, () => currentMove == MoveSet.Rampage));
            clubAttack.AddTransition(new Transition(throwAttack, () => currentMove == MoveSet.ThrowAttack));

            throwAttack.AddTransition(new Transition(idle, () => currentMove == MoveSet.Rampage));
            throwAttack.AddTransition(new Transition(clubAttack, () => currentMove == MoveSet.ClubAttack));

            fsm.AddState(idle);
            fsm.AddState(clubAttack);
            fsm.AddState(throwAttack);

            fsm.Initialise("Rampage");

            idleAnimation = new Animation(content.Load<Texture2D>("Graphics/Boss/SwampCryIdle"), 0.1f, true, 18);
            runAnimation = new Animation(content.Load<Texture2D>("Graphics/Boss/SwampCryRunning"), 0.06f, true, 12);
            throwAnimation = new Animation(content.Load<Texture2D>("Graphics/Boss/SwampCryThrowing"), 0.07f, true, 12);
            fallAnimation = new Animation(content.Load<Texture2D>("Graphics/Boss/SwampCryFalling"), 0.3f, false, 6);
            dieAnimation = new Animation(content.Load<Texture2D>("Graphics/Boss/SwampCryDying"), 0.08f, false, 15);

            movementSpeed = 750.0f;

            startingPosition = new Vector2(graphics.Viewport.Width / 2, 0.0f);
            position = startingPosition;
            dest = Vector2.Zero;

            direction = 1;

            maxHealth = 500;

            if (hp == 0)
                health = maxHealth;
            else
                health = hp;

            FSMWaitDelay = 3.0f;
            FSMWaitTime = FSMWaitDelay;

            int width = (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.1);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            sprite = new Animator(OriginLocation.Center);
            sprite.PlayAnimation(fallAnimation);

            club = new SwampCryClub();
            spell = new SwampCrySpell();

            club.Initialise(content, position);
            spell.Initialise(content);

            currentMoveNum = 0;

        }

        public override void Reset()
        {
            position = startingPosition;

            currentMoveNum = 0;

            FSMWaitTime = FSMWaitDelay;

            club.Attacked = false;
            club.Swinging = false;
            spell.Thrown = false;


            SetHealth(GetMaxHealth());

            sprite.PlayAnimation(fallAnimation);

        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void Update(float elapsed, GraphicsDevice graphicsDevice, Vector2 p_Position)
        {

            playerPosition = p_Position;

            ApplyPhysics(elapsed);

            if (GetHealth() > 0)
            {
                fsm.Update(elapsed);

                FSMWaitTime += elapsed;

                if (FSMWaitTime >= FSMWaitDelay)
                {
                    currentMove = (MoveSet)currentMoveNum;

                    int previousMoveNum = currentMoveNum;
                    currentMoveNum = rand.Next(0, 3);

                    if (currentMoveNum == previousMoveNum)
                    {
                        currentMoveNum++;
                        currentMoveNum = currentMoveNum % 3;
                    }

                    FSMWaitTime = 0.0f;

                    // Resetting values for throw attack
                    pauseTimer = 0.0f;
                    pauseDelay = 0.35f;
                    PausedInitially = false;

                    initialPositionSet = false;

                }
            }
            else
                sprite.PlayAnimation(dieAnimation);


            position.X = MathHelper.Clamp(position.X, localBounds.Width / 3,
                graphicsDevice.Viewport.Width - localBounds.Width / 3);

            club.Update(elapsed, position);

            spell.Update(elapsed, position, direction, graphicsDevice);

        }

        public void ApplyPhysics(float elapsed)
        {
            if (onGround == false)
            {
                velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
                position.Y += (velocity.Y * elapsed);
            }

        }

        public override bool CollisionTest(Collidable obj)
        {
            if (obj is Spell spell)
            {
                if (spell.SpellCasted == true &&
                    GetHealth() > 0)
                    return Vector2.DistanceSquared(GetPosition(), spell.GetPosition()) <= (100.0f * 100.0f);
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
                health -= spell.GetDamage();
            }
            else if (obj is Platform platform)
            {
                if (onGround == false)
                {
                    onGround = true;
                    velocity.Y = 0;

                    // Calculating depth to resolve collisions
                    // Method used from Platformer Game from Microsoft XNA Community Game Platform
                    Vector2 depth = RectangleExtensions.GetIntersectionDepth(GetRectangle(), platform.GetRectangle());

                    // If there is some depth,
                    // then push the player appropriately in the Y axis
                    if (depth != Vector2.Zero)
                    {
                        position = new Vector2(position.X, position.Y + depth.Y);
                    }
                }

            }

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (direction == 1)
                flip = SpriteEffects.None;
            else if (direction == -1)
                flip = SpriteEffects.FlipHorizontally;

            club.Draw(spriteBatch, direction);

            spell.Draw(spriteBatch);

            sprite.Draw(gameTime, spriteBatch, GetPosition(), 0.0f, 0.5f, flip);

        }

        // GET FUNCTIONS

        public SwampCryClub GetClub()
        {
            return club;
        }

        public SwampCrySpell GetSpell()
        {
            return spell;
        }

        // SWAMP CRY MOVE SETS

        public bool ExecuteRampage(float elapsed)
        {
            if (onGround == true)
            {

                position.X += (direction * movementSpeed * elapsed);

                if (position.X > graphicsDevice.Viewport.Width - 100.0f)
                {
                    direction = -1;
                    sprite.PlayAnimation(idleAnimation);
                    return true;
                }
                else if (position.X < graphicsDevice.Viewport.X + 100.0f)
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
                return false;

        }

        public bool ExecuteClubAttack(float elapsed)
        {
            if (initialPositionSet == false)
            {

                dest = playerPosition;

                if (dest.X <= GetPosition().X)
                    direction = -1;
                else
                    direction = 1;

                initialPositionSet = true;

            }

            position.X += (movementSpeed * 1.5f * direction * elapsed);

            sprite.PlayAnimation(runAnimation);

            if (Math.Abs(GetPosition().X - dest.X) <= 160.0f)
            {
                club.Swinging = true;

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

        public bool ExecuteThrowAttack(float elapsed)
        {
            // Set the player position once
            if (initialPositionSet == false)
            {
                dest = playerPosition;

                if (position.X < dest.X)
                    direction = 1;
                else
                    direction = -1;

                initialPositionSet = true;

            }

            sprite.PlayAnimation(throwAnimation, 0, true);

            // The action is paused initially
            // for a short time to allow the
            // player to react
            pauseTimer += elapsed;

            if (PausedInitially == false)
            {
                // Throw the spell after the pause
                if (pauseTimer >= pauseDelay)
                {
                    sprite.Paused = false;
                    PausedInitially = true;
                    pauseTimer = 0.0f;

                    // Setting a new delay
                    // for the throw animation to play out
                    pauseDelay = 0.85f;

                    spell.Thrown = true;
                }
            }
            else if (pauseTimer >= pauseDelay)
            {
                // The boss waits for the spell
                // to either hit the player
                // or go out of bounds before
                // throwing another one
                if (spell.Thrown == false)
                {
                    spell.Thrown = true;
                    sprite.Paused = false;
                    pauseTimer = 0.0f;

                }
                else
                    sprite.Paused = true;

                // And pause or resume from the first frame
                sprite.FrameIndex = 0;
            }

            return true;

        }

        public void ResetWait()
        {
            FSMWaitDelay = 3.0f;
        }

        public void SetWait(float value)
        {
            FSMWaitDelay = value;
        }

    }
}

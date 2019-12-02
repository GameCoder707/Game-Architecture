using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Coursework_180016989
{
    //************************************************
    // REFER BlackFogSword.cs FOR UNCOMMENTED SECTIONS
    //************************************************

    class SwampCryClub : Collidable
    {

        private Texture2D texture2D;

        private Vector2 position;
        public override Vector2 GetPosition() { return position; }

        private float rotation;
        public float Rotation { get { return rotation; } }


        private Vector2 Origin
        {
            get { return new Vector2(texture2D.Width / 2.0f, texture2D.Height); }
        }

        private int damage;
        public int Damage { get { return damage; } }

        public bool Attacked;
        public bool Swinging;

        private Rectangle bounds;

        public override Rectangle GetRectangle() { return bounds; }

        public void Initialise(ContentManager content, Vector2 sc_position)
        {
            texture2D = content.Load<Texture2D>("Graphics/Boss/Weapons/SwampCryClub");

            damage = 15;

            bounds = new Rectangle(texture2D.Bounds.X, texture2D.Bounds.Y, texture2D.Width, texture2D.Height);

            position = sc_position;
            rotation = 0.0f;
            Attacked = false;
            Swinging = false;
        }

        public void Update(float elapsed, Vector2 sc_position)
        {

            position = sc_position;

            if (Swinging)
            {
                rotation += (8.0f * elapsed);

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
            if (obj is Player player)
                return Vector2.DistanceSquared(GetPosition(), player.GetPosition()) <= (175.0f * 175.0f);
            

            return false;
        }

        public override void OnCollision(Collidable obj)
        {
            if (obj is Player player)
            {
                if (Swinging == true &&
                    Attacked == false)
                    Attacked = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, int dir)
        {
            if(Swinging)
            {
                if (dir == 1)
                    spriteBatch.Draw(texture2D, GetPosition(), GetRectangle(), Color.White, rotation, Origin, 1.5f, SpriteEffects.None, 0.0f);
                else
                    spriteBatch.Draw(texture2D, GetPosition(), GetRectangle(), Color.White, -rotation, Origin, 1.5f, SpriteEffects.FlipHorizontally, 0.0f);

            }
        }

    }
}

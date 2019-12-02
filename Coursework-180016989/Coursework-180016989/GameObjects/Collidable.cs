using Microsoft.Xna.Framework;

namespace Coursework_180016989
{
    // Base class for any object that needs collision
    public class Collidable
    {
        // Rectangle based collision detection
        // OR
        // Distance-based based collision detection

        public virtual Rectangle GetRectangle() { return Rectangle.Empty; }

        public virtual Vector2 GetPosition() { return Vector2.Zero; }

        // Collision Detection with obj
        public virtual bool CollisionTest(Collidable obj) { return false; }

        // Collision Response with obj
        public virtual void OnCollision(Collidable obj) { }

    }
}

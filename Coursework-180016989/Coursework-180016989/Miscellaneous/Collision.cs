using System.Collections.Generic;

namespace Coursework_180016989
{
    public class CollisionComparer : IEqualityComparer<Collision>
    {
        // To avoid collision tests of an object
        // with itself
        public bool Equals(Collision a, Collision b)
        {
            if ((a== null) || (b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public int GetHashCode(Collision a)
        {
            return a.GetHashCode();
        }

    }

    // Collision component between objects A and B
    public class Collision
    {
        // The objects under collision
        public Collidable A;
        public Collidable B;

        public Collision(Collidable a, Collidable b) { A = a; B = b; }

        // To avoid repeated collision tests
        public bool Equals(Collision other)
        {
            if (other == null) return false;

            if(A.Equals(other.A) && B.Equals(other.B))
            {
                return true;
            }

            return false;
        }

        // Will be called if collision
        // test between A and B is true
        public void Resolve() { A.OnCollision(B); }

    }
}

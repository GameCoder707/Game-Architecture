using System.Collections.Generic;

namespace Coursework_180016989
{
    class CollisionManager
    {
        // The list of objects that are 
        // collidable in the game scene
        private List<Collidable> m_Collidables = new List<Collidable>();

        // Using a Hashset to avoid duplicates
        private HashSet<Collision> m_Collisions = new HashSet<Collision>(new CollisionComparer());

        // Add it to the list
        public void AddCollidable(Collidable c) { m_Collidables.Add(c); }

        // Insert the collidable at a certain index.
        // Useful when some collision events have to happen
        // before the latter events
        public void InsertCollidable(int index, Collidable c) { m_Collidables.Insert(index, c); }

        // Get the index if you want to insert a collidable
        // at that spot
        public int GetIndexOfCollidable(Collidable c) { return m_Collidables.IndexOf(c); }

        // When some objects are no longer used,
        // we can remove it for faster collision checks
        public void RemoveCollidable(Collidable c) { m_Collidables.Remove(c); }

        public void Update()
        {
            // Responsible for performing collision tests
            UpdateCollisions();

            // Responsible for performing collision responses
            ResolveCollisions();
        }

        private void UpdateCollisions()
        {
            // To start afresh
            if (m_Collisions.Count > 0)
                m_Collisions.Clear();

            for (int i = 0; i < m_Collidables.Count; i++)
            {
                for (int j = 0; j < m_Collidables.Count; j++)
                {
                    // Looping through one object with every other
                    Collidable collidable1 = m_Collidables[i];
                    Collidable collidable2 = m_Collidables[j];

                    // Making sure they're not same
                    if (!collidable1.Equals(collidable2))
                    {
                        // If the collision test is successful, then
                        // add a new collsion betweeen the two objects
                        if (collidable1.CollisionTest(collidable2))
                        {
                            m_Collisions.Add(new Collision(collidable1, collidable2));
                        }
                    }
                }
            }
        }

        // For each successful collision,
        // apply the responses
        private void ResolveCollisions()
        {
            foreach (Collision c in m_Collisions)
            {
                c.Resolve();
            }
        }
    }
}

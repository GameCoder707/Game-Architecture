using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework_180016989
{
    public class SaveInfo
    {
        // Player's Health and Magic Points
        public int PlayerHealth = 0;
        public int PlayerMagic = 0;

        // Total time played in defeating the boss
        public int MinutesPlayed = 0;
        public int SecondsPlayed = 0;

        // Spells casted and hit
        // for weapon accuracy calculation
        public float SpellsFired = 0;
        public float SpellsHit = 0;

        // The name of the boss
        // to load the appropriate boss
        // its health
        public string BossName;
        public int BossHealth = 0;

        // Making use of the singleton pattern
        private static SaveInfo mInstance = null;

        public static SaveInfo Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new SaveInfo();
                return mInstance;
            }

            set { mInstance = value; }
        }

    }

}

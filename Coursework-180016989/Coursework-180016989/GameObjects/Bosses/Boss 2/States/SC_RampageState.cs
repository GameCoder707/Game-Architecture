using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Coursework_180016989
{
    // SC = Swamp Cry (Boss 2)
    class SC_RampageState : State
    {

        private float WaitDelay = 0.2f;
        private float WaitTime = 0.0f;

        public SC_RampageState() { Name = "Rampage"; }

        public override void Enter(object owner)
        {
            SwampCry boss = owner as SwampCry;
            WaitTime = WaitDelay;

            // Default duration of the state
            boss.ResetWait();
        }

        public override void Exit(object owner)
        {
            SwampCry boss = owner as SwampCry;
            WaitTime = 0.0f;
            boss.ResetWait();
        }

        public override void Execute(object owner, float elapsed)
        {
            if (!(owner is SwampCry boss)) return;

            if (WaitTime >= WaitDelay)
            {
                // Boss is made to wait before moving again
                if (boss.ExecuteRampage(elapsed))
                {
                    WaitTime = 0.0f;
                }
            }
            else
            {
                WaitTime += elapsed;
            }

        }

    }
}

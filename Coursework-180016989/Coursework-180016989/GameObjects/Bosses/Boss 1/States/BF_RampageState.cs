using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Coursework_180016989
{
    class BF_RampageState : State
    {
        private float rampageWaitTime = 0.0f;
        private float rampageWaitDelay = 0.3f;

        public BF_RampageState() { Name = "Rampage"; }

        public override void Enter(object owner)
        {
            BlackFog boss = owner as BlackFog;
            rampageWaitTime = rampageWaitDelay;

            // Setting the default duration
            boss.ResetWait();
        }

        public override void Exit(object owner)
        {
            BlackFog boss = owner as BlackFog;
            rampageWaitTime = 0.0f;
            boss.ResetWait();
        }

        public override void Execute(object owner, float elapsed)
        {
            if (!(owner is BlackFog boss)) return;

            if (rampageWaitTime >= rampageWaitDelay)
            {
                // The boss waits before moving again
                if(boss.ExecuteRampage(elapsed))
                {
                    rampageWaitTime = 0.0f;
                }

            }
            else
            {
                rampageWaitTime += elapsed;
            }

        }

    }
}

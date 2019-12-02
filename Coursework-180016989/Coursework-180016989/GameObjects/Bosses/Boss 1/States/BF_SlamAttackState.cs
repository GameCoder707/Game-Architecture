using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Coursework_180016989
{
    class BF_SlamAttackState : State
    {

        private float slamWaitTime = 0.0f;
        private float slamWaitDelay = 0.75f;

        public BF_SlamAttackState() { Name = "SlamAttack"; }

        public override void Enter(object owner)
        {
            BlackFog boss = owner as BlackFog;
            slamWaitTime = slamWaitDelay;

            // Duration of the state being active
            boss.SetWait(2.0f);
        }

        public override void Exit(object owner)
        {
            BlackFog boss = owner as BlackFog;
            slamWaitTime = 0.0f;
            boss.ResetWait();
        }

        public override void Execute(object owner, float elapsed)
        {
            if (!(owner is BlackFog boss)) return;

            if (slamWaitTime >= slamWaitDelay)
            {
                // The boss waits before slamming again
                if (boss.ExecuteSlamAttack(elapsed))
                {
                    slamWaitTime = 0.0f;
                }
            }
            else
            {
                slamWaitTime += elapsed;
            }



        }

    }

}

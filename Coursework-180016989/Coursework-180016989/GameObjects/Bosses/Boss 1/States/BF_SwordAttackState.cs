using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Coursework_180016989
{
    class BF_SwordAttackState : State
    {
        // Time to wait between attacks
        private float attackWaitTime = 0.0f;
        private float attackWaitDelay = 1.0f;

        public BF_SwordAttackState() { Name = "SwordAttack"; }

        public override void Enter(object owner)
        {
            BlackFog boss = owner as BlackFog;
            attackWaitTime = attackWaitDelay;

            // Duration of the state being active
            boss.SetWait(1.5f);
        }

        public override void Exit(object owner)
        {
            BlackFog boss = owner as BlackFog;
            attackWaitTime = 0.0f;
            boss.ResetWait();
        }

        public override void Execute(object owner, float elapsed)
        {
            if (!(owner is BlackFog boss)) return;
            
            if(attackWaitTime >= attackWaitDelay)
            {
                // When sword is executed, the boss
                // is made to wait before striking again
                if (boss.ExecuteSwordAttack(elapsed))
                {
                    attackWaitTime = 0.0f;
                }
            }
            else
            {
                attackWaitTime += elapsed;
            }


        }

    }
}

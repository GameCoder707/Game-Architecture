using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Coursework_180016989
{
    // SC = Swamp Cry (Boss 2)
    class SC_ClubAttackState : State
    {

        private float attackWaitDelay = 0.75f;
        private float attackWaitTime = 0.0f;

        public SC_ClubAttackState() { Name = "ClubAttack"; }

        public override void Enter(object owner)
        {
            SwampCry boss = owner as SwampCry;
            attackWaitTime = attackWaitDelay;

            // Duration of the state
            boss.SetWait(2.0f);

        }

        public override void Exit(object owner)
        {
            SwampCry boss = owner as SwampCry;
            attackWaitTime = 0.0f;
            boss.ResetWait();
        }

        public override void Execute(object owner, float elapsed)
        {
            if (!(owner is SwampCry boss)) return;

            if(attackWaitTime >= attackWaitDelay)
            {
                // Boss is made to wait before attacking again
                if(boss.ExecuteClubAttack(elapsed))
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

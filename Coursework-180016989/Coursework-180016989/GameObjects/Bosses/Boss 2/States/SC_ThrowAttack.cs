using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Coursework_180016989
{
    // SC = Swamp Cry (Boss 2)
    class SC_ThrowAttack : State
    {

        public SC_ThrowAttack() { Name = "ThrowAttack"; }

        public override void Enter(object owner)
        {
            SwampCry boss = owner as SwampCry;

            // Duration of the state
            boss.SetWait(1.7f);

        }

        public override void Exit(object owner)
        {
            SwampCry boss = owner as SwampCry;
            boss.ResetWait();
        }

        public override void Execute(object owner, float elapsed)
        {
            if (!(owner is SwampCry boss)) return;

            boss.ExecuteThrowAttack(elapsed);

        }

    }
}

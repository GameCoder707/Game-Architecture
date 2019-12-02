using Microsoft.Xna.Framework;

namespace Coursework_180016989
{
    class VictoryState : State
    {

        public VictoryState() { Name = "Victory"; }

        public override void Enter(object owner) { }

        public override void Exit(object owner) { }

        public override void Execute(object owner, float elapsed)
        {
            if (!(owner is Game1 game)) return;

            game.VictoryUpdate();
        }

    }
}

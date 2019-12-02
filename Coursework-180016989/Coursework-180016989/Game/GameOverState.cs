using Microsoft.Xna.Framework;

namespace Coursework_180016989
{
    // When the player dies, this
    // state becomes active
    class GameOverState : State
    {
        public GameOverState() { Name = "GameOver"; }

        public override void Enter(object owner) { }

        public override void Exit(object owner) { }

        public override void Execute(object owner, float elapsed)
        {
            if (!(owner is Game1 game)) return;

            game.GameOverUpdate();

        }
    }
}

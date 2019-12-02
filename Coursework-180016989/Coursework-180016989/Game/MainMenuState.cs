using Microsoft.Xna.Framework;

namespace Coursework_180016989
{
    // The state that is active
    // on start up
    class MainMenuState : State
    {
        public MainMenuState() { Name = "MainMenu"; }

        public override void Enter(object owner) { }

        public override void Exit(object owner) { }

        public override void Execute(object owner, float elapsed)
        {
            if (!(owner is Game1 game)) return;

            game.MainMenuUpdate();

        }
    }
}

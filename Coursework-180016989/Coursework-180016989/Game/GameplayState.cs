﻿using Microsoft.Xna.Framework;

namespace Coursework_180016989
{
    // The state that is active
    // during the boss fight
    class GameplayState : State
    {

        public void GameplayUpdateState() { Name = "Gameplay"; }

        public override void Enter(object owner) { }

        public override void Exit(object owner) { }

        public override void Execute(object owner, float elapsed)
        {
            if (!(owner is Game1 game)) return;

            game.GameUpdate(elapsed);

        }
    }
}

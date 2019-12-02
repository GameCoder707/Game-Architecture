using System;
using Microsoft.Xna.Framework.Input;

namespace Coursework_180016989
{
    // A struct like class to store the keyboard states
    // and the key to invoke input actions
    public class KeyboardEventArgs : EventArgs
    {

        public KeyboardEventArgs(Keys key, KeyboardState currentKeyboardState, KeyboardState prevKeyboardState)
        {
            CurrentState = currentKeyboardState;
            PrevState = prevKeyboardState;
            Key = key;
        }

        public readonly KeyboardState CurrentState;
        public readonly KeyboardState PrevState;
        public readonly Keys Key;

    }

}

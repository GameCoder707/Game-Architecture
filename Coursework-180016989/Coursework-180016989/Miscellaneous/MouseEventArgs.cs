using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Coursework_180016989
{
    // Different buttons for the MouseButton
    public enum MouseButton
    {
        NONE = 0x00,
        LEFT = 0x01,
        RIGHT = 0x02,
        MIDDLE = 0x04,
        XBUTTON1 = 0x08,
        XBUTTON2 = 0x10,
    }

    // A struct like class to store the mouse states
    // and the button to invoke input actions
    public class MouseEventArgs : EventArgs
    {
        public MouseEventArgs(MouseButton button, MouseState currentState, MouseState prevState)
        {
            CurrentState = currentState;
            PrevState = prevState;
            Button = button;
        }

        public readonly MouseState CurrentState;
        public readonly MouseState PrevState;
        public readonly MouseButton Button;
    }
}

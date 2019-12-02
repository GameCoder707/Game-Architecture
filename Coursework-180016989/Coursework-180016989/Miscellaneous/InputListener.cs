using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Coursework_180016989
{
    class InputListener
    {
        // Current and previous keyboard states
        private KeyboardState PrevKeyboardState { get; set; }
        private KeyboardState CurrentKeyboardState { get; set; }

        // Current and previous mouse button states
        private MouseState PrevMouseState { get; set; }
        private MouseState CurrentMouseState { get; set; }

        // List of keys to check for
        public HashSet<Keys> KeyList;

        // List of buttons to check for
        public HashSet<MouseButton> ButtonList;

        // Keyboard event handlers
        // key is down
        public event EventHandler<KeyboardEventArgs> OnKeyDown = delegate { };

        // key was up and is now down
        public event EventHandler<KeyboardEventArgs> OnKeyPressed = delegate { };

        // key was down and is now up
        public event EventHandler<KeyboardEventArgs> OnKeyUp = delegate { };

        // Mouse event handlers
        // button is down
        public event EventHandler<MouseEventArgs> OnButtonDown = delegate { };

        // key was up and is now down
        public event EventHandler<MouseEventArgs> OnButtonPressed = delegate { };

        // key was down and is now up
        public event EventHandler<MouseEventArgs> OnButtonUp = delegate { };

        public InputListener()
        {
            CurrentKeyboardState = Keyboard.GetState();
            PrevKeyboardState = CurrentKeyboardState;

            CurrentMouseState = Mouse.GetState();
            PrevMouseState = CurrentMouseState;

            KeyList = new HashSet<Keys>();
            ButtonList = new HashSet<MouseButton>();

        }

        public void AddKey(Keys key)
        {
            KeyList.Add(key);
        }

        public void AddButton(MouseButton button)
        {
            ButtonList.Add(button);
        }

        public void Update()
        {
            // Retrieving the states of both
            // the keyboard and the mouse 
            PrevKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();

            PrevMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();

            FireKeyboardEvents();
            FireMouseEvents();

        }

        private void FireKeyboardEvents()
        {
            // Check through each key in the key list
            foreach (Keys key in KeyList)
            {
                // Is the key currently down?
                if (CurrentKeyboardState.IsKeyDown(key))
                {
                    // Fire the OnKeyDown event
                    OnKeyDown?.Invoke(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));
                }

                // Has the key been released? (Was down and is now up)
                if (PrevKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyUp(key))
                {
                    OnKeyUp?.Invoke(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));
                }

                // Has the key been pressed? (Was up and is now down)
                if (PrevKeyboardState.IsKeyUp(key) && CurrentKeyboardState.IsKeyDown(key))
                {
                    OnKeyPressed?.Invoke(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));
                }
            }

        }

        private void FireMouseEvents()
        {
            foreach (MouseButton button in ButtonList)
            {
                if(button == MouseButton.LEFT)
                {
                    // Button is held down
                    if (CurrentMouseState.LeftButton == ButtonState.Pressed)
                    {
                        OnButtonDown?.Invoke(this, new MouseEventArgs(button, CurrentMouseState, PrevMouseState));
                    }

                    // Button is pressed once
                    if (PrevMouseState.LeftButton == ButtonState.Released &&
                        CurrentMouseState.LeftButton == ButtonState.Pressed)
                    {
                        OnButtonPressed?.Invoke(this, new MouseEventArgs(button, CurrentMouseState, PrevMouseState));
                    }


                    // Button is just released once
                    if (PrevMouseState.LeftButton == ButtonState.Pressed &&
                        CurrentMouseState.LeftButton == ButtonState.Released)
                    {
                        OnButtonUp?.Invoke(this, new MouseEventArgs(button, CurrentMouseState, PrevMouseState));
                    }

                }
                else if(button == MouseButton.RIGHT)
                {
                    // Button is held down
                    if (CurrentMouseState.RightButton == ButtonState.Pressed)
                    {
                        OnButtonDown?.Invoke(this, new MouseEventArgs(button, CurrentMouseState, PrevMouseState));
                    }

                    // Button is pressed once
                    if (PrevMouseState.RightButton == ButtonState.Released &&
                        CurrentMouseState.RightButton == ButtonState.Pressed)
                    {
                        OnButtonPressed?.Invoke(this, new MouseEventArgs(button, CurrentMouseState, PrevMouseState));
                    }

                    // Button is just released
                    if (PrevMouseState.RightButton == ButtonState.Pressed &&
                        CurrentMouseState.RightButton == ButtonState.Released)
                    {
                        OnButtonUp?.Invoke(this, new MouseEventArgs(button, CurrentMouseState, PrevMouseState));
                    }

                }
                
            }
        }

    }
}

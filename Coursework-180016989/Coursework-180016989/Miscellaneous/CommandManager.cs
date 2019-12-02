using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Coursework_180016989
{
    // Delegate action for input actions
    // Vector2 is used for two-dimensional inputs such as joysticks
    public delegate void GameAction(eButtonState buttonState, Vector2 amount);

    class CommandManager
    {
        // Listener object to receive input state
        // from both keyboard and mouse
        private InputListener m_Input;

        // Dictionary of all the keyboard keys 
        // and mouse buttons used in the game
        // It holds the Key/MouseButton to invoke a GameAction
        private Dictionary<Keys, GameAction> m_KeyBindings = new Dictionary<Keys, GameAction>();
        private Dictionary<MouseButton, GameAction> m_MouseBindings = new Dictionary<MouseButton, GameAction>();

        public CommandManager()
        {
            m_Input = new InputListener();

            // Register events with the input listener
            m_Input.OnKeyDown += OnKeyDown;
            m_Input.OnKeyPressed += OnKeyPressed;
            m_Input.OnKeyUp += OnKeyUp;

            m_Input.OnButtonDown += OnMouseButtonDown;
            m_Input.OnButtonPressed += OnMouseButtonPressed;
            m_Input.OnButtonUp += OnMouseButtonUp;

        }

        public void Update()
        {
            m_Input.Update();
        }

        // *****************************************
        // ************* KEYBOARD ******************
        //******************************************

        public void AddKeyboardBindings(Keys key, GameAction action)
        {
            // Add key to listen for when polling
            m_Input.AddKey(key);

            // Add the binding to the command map
            m_KeyBindings.Add(key, action);

        }

        // Depending on the input state,
        // the game action is invoked by the key
        public void OnKeyDown(object sender, KeyboardEventArgs e)
        {
            m_KeyBindings[e.Key]?.Invoke(eButtonState.DOWN, new Vector2(1.0f));
        }

        public void OnKeyUp(object sender, KeyboardEventArgs e)
        {
            m_KeyBindings[e.Key]?.Invoke(eButtonState.UP, new Vector2(1.0f));
        }

        public void OnKeyPressed(object sender, KeyboardEventArgs e)
        {
            m_KeyBindings[e.Key]?.Invoke(eButtonState.PRESSED, new Vector2(1.0f));
        }

        // *****************************************
        // ************** MOUSE ********************
        // *****************************************

        public void AddMouseBindings(MouseButton button, GameAction action)
        {
            m_Input.AddButton(button);

            m_MouseBindings.Add(button, action);
        }

        public void OnMouseButtonDown(object sender, MouseEventArgs e)
        {
            m_MouseBindings[e.Button]?.Invoke(eButtonState.DOWN, new Vector2(e.CurrentState.X, e.CurrentState.Y));
        }

        public void OnMouseButtonUp(object sender, MouseEventArgs e)
        {
            m_MouseBindings[e.Button]?.Invoke(eButtonState.UP, new Vector2(e.CurrentState.X, e.CurrentState.Y));
        }

        public void OnMouseButtonPressed(object sender, MouseEventArgs e)
        {
            m_MouseBindings[e.Button]?.Invoke(eButtonState.PRESSED, new Vector2(e.CurrentState.X, e.CurrentState.Y));
        }

    }
}

using System.Collections.Generic;

namespace Coursework_180016989
{
    public abstract class State
    {
        // Enter, Exit and Execute functions of a state
        // Inherited states will have various actions in
        // each level
        public abstract void Enter(object owner);
        public abstract void Exit(object owner);
        public abstract void Execute(object owner, float elapsed);

        // Name of the state used in initialising the FSM
        public string Name { get; set; }

        // List of transitions associated with this state
        private List<Transition> m_Transitions = new List<Transition>();
        public List<Transition> Transitions { get { return m_Transitions; } }

        public void AddTransition(Transition transition) { m_Transitions.Add(transition); }


    }

}

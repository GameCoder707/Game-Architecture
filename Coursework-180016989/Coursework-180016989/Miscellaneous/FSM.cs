using System.Collections.Generic;

namespace Coursework_180016989
{
    // FSM = Finite State Machine
    class FSM
    {
        // Owner of the FSM
        private object m_Owner;

        // The list of states the owner
        // will switch between
        private List<State> m_States;

        private State m_CurrentState;

        public FSM() : this(null) { }

        public FSM(object owner)
        {
            m_Owner = owner;
            m_States = new List<State>();
            m_CurrentState = null;
        }

        // Initialising the current by its name
        public void Initialise(string stateName)
        {
            m_CurrentState = m_States.Find(state => state.Name.Equals(stateName));

            if (m_CurrentState != null)
            {
                m_CurrentState.Enter(m_Owner);
            }

        }

        public void AddState(State state) { m_States.Add(state); }

        // Update the current states by checking
        // its transition conditions
        public void Update(float elapsed)
        {
            if (m_CurrentState == null) return;

            for (int i = 0; i < m_CurrentState.Transitions.Count; i++)
            {
                // If the condition is satisfied,
                // then exit the current, set the new
                // current state and enter it
                if (m_CurrentState.Transitions[i].Condition())
                {
                    m_CurrentState.Exit(m_Owner);
                    m_CurrentState = m_CurrentState.Transitions[i].NextState;
                    m_CurrentState.Enter(m_Owner);
                    break;
                }
            }

            // Execute the current state
            m_CurrentState.Execute(m_Owner, elapsed);

        }
    }
}

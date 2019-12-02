using System;

namespace Coursework_180016989
{
    // A class responsible for switching
    // to the next state if a condition
    // is satisfied
    public class Transition
    {
        public readonly State NextState;
        public readonly Func<bool> Condition;

        public Transition(State nextState, Func<bool> condition)
        {
            NextState = nextState;
            Condition = condition;
        }
    }
}
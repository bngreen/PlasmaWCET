//    This file is part of PlasmaWCET.
//
//    PlasmaWCET is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    PlasmaWCET is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with PlasmaWCET.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WCET
{
    public class StateTransition: AStar.IStateTransition
    {

        public IEnumerable<Tuple<AStar.IState, AStar.IAction, double>> Expand(AStar.IState state)
        {
            var st = state as State;
            var expandedStates = new List<Tuple<AStar.IState, AStar.IAction, double>>();
            if (st.Node.Left != null)
            {
                var cState = new State();
                cState.Cache = st.Cache.Clone();
                cState.Node = st.Node.Left;
                expandedStates.Add(new Tuple<AStar.IState, AStar.IAction, double>(cState, new Action("Left"), ComputeCost(cState)));
            }
            if (st.Node.Right != null)
            {
                var cState = new State();
                cState.Cache = st.Cache.Clone();
                cState.Node = st.Node.Right;
                expandedStates.Add(new Tuple<AStar.IState, AStar.IAction, double>(cState, new Action("Right"), ComputeCost(cState)));
            }
            return expandedStates;
        }

        public uint ComputeCost(State cState)
        {
            var cost = cState.Node.StaticCost;
            foreach (var x in cState.Node.Content)
                cost += cState.Cache.Read(x.Address);
            return cost;
        }

        public bool IsGoal(AStar.IState state, AStar.IState goal)
        {
            return state.CompareTo(goal) == 0;
        }
    }
}

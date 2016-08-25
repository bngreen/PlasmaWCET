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
    using InstNode = Node<IList<MIPSI.IInstruction>>;
    public class GraphHeuristic
    {
        public InstructionRuntime InstructionRuntime { get; set; }
        public uint CacheMissOverhead { get; set; }
        public GraphHeuristic()
        {
            InstructionRuntime = new InstructionRuntime();
            CacheMissOverhead = 5;
        }
        public int ex = 0;
        public void ComputeHeuristic(InstNode node)
        {
            if (node.StaticCost != 0)
            {
                ex++;
                return;
            }
            node.StaticCost = 0;
            foreach (var x in node.Content)
                node.StaticCost += InstructionRuntime.GetInstructionRuntime(x);
            var leftH = 0u;
            var rightH = 0u;
            if (node.Left != null)
            {
                ComputeHeuristic(node.Left);
                leftH = node.Left.Heuristic + (uint)(node.Left.Content.Count * CacheMissOverhead);
            }
            if (node.Right != null)
            {
                ComputeHeuristic(node.Right);
                rightH = node.Right.Heuristic + (uint)(node.Right.Content.Count * CacheMissOverhead);
            }
            node.Heuristic = node.StaticCost + Math.Max(leftH, rightH);
        }
    }
}

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
using MIPSI;

namespace WCET
{
    public class GraphBuilder
    {
        public Node<IList<MIPSI.IInstruction>> Build(SubCode code)
        {
            var Root = Builder(code).Item1;
            return Root;
        }

        private Tuple<Node<IList<IInstruction>>, IList<Node<IList<IInstruction>>>> Builder(SubCode code)
        {
            var returnNodes = new List<Node<IList<MIPSI.IInstruction>>>();
            var partitions = new List<uint>();
            foreach (var x in code.Branches)
            {
                partitions.Add(x.Address);
                partitions.Add(x.Pointer - 1);
            }
            foreach (var x in code.FunctionCalls)
            {
                partitions.Add(x.CallerAddress);
                //partitions.Add(x.FunctionAddress);
            }
            foreach (var x in code.Jumps)
            {
                partitions.Add(x.From);
                partitions.Add(x.To);
            }
            var insts = code.VisitedInstructions.OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);
            var nodes = new Dictionary<uint, Node<IList<IInstruction>>>();
            partitions = partitions.OrderBy(x => x).ToList();
            var currentNode = CreateNode();
            var Root = currentNode;
            var currentPartition = partitions.Count > 0 ? partitions[0] : uint.MaxValue;
            var partitionIndex = 1;
            foreach (var x in insts)
            {
                currentNode.Content.Add(x.Value);
                nodes.Add(x.Key, currentNode);
                if (x.Key == currentPartition)
                {
                    var newnode = CreateNode();
                    currentNode.Left = newnode;
                    currentNode = newnode;
                    currentPartition = partitionIndex < partitions.Count ? partitions[partitionIndex] : uint.MaxValue;
                    partitionIndex++;
                }

            }
            foreach (var x in code.Branches)
            {
                var from = nodes[x.Address];
                var to = nodes[x.Pointer];
                //var notTaken = nodes[x.Address + 1];
                //from.Left = notTaken;
                from.Right = to;
            }
            foreach (var x in code.Jumps)
            {
                var from = nodes[x.From];
                var to = nodes[x.To];
                from.Left = to;
            }
            foreach (var x in code.FunctionCalls)
            {
                var from = nodes[x.CallerAddress];
                var returnPoint = nodes[x.CallerAddress + 1];
                var func = Builder(x.FunctionBody);
                from.Left = func.Item1;
                foreach (var r in func.Item2)
                    r.Left = returnPoint;
            }
            foreach (var x in code.Returns)
                returnNodes.Add(nodes[x]);
            return new Tuple<Node<IList<IInstruction>>, IList<Node<IList<IInstruction>>>>(Root, returnNodes);
        }
        private Node<IList<IInstruction>> CreateNode()
        {
            return new Node<IList<IInstruction>>() { Content = new List<IInstruction>() };
        }


    }
}

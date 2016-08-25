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
using System.IO;

namespace WCET
{
    using InstNode = Node<IList<IInstruction>>;
    public class GraphLinearizer
    {

        public bool SearchRec(InstNode current, InstNode target, HashSet<InstNode> visitedNodes, ISet<InstNode> paths)
        {
            bool found = false;
            if (current == target)
                found = true;
            if (visitedNodes.Contains(current))
                return found;
            visitedNodes.Add(current);
            paths.Add(current);
            var pth = new HashSet<InstNode>();
            if (current.Left != null && SearchRec(current.Left, target, visitedNodes, pth))
            {
                found = true;
                foreach (var x in pth)
                    paths.Add(x);
            }
            pth = new HashSet<InstNode>();
            if (current.Right != null && SearchRec(current.Right, target, visitedNodes, pth))
            {
                found = true;
                foreach (var x in pth)
                    paths.Add(x);
            }
            return found;
        }


        private ISet<Tuple<InstNode, InstNode>> FindCycles2(InstNode root)
        {
            var visitedNodes = new HashSet<InstNode>();
            var cycles = new HashSet<Tuple<InstNode, InstNode>>();
            GraphTraversal.PerformTraversal<InstNode>(root, (node, fringeAdder) =>
            {
                if (visitedNodes.Contains(node))
                    return;
                visitedNodes.Add(node);
                if (node.Left != null)
                {
                    if (visitedNodes.Contains(node.Left))
                        cycles.Add(new Tuple<InstNode, InstNode>(node, node.Left));
                }
                if (node.Right != null)
                {
                    if (visitedNodes.Contains(node.Right))
                        cycles.Add(new Tuple<InstNode, InstNode>(node, node.Right));
                }
                if (node.Left != null)
                {
                    if (!visitedNodes.Contains(node.Left))
                        fringeAdder(node.Left);
                }
                if (node.Right != null)
                {
                    if (!visitedNodes.Contains(node.Right))
                        fringeAdder(node.Right);
                }
            });
            return cycles;
        }

        private void FindCyclesRec(InstNode node, HashSet<InstNode> visitedNodes, ISet<Tuple<InstNode, InstNode>> cycles)
        {
            if(visitedNodes.Contains(node))
                return;
            visitedNodes.Add(node);
            if (node.Left != null)
            {
                if (visitedNodes.Contains(node.Left))
                    cycles.Add(new Tuple<InstNode, InstNode>(node, node.Left));
            }
            if (node.Right != null)
            {
                if (visitedNodes.Contains(node.Right))
                    cycles.Add(new Tuple<InstNode, InstNode>(node, node.Right));
            }
            if (node.Left != null)
            {
                if (!visitedNodes.Contains(node.Left))
                    FindCyclesRec(node.Left, visitedNodes, cycles);
            }
            if (node.Right != null)
            {
                if (!visitedNodes.Contains(node.Right))
                    FindCyclesRec(node.Right, visitedNodes, cycles);
            }
        }

        private void LinearizeRec(InstNode node, ISet<InstNode> incNodes)
        {
            if (node.Left != null)
            {
                if (incNodes.Contains(node.Left))
                    LinearizeRec(node.Left, incNodes);
                else
                    node.Left = null;
            }
            if (node.Right != null)
            {
                if (incNodes.Contains(node.Right))
                    LinearizeRec(node.Right, incNodes);
                else
                    node.Right = null;
            }
        }

        public void Linearize(InstNode root)
        {
            var ind = 0;
            var gv = new GraphViz();
            while (true)
            {
                var cycles = FindCycles(root);
                if (cycles.Count == 0)
                    return;
                
                var cyclesN = new List<Tuple<Tuple<InstNode, InstNode>,
                                        ISet<InstNode>>>();
                foreach (var x in cycles)
                {
                    var set = new HashSet<InstNode>();
                    SearchRec(x.Item2, x.Item1, new HashSet<InstNode>(), set);
                    cyclesN.Add(new Tuple<Tuple<InstNode, InstNode>, ISet<InstNode>>(x, set));
                }
                var availableCycles = new List<Tuple<Tuple<InstNode, InstNode>,
                                        ISet<InstNode>>>();
                foreach (var x in cyclesN)
                {
                    var valid = true;
                    foreach (var y in cyclesN)
                        if (x != y)
                            foreach (var e in x.Item2)
                                if (y.Item2.Contains(e))
                                    valid = false;
                    if (valid)
                        availableCycles.Add(x);

                }
                if (availableCycles.Count == 0)
                    throw new InvalidOperationException("Error During Linearization");

                foreach (var y in availableCycles)
                {
                    var set = y.Item2;
                    var x = y.Item1;
                    var n1 = x.Item2;
                    var n2 = x.Item1.Clone();
                    var n3 = x.Item1;
                    if (n2.Left == n1)
                    {
                        n2.Left = null;
                        n3.Left = null;
                    }
                    if (n2.Right == n1)
                    {
                        n2.Right = null;
                        n3.Right = null;
                    }
                    var n3l = n3.Left;
                    var n3r = n3.Right;
                    n3.Left = null;
                    n3.Right = null;
                    var lst = n1.CloneAndReturnSN(n3);
                    lst.Item2.Left = n3l;
                    lst.Item2.Right = n3r;
                    n3.Left = n3l;
                    n3.Right = n3l;
                    LinearizeRec(n1, set);

                    var ccs = new CycleCountSelector(gv.CreateGraph(n1));
                    if (ccs.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        Console.WriteLine("Operation Cancelled");
                        Environment.Exit(0);
                    }


                    var N = ccs.NumberOfRepetitions;
                    //var N = 3;

                    if (N == 1)
                    {
                        n1.Left = lst.Item1.Left;
                        n2.Right = lst.Item1.Right;
                        continue;
                    }


                    var last = new Tuple<InstNode, InstNode>(n1, n3);
                    for (int j = 2; j < N; j++)
                    {
                        var clone = last.Item1.CloneAndReturnSN(last.Item2);
                        //var clone = n1.CloneAndReturnSN(n3);
                        last.Item2.Left = clone.Item1;
                        last = clone;
                    }
                    //last.Item2.Left = n2.Left;
                    //last.Item2.Right = n2.Right;
                    last.Item2.Left = lst.Item1;
                }
                if (cycles.Count == availableCycles.Count)
                    return;
            }
        }

        public ISet<Tuple<InstNode, InstNode>> FindCycles(InstNode root)
        {
            var cycles = new HashSet<Tuple<InstNode,InstNode>>();
            FindCyclesRec(root, new HashSet<InstNode>(), cycles);
            return cycles;
        }

        public InstNode FindNodeByInstAddress(InstNode node, uint address)
        {
            foreach (var x in node.Content)
                if (x.Address == address)
                    return node;
            if (node.Left != null)
            {
                var nd = FindNodeByInstAddress(node.Left, address);
                if (nd != null)
                    return nd;
            }
            if (node.Right != null)
            {
                var nd = FindNodeByInstAddress(node.Right, address);
                if (nd != null)
                    return nd;
            }

            return null;
        }

    }
}

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
    public class CachelessWCET
    {
        private Dictionary<InstNode, uint> visitedNodes = new Dictionary<InstNode, uint>();

        public uint CalculateWCET2(InstNode root)
        {
            var visitedNodes = new Dictionary<InstNode, Func<uint>>();
            var cache = new Dictionary<InstNode, uint>();
            GraphTraversal.PerformTraversal<InstNode>(root, (node, fringeAdder) =>
            {
                if (visitedNodes.ContainsKey(node))
                    return;
                var cycles = (uint)(node.Content.Count * 0);
                cycles += node.StaticCost;
                Func<uint> lc = () => 0u;
                Func<uint> rc = () => 0u;
                if (node.Left != null)
                {
                    fringeAdder(node.Left);
                    lc = () =>
                    {
                        uint c;
                        if (cache.TryGetValue(node.Left, out c))
                            return c;
                        c = visitedNodes[node.Left]();
                        cache.Add(node.Left, c);
                        return c;
                    };
                }
                if (node.Right != null)
                {
                    fringeAdder(node.Right);
                    rc = () =>
                    {
                        uint c;
                        if (cache.TryGetValue(node.Right, out c))
                            return c;
                        c = visitedNodes[node.Right]();
                        cache.Add(node.Right, c);
                        return c;
                    };
                }
                visitedNodes.Add(node, () => cycles + Math.Max(lc(), rc()));
            }, true);
            return visitedNodes[root]();
        }

        public uint CalculateWCET(InstNode nd)
        {
            uint pc;
            if (visitedNodes.TryGetValue(nd, out pc))
                return pc;
            var cycles = (uint)(nd.Content.Count * 0);
            cycles += nd.StaticCost;
            var lc = 0u;
            var rc = 0u;
            if (nd.Left != null)
                lc = CalculateWCET(nd.Left);
            if (nd.Right != null)
                rc = CalculateWCET(nd.Right);
            cycles += Math.Max(lc, rc);
            visitedNodes.Add(nd, cycles);
            return cycles;
        }
    }
}

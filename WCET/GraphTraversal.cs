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
    public class GraphTraversal
    {
        public static void PerformTraversal<T>(T root, Action<T, Action<T>> action, bool depthFirst=false)
        {
            var fringe = new LinkedList<T>();
            fringe.AddFirst(root);
            Action<T> adder;
            if (depthFirst)
                adder = n => fringe.AddFirst(n);
            else
                adder = n => fringe.AddLast(n);
            while (fringe.Count > 0)
            {
                var node = fringe.First.Value;
                fringe.RemoveFirst();
                action(node, adder);
            }
        }

    }
}

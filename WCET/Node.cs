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
    public class Node<T>
    {
        public T Content { get; set; }
        public Node<T> Left { get; set; }
        public Node<T> Right { get; set; }
        public Node<T> Clone()
        {
            return new Node<T>() { Content = this.Content, Left = this.Left, Right = this.Right };
        }

        public Tuple<Node<T>, Node<T>> CloneAndReturnSN(Node<T> returnNode)
        {
            return CloneAndReturnSN(returnNode, new Dictionary<Node<T>, Tuple<Node<T>, Node<T>>>());
        }

        public Tuple<Node<T>, Node<T>> CloneAndReturnSN(Node<T> returnNode, IDictionary<Node<T>, Tuple<Node<T>, Node<T>>> oldToNewMapping)
        {
            Tuple<Node<T>, Node<T>> ndm;
            if (oldToNewMapping.TryGetValue(this, out ndm))
                return ndm;
            var r = (Right == null) ? new Tuple<Node<T>, Node<T>>(null, null) : Right.CloneAndReturnSN(returnNode, oldToNewMapping);
            var l = (Left == null) ? new Tuple<Node<T>, Node<T>>(null, null) : Left.CloneAndReturnSN(returnNode, oldToNewMapping);
            var newN = new Node<T>() { Content = this.Content, Left = l.Item1, Right = r.Item1 };
            var newSN = (this == returnNode) ? newN : (r.Item2 != null ? r.Item2 : l.Item2);
            var ret = new Tuple<Node<T>, Node<T>>(newN, newSN);
            oldToNewMapping.Add(this, ret);
            return ret;
        }

        public uint StaticCost { get; set; }
        public uint Heuristic { get; set; }

    }
}

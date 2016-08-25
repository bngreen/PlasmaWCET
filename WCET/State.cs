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
using AStar;
namespace WCET
{
    using InstNode = Node<IList<IInstruction>>;
    public class State : AStar.IState
    {
        public InstNode Node { get; set; }
        public ICache Cache { get; set; }
        public int CompareTo(AStar.IState other)
        {
            var nd = other as State;
            if (nd == null)
                return -1;
            if (nd.Node == this.Node)
                return 0;
            return -1;
        }
        public override bool Equals(object obj)
        {
            var os = obj as State;
            if (os == null)
                return false;
            if (Node == os.Node)
                return true;
            return false;
        }
        public override int GetHashCode()
        {
            return Node.Content.Count;
        }
    }
}

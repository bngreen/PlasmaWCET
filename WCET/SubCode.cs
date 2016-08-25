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
    public class SubCode
    {
        public IDictionary<uint, IInstruction> VisitedInstructions { get; set; }
        public IList<Branch> Branches { get; set; }
        public IList<uint> Returns { get; set; }
        public IList<FunctionCall> FunctionCalls { get; set; }
        public IList<Jump> Jumps { get; set; }
        public SubCode()
        {
            VisitedInstructions = new Dictionary<uint, IInstruction>();
            Branches = new List<Branch>();
            Returns = new List<uint>();
            FunctionCalls = new List<FunctionCall>();
            Jumps = new List<Jump>();
        }
        public void Clone()
        {
            var s = new SubCode();
            foreach (var x in VisitedInstructions)
                s.VisitedInstructions.Add(x);
            foreach (var x in Branches)
                s.Branches.Add(x);
            foreach (var x in Returns)
                s.Returns.Add(x);
        }
        public bool AddVisitedInstruction(uint index, IInstruction instruction)
        {
            if (VisitedInstructions.ContainsKey(index))
                return false;
            VisitedInstructions.Add(index, instruction);
            return true;
        }
    }
}

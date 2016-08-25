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

namespace MIPSI
{
    public class ITypeInstruction : IInstruction
    {
        public uint Address { get; set; }
        public Opcode Opcode { get; set; }
        public uint Rs { get; set; }
        public uint Rt { get; set; }
        public uint Immediate { get; set; }
        public static ITypeInstruction FromInteger(uint v)
        {
            var it = new ITypeInstruction()
            {
                Opcode = (MIPSI.Opcode)(v >> 26),
                Rs = (v >> 21) & 0x1f,
                Rt = (v >> 16) & 0x1f,
                Immediate = ((ushort)((v) & 0xFFFF))
            };
            return it;
        }
        public uint ToInteger()
        {
            var v = 0u;
            v |= ((uint)Opcode) << 26;
            v |= (Rs & 0x1f) << 21;
            v |= (Rt & 0x1f) << 16;
            v |= (Immediate & 0xFFFF);
            return v;
        }

        public Action<ITypeInstruction, IStateAccessor> Executor { get; set; }

        public void Execute(IStateAccessor state)
        {
            Executor(this, state);
        }
    }
}

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
    public class CoProcessorInstruction : IInstruction
    {
        public uint Address { get; set; }
        public Opcode Opcode { get; set; }
        public CoProcessorFormat Format { get; set; }
        public uint Rt { get; set; }
        public uint Rd { get; set; }
        public static CoProcessorInstruction FromInteger(uint v)
        {
            var cp = new CoProcessorInstruction()
            {
                Opcode = (MIPSI.Opcode)(v >> 26),
                Format = (CoProcessorFormat)((v >> 21) & 0x1f),
                Rt = (v >> 16) & 0x1f,
                Rd = (v >> 11) & 0x1f
            };
            return cp;

        }
        public uint ToInteger()
        {
            throw new NotImplementedException();
        }

        public Action<CoProcessorInstruction, IStateAccessor> Executor { get; set; }

        public void Execute(IStateAccessor state)
        {
            Executor(this, state);
        }
    }
}

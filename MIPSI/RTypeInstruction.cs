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
    public class RTypeInstruction : IInstruction
    {
        public uint Address { get; set; }
        public uint Rs { get; set; }
        public uint Rt { get; set; }
        public uint Rd { get; set; }
        public uint Sa { get; set; }
        public RTypeFunction Function { get; set; }
        public static RTypeInstruction FromInteger(uint v)
        {
            var rtype = new RTypeInstruction()
            {
                Function = (RTypeFunction)(v & 0x3f),
                Sa = ((v >> 6) & 0x1f),
                Rd = ((v >> 11) & 0x1f),
                Rt = ((v >> 16) & 0x1f),
                Rs = ((v >> 21) & 0x1f),
            };
            return rtype;
        }
        public uint ToInteger()
        {
            uint v = 0;
            v |= (((uint)Function) & 0x3f);
            v |= ((Sa & 0x1f)) << 6;
            v |= ((Rd & 0x1f)) << 11;
            v |= ((Rt & 0x1f)) << 16;
            v |= ((Rs & 0x1f)) << 21;
            return v;
        }

        public Action<RTypeInstruction, IStateAccessor> Executor { get; set; }

        public void Execute(IStateAccessor state)
        {
            Executor(this, state);
        }
    }
}

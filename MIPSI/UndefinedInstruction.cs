﻿//    This file is part of PlasmaWCET.
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
    public class UndefinedInstruction : IInstruction
    {
        public uint Data { get; set; }

        public static UndefinedInstruction FromInteger(uint data)
        {
            return new UndefinedInstruction() { Data = data };
        }
        public uint ToInteger()
        {
            return Data;
        }


        public void Execute(IStateAccessor state)
        {
            
        }


        public uint Address { get; set; }
    }
}
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
    public enum Opcode : uint
    {
        RType = 0,
        AddImmediate = 0x8,
        UnsignedAddImmediate = 9,
        BitwiseAndImmediate = 0xc,
        BranchOnEqual = 4,
        Branch = 1,
        BranchOnGreaterThanZero = 7,
        BranchOnLessThanOrEqualToZero = 6,
        BranchOnNotEqual = 5,
        LoadByte = 0x20,
        UnsignedLoadByte = 0x24,
        LoadHalfword = 0x21,
        UnsignedLoadHalfword = 0x25,
        LoadUpperImmediate = 0xf,
        LoadWord = 0x23,
        BitwiseOrImmediate = 0xd,
        StoreByte = 0x28,
        SetOnLessThanImmediate = 0xa,
        UnsignedSetOnLessThanImmediate = 0xb,
        StoreHalfword = 0x29,
        StoreWord = 0x2b,
        BitwiseExclusiveOrImmediate = 0xe,
        Jump = 2,
        JumpAndLink = 3,
        CoProcessor = 0x10,
    }
}

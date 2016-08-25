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
    public enum RTypeFunction
    {
        Add = 32,
        UnsignedAdd = 33,
        BitwiseAnd = 36,
        Break = 0xd,
        Divide = 26,
        UnsignedDivide = 27,
        JumpAndLinkRegister = 9,
        JumpRegister = 8,
        MoveFromHi = 16,
        MoveFromLo = 18,
        MoveToHi = 17,
        MoveToLo = 19,
        Multiply = 0x18,
        UnsignedMultiply = 25,
        BitwiseNor = 39,
        BitwiseOr = 37,
        ShiftLeftLogical = 0,
        ShiftLeftLogicalVariable = 4,
        SetOnLessThan = 0x2a,
        UnsignedSetOnLessThan = 0x2b,
        ShiftRightArithmetic = 3,
        ShiftRightArithmeticVariable = 7,
        ShiftRightLogical = 2,
        ShiftRightLogicalVariable = 6,
        Subtract = 0x22,
        UnsignedSubtract = 0x23,
        Syscall = 0x0c,
        BitwiseExclusiveOr = 0x26,

    }
}

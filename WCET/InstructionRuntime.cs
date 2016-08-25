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
    public class InstructionRuntime
    {
        private Dictionary<Opcode, uint> ITypeInstructionsRuntimeDict { get; set; }
        private Dictionary<RTypeFunction, uint> RTypeInstructionsRuntimeDict { get; set; }
        private Dictionary<JumpType, uint> JTypeInstructionsRuntimeDict { get; set; }
        private void FillITypeInstructionsRuntime(uint memoryReadCycles, uint memoryWriteCycles)
        {
            ITypeInstructionsRuntimeDict = new Dictionary<Opcode, uint>
            {
                { Opcode.AddImmediate, 1
                },
                { Opcode.UnsignedAddImmediate, 1
                },
                { Opcode.BitwiseAndImmediate, 1
                },
                { Opcode.BranchOnEqual, 1
                },
                { Opcode.Branch, 1
                },
                { Opcode.BranchOnGreaterThanZero, 1
                },
                { Opcode.BranchOnLessThanOrEqualToZero, 1
                },
                { Opcode.BranchOnNotEqual, 1
                },
                { Opcode.LoadByte, 1 + memoryReadCycles
                },
                { Opcode.UnsignedLoadByte, 1 + memoryReadCycles
                },
                { Opcode.LoadHalfword, 1 + memoryReadCycles
                },
                { Opcode.UnsignedLoadHalfword, 1 + memoryReadCycles
                },
                { Opcode.LoadUpperImmediate, 1
                },
                { Opcode.LoadWord, 1 + memoryReadCycles
                },
                { Opcode.BitwiseOrImmediate, 1
                },
                { Opcode.StoreByte, 1 + memoryWriteCycles
                },
                { Opcode.SetOnLessThanImmediate, 1
                },
                { Opcode.UnsignedSetOnLessThanImmediate, 1
                },
                { Opcode.StoreHalfword, 1 + memoryWriteCycles
                },
                { Opcode.StoreWord, 1 + memoryWriteCycles
                },
                { Opcode.BitwiseExclusiveOrImmediate, 1
                },
            };
        }
        private void FillRTypeInstructionsRuntime()
        {
            RTypeInstructionsRuntimeDict = new Dictionary<RTypeFunction, uint>
            {
                { RTypeFunction.Add, 1
                },
                { RTypeFunction.UnsignedAdd, 1
                },
                { RTypeFunction.BitwiseAnd, 1
                },
                /*{ RTypeFunction.Break, 1
                },*/
                { RTypeFunction.Divide, 32
                },
                { RTypeFunction.UnsignedDivide, 32
                },
                { RTypeFunction.JumpAndLinkRegister, 1
                },
                { RTypeFunction.JumpRegister, 1
                },
                { RTypeFunction.MoveFromHi, 32
                },
                { RTypeFunction.MoveFromLo, 32
                },
                { RTypeFunction.MoveToHi, 32
                },
                { RTypeFunction.MoveToLo, 32
                },
                { RTypeFunction.Multiply, 32
                },
                { RTypeFunction.UnsignedMultiply, 32
                },
                { RTypeFunction.BitwiseNor, 1
                },
                { RTypeFunction.BitwiseOr, 1
                },
                { RTypeFunction.ShiftLeftLogical, 1
                },
                { RTypeFunction.ShiftLeftLogicalVariable, 1
                },
                { RTypeFunction.SetOnLessThan, 1
                },
                { RTypeFunction.UnsignedSetOnLessThan, 1
                },
                { RTypeFunction.ShiftRightArithmetic, 1
                },
                { RTypeFunction.ShiftRightArithmeticVariable, 1
                },
                { RTypeFunction.ShiftRightLogical, 1
                },
                { RTypeFunction.ShiftRightLogicalVariable, 1
                },
                { RTypeFunction.Subtract, 1
                },
                { RTypeFunction.UnsignedSubtract, 1
                },
                /*{ RTypeFunction.Syscall, 1
                },*/
                { RTypeFunction.BitwiseExclusiveOr, 1
                },
            };
        }
        private void FillJTypeInstructionsRuntime()
        {
            JTypeInstructionsRuntimeDict = new Dictionary<JumpType, uint>
            {
                {JumpType.Jump, 1},
                {JumpType.JumpAndLink, 1},
            };
        }
        public InstructionRuntime(uint memoryReadCycles=4, uint memoryWriteCycles=3)
        {
            FillITypeInstructionsRuntime(memoryReadCycles, memoryWriteCycles);
            FillJTypeInstructionsRuntime();
            FillRTypeInstructionsRuntime();
        }
        public uint GetInstructionRuntime(IInstruction inst)
        {
            if (inst is ITypeInstruction)
            {
                var i = inst as ITypeInstruction;
                return ITypeInstructionsRuntimeDict[i.Opcode];
            }
            else if (inst is RTypeInstruction)
            {
                var r = inst as RTypeInstruction;
                return RTypeInstructionsRuntimeDict[r.Function];
            }
            else if (inst is JTypeInstruction)
            {
                var j = inst as JTypeInstruction;
                return JTypeInstructionsRuntimeDict[j.JumpType];
            }
            throw new InvalidOperationException("Invalid Instruction");
        }
    }
}

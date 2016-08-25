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
    public class SubCodeReader
    {
        private uint TextSectionAddr { get; set; }
        public SubCodeReader(uint textSectionAddr)
        {
            TextSectionAddr = textSectionAddr;
        }
        uint getIndex(uint address)
        {
            return (address - TextSectionAddr) >> 2;
        }
        uint getAddress(uint index)
        {
            return (index << 2) + TextSectionAddr;
        }

        private void ParseRec(IList<IInstruction> instructions, uint currentIndex, uint endIndex, SubCode subCode)
        {
            do
            {
                if (subCode.VisitedInstructions.ContainsKey(currentIndex))
                    return;

                var inst = instructions[(int)currentIndex];
                CheckInstruction(inst);
                subCode.AddVisitedInstruction(currentIndex, inst);


                if (inst is ITypeInstruction)
                {
                    var i = inst as ITypeInstruction;
                    switch (i.Opcode)
                    {
                        case Opcode.Branch:
                        case Opcode.BranchOnEqual:
                        case Opcode.BranchOnGreaterThanZero:
                        case Opcode.BranchOnLessThanOrEqualToZero:
                        case Opcode.BranchOnNotEqual:
                            var branchaddr = (uint)(currentIndex + (short)i.Immediate + 1);
                            subCode.Branches.Add(new Branch(currentIndex+1, branchaddr, subCode.VisitedInstructions.ContainsKey(branchaddr)));
                            ParseRec(instructions, branchaddr, endIndex, subCode);
                            
                            break;
                    }
                }
                else if (inst is RTypeInstruction)
                {
                    var r = inst as RTypeInstruction;
                    if (r.Function == RTypeFunction.JumpRegister && r.Rs == 31)
                    {
                        var nextIndex = currentIndex + 1;
                        var ist = instructions[(int)(nextIndex)];
                        CheckInstruction(ist);
                        subCode.AddVisitedInstruction(nextIndex, ist);
                        subCode.Returns.Add(nextIndex);
                        return;
                    }
                }
                else if (inst is JTypeInstruction)
                {
                    var j = inst as JTypeInstruction;
                    if (j.JumpType == JumpType.JumpAndLink)
                    {
                        var fcnsubcode = new SubCode();
                        ParseRec(instructions, j.Target, endIndex, fcnsubcode);
                        subCode.FunctionCalls.Add(new FunctionCall() { CallerAddress = currentIndex + 1, FunctionBody = fcnsubcode, FunctionAddress = j.Target });
                    }
                    else
                    {
                        var nextIndex = currentIndex + 1;
                        var ist = instructions[(int)(nextIndex)];
                        CheckInstruction(ist);
                        subCode.AddVisitedInstruction(nextIndex, ist);
                        currentIndex = j.Target;
                        subCode.Jumps.Add(new Jump() { From = nextIndex, To = j.Target });
                        continue;
                    }
                }

                currentIndex++;
            } while (currentIndex != endIndex + 1);
        }

        public SubCode Parse(IList<IInstruction> instructions, uint taskStart, uint taskEnd)
        {
            var sindex = getIndex(taskStart);
            var currentIndex = sindex;
            var subcode = new SubCode();
            var endIndex = getIndex(taskEnd);
            ParseRec(instructions, currentIndex, endIndex, subcode);
            return subcode;
        }

        private void CheckInstruction(IInstruction inst)
        {
            if (inst is ITypeInstruction)
            {
                var i = inst as ITypeInstruction;
                if (i.Rt == 31)
                {
                    switch (i.Opcode)
                    {
                        case Opcode.Branch:
                        case Opcode.BranchOnEqual:
                        case Opcode.BranchOnGreaterThanZero:
                        case Opcode.BranchOnLessThanOrEqualToZero:
                        case Opcode.BranchOnNotEqual:
                        case Opcode.LoadByte:
                        case Opcode.LoadHalfword:
                        case Opcode.LoadWord:
                        case Opcode.StoreByte:
                        case Opcode.StoreHalfword:
                        case Opcode.StoreWord:
                        case Opcode.UnsignedLoadByte:
                        case Opcode.UnsignedLoadHalfword:
                            break;
                        default:
                            throw new InvalidOperationException(String.Format("Invalid Operation on Ra($31) register: {0}", i.Opcode));
                    }
                }
            }
            else if (inst is RTypeInstruction)
            {
                var r = inst as RTypeInstruction;
                if (r.Function == RTypeFunction.JumpRegister)
                {
                    if (r.Rs != 31)
                        throw new InvalidOperationException(String.Format("JumpRegister with invalid register(${0}), the only allowed is $31", r.Rs));
                }
                else if (r.Function == RTypeFunction.JumpAndLinkRegister)
                    throw new InvalidOperationException("JumpAndLinkRegister is not allowed");
                else if (r.Function == RTypeFunction.Break || r.Function == RTypeFunction.Syscall)
                    throw new InvalidOperationException("Break or Syscall is not allowed");
                else
                    if (r.Rd == 31)
                        throw new InvalidOperationException(String.Format("Invalid Operation on Ra($31) register: {0}", r.Function));
            }
        }
    }
}

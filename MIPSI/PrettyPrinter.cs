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
using System.IO;

namespace MIPSI
{
    public class PrettyPrinter
    {
        private static Dictionary<Type, Action<IInstruction, StreamWriter>> PrettyPrinters { get; set; }
        private static Dictionary<RTypeFunction, Action<RTypeInstruction, StreamWriter>> RTypePrettyPrinters { get; set; }
        private static Dictionary<JumpType, Action<JTypeInstruction, StreamWriter>> JTypePrettyPrinters { get; set; }
        private static Dictionary<Opcode, Action<ITypeInstruction, StreamWriter>> ITypePrettyPrinters { get; set; }
        static PrettyPrinter()
        {
            FillRTypePrettyPrinters();
            FillJTypePrettyPrinters();
            FillITypePrettyPrinters();
            PrettyPrinters = new Dictionary<Type, Action<IInstruction, StreamWriter>>{
                { typeof(RTypeInstruction), (x, writer) =>{
                    var ins = x as RTypeInstruction;
                    RTypePrettyPrinters[ins.Function](ins, writer);
                }},
                { typeof(JTypeInstruction), (x, writer) => {
                    var ins = x as JTypeInstruction;
                    JTypePrettyPrinters[ins.JumpType](ins, writer);
                }},
                { typeof(ITypeInstruction), (x, writer) => {
                    var ins = x as ITypeInstruction;
                    ITypePrettyPrinters[ins.Opcode](ins, writer);
                }},
                { typeof(UndefinedInstruction), (x, writer) => {
                    writer.WriteLine();
                }},
            };
        }

        static void FillRTypePrettyPrinters()
        {
            RTypePrettyPrinters = new Dictionary<RTypeFunction, Action<RTypeInstruction, StreamWriter>>
            {
                {RTypeFunction.Add, (x, writer) => {
                    
                    writer.WriteLine("add ${0} ${1} ${2}", x.Rd, x.Rs, x.Rt);
                }},
                {RTypeFunction.UnsignedAdd, (x, writer) => {
                    
                    writer.WriteLine("addu ${0} ${1} ${2}", x.Rd, x.Rs, x.Rt);
                }},
                {RTypeFunction.BitwiseAnd, (x, writer) => {
                    
                    writer.WriteLine("and ${0} ${1} ${2}", x.Rd, x.Rs, x.Rt);
                }},
                {RTypeFunction.Break, (x, writer) => {
                    var code = (x.Rs << 15) | (x.Rt << 10) | (x.Rd << 5) | x.Sa;
                    writer.WriteLine("break {0}", code);
                }},
                {RTypeFunction.Divide, (x, writer) => {
                    
                    writer.WriteLine("div ${0} ${1}", x.Rs, x.Rt);
                }},
                {RTypeFunction.UnsignedDivide, (x, writer) => {
                    
                    writer.WriteLine("div ${0} ${1}", x.Rs, x.Rt);
                }},
                {RTypeFunction.JumpAndLinkRegister, (x, writer) => {
                    
                    writer.WriteLine("jalr ${0}", x.Rs);
                }},
                {RTypeFunction.JumpRegister, (x, writer) => {
                    
                    writer.WriteLine("jr ${0}", x.Rs);
                }},
                {RTypeFunction.MoveFromHi, (x, writer) => {
                    
                    writer.WriteLine("mfhi ${0}", x.Rd);
                }},
                {RTypeFunction.MoveFromLo, (x, writer) => {
                    
                    writer.WriteLine("mflo ${0}", x.Rd);
                }},
                {RTypeFunction.MoveToHi, (x, writer) => {
                    
                    writer.WriteLine("mthi ${0}", x.Rs);
                }},
                {RTypeFunction.MoveToLo, (x, writer) => {
                    
                    writer.WriteLine("mtlo ${0}", x.Rs);
                }},
                {RTypeFunction.Multiply, (x, writer) => {
                    
                    writer.WriteLine("mult ${0} ${1}", x.Rs, x.Rt);
                }},
                {RTypeFunction.UnsignedMultiply, (x, writer) => {
                    
                    writer.WriteLine("multu ${0} ${1}", x.Rs, x.Rt);
                }},
                {RTypeFunction.BitwiseNor, (x, writer) => {
                    
                    writer.WriteLine("nor ${0} ${1} ${2}", x.Rd, x.Rs, x.Rt);
                }},
                {RTypeFunction.BitwiseOr, (x, writer) => {
                    
                    writer.WriteLine("or ${0} ${1} ${2}", x.Rd, x.Rs, x.Rt);
                }},
                {RTypeFunction.ShiftLeftLogical, (x, writer) => {
                    
                    writer.WriteLine("sll ${0} ${1} ${2}", x.Rd, x.Rt, x.Sa);
                }},
                {RTypeFunction.ShiftLeftLogicalVariable, (x, writer) => {
                    
                    writer.WriteLine("sllv ${0} ${1} ${2}", x.Rd, x.Rt, x.Rs);
                }},
                {RTypeFunction.SetOnLessThan, (x, writer) => {
                    
                    writer.WriteLine("slt ${0} ${1} ${2}", x.Rd, x.Rs, x.Rt);
                }},
                {RTypeFunction.UnsignedSetOnLessThan, (x, writer) => {
                    
                    writer.WriteLine("sltu ${0} ${1} ${2}", x.Rd, x.Rs, x.Rt);
                }},
                {RTypeFunction.ShiftRightArithmetic, (x, writer) => {
                    
                    writer.WriteLine("sra ${0} ${1} ${2}", x.Rd, x.Rt, x.Sa);
                }},
                {RTypeFunction.ShiftRightArithmeticVariable, (x, writer) => {
                    
                    writer.WriteLine("srav ${0} ${1} ${2}", x.Rd, x.Rt, x.Rs);
                }},
                {RTypeFunction.ShiftRightLogical, (x, writer) => {
                    
                    writer.WriteLine("srl ${0} ${1} ${2}", x.Rd, x.Rt, x.Sa);
                }},
                {RTypeFunction.ShiftRightLogicalVariable, (x, writer) => {
                    
                    writer.WriteLine("srlv ${0} ${1} ${2}", x.Rd, x.Rt, x.Rs);
                }},
                {RTypeFunction.Subtract, (x, writer) => {
                    
                    writer.WriteLine("sub ${0} ${1} ${2}", x.Rd, x.Rs, x.Rt);
                }},
                {RTypeFunction.UnsignedSubtract, (x, writer) => {
                    
                    writer.WriteLine("subu ${0} ${1} ${2}", x.Rd, x.Rs, x.Rt);
                }},
                {RTypeFunction.Syscall, (x, writer) => {
                    
                    writer.WriteLine("syscall");
                }},
                {RTypeFunction.BitwiseExclusiveOr, (x, writer) => {
                    
                    writer.WriteLine("xor ${0} ${1} ${2}", x.Rd, x.Rs, x.Rt);
                }},
            };
        }
        static void FillJTypePrettyPrinters()
        {
            JTypePrettyPrinters = new Dictionary<JumpType, Action<JTypeInstruction, StreamWriter>>
            {
                { JumpType.Jump, (x, writer) => {
                    writer.WriteLine("j {0}", x.Target);
                }},
                { JumpType.JumpAndLink, (x, writer) => {
                    writer.WriteLine("jal {0}", x.Target);
                }},
            };
        }
        static void FillITypePrettyPrinters()
        {
            ITypePrettyPrinters = new Dictionary<Opcode, Action<ITypeInstruction, StreamWriter>>
            {
                { Opcode.AddImmediate, (x, writer) => {
                    writer.WriteLine("addi ${0} ${1} {2}", x.Rt, x.Rs, printImmediate(x.Immediate));
                }},
                { Opcode.UnsignedAddImmediate, (x, writer) => {
                    writer.WriteLine("addiu ${0} ${1} {2}", x.Rt, x.Rs, printImmediate(x.Immediate));
                }},
                { Opcode.BitwiseAndImmediate, (x, writer) => {
                    writer.WriteLine("andi ${0} ${1} {2}", x.Rt, x.Rs, printImmediate(x.Immediate));
                }},
                { Opcode.BranchOnEqual, (x, writer) => {
                    writer.WriteLine("beq ${0} ${1} {2}", x.Rs, x.Rt, printImmediate(x.Immediate));
                }},
                { Opcode.Branch, (x, writer) => {
                    switch(x.Rt)
                    {
                        case 0:
                            writer.WriteLine("bltz ${0} {1}", x.Rs, printImmediate(x.Immediate));
                            break;
                        case 1:
                            writer.WriteLine("bgez ${0} {1}", x.Rs, printImmediate(x.Immediate));
                            break;
                        case 17:
                            writer.WriteLine("bgezal ${0} {1}", x.Rs, printImmediate(x.Immediate));
                            break;
                        case 16:
                            writer.WriteLine("bltzal ${0} {1}", x.Rs, printImmediate(x.Immediate));
                            break;
                        default:
                            throw new InvalidOperationException(String.Format("Invalid Branch Type: {0}", x.Rt));

                    }
                }},
                { Opcode.BranchOnGreaterThanZero, (x, writer) => {
                    writer.WriteLine("bgtz ${0} {1}", x.Rs, printImmediate(x.Immediate));
                }},
                { Opcode.BranchOnLessThanOrEqualToZero, (x, writer) => {
                    writer.WriteLine("blez ${0} {1}", x.Rs, printImmediate(x.Immediate));
                }},
                { Opcode.BranchOnNotEqual, (x, writer) => {
                    writer.WriteLine("bne ${0} ${1} {2}", x.Rs, x.Rt, printImmediate(x.Immediate));
                }},
                { Opcode.LoadByte, (x, writer) => {
                    writer.WriteLine("lb ${0}, {1}(${2})", x.Rt, printImmediate(x.Immediate), x.Rs);
                }},
                { Opcode.UnsignedLoadByte, (x, writer) => {
                    writer.WriteLine("lbu ${0}, {1}(${2})", x.Rt, printImmediate(x.Immediate), x.Rs);
                }},
                { Opcode.LoadHalfword, (x, writer) => {
                    writer.WriteLine("lh ${0}, {1}(${2})", x.Rt, printImmediate(x.Immediate), x.Rs);
                }},
                { Opcode.UnsignedLoadHalfword, (x, writer) => {
                    writer.WriteLine("lhu ${0}, {1}(${2})", x.Rt, printImmediate(x.Immediate), x.Rs);
                }},
                { Opcode.LoadUpperImmediate, (x, writer) => {
                    writer.WriteLine("lui ${0}, {1}", x.Rt, printImmediate(x.Immediate));
                }},
                { Opcode.LoadWord, (x, writer) => {
                    writer.WriteLine("lw ${0}, {1}(${2})", x.Rt, printImmediate(x.Immediate), x.Rs);
                }},
                { Opcode.BitwiseOrImmediate, (x, writer) => {
                    writer.WriteLine("ori ${0} ${1} {2}", x.Rt, x.Rs, printImmediate(x.Immediate));
                }},
                { Opcode.StoreByte, (x, writer) => {
                    writer.WriteLine("sb ${0}, {1}(${2})", x.Rt, printImmediate(x.Immediate), x.Rs);
                }},
                { Opcode.SetOnLessThanImmediate, (x, writer) => {
                    writer.WriteLine("slti ${0} ${1} {2}", x.Rt, x.Rs, printImmediate(x.Immediate));
                }},
                { Opcode.UnsignedSetOnLessThanImmediate, (x, writer) => {
                    writer.WriteLine("sltiu ${0} ${1} {2}", x.Rt, x.Rs, printImmediate(x.Immediate));
                }},
                { Opcode.StoreHalfword, (x, writer) => {
                    writer.WriteLine("sh ${0}, {1}(${2})", x.Rt, printImmediate(x.Immediate), x.Rs);
                }},
                { Opcode.StoreWord, (x, writer) => {
                    writer.WriteLine("sw ${0}, {1}(${2})", x.Rt, printImmediate(x.Immediate), x.Rs);
                }},
                { Opcode.BitwiseExclusiveOrImmediate, (x, writer) => {
                    writer.WriteLine("xori ${0} ${1} {2}", x.Rt, x.Rs, printImmediate(x.Immediate));
                }},
            };
        }
        private static string printImmediate(uint v)
        {
            return "0x" + v.ToString("X").PadLeft(4, '0');
        }
        public static void PrettyPrint(IList<IInstruction> instructions, StreamWriter writer)
        {
            foreach (var x in instructions)
            {
                PrettyPrint(writer, x);
            }
        }

        public static void PrettyPrint(StreamWriter writer, IInstruction x, bool printAddress=true)
        {
            if (printAddress)
                writer.Write("{0} {1}: ", (x.Address).ToString("X").PadLeft(8, '0'), x.ToInteger().ToString("X").PadLeft(8, '0'));
            PrettyPrinters[x.GetType()](x, writer);
        }
    }
}

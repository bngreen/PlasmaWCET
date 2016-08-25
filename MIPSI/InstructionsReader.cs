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
    public class InstructionsReader
    {
        private Func<uint, uint> DataConverterUInt32 { get; set; }
        private Func<UInt16, UInt16> DataConverterUInt16 { get; set; }
        private Dictionary<JumpType, Action<JTypeInstruction, IStateAccessor>> JTypeExecutors { get; set; }
        private Dictionary<RTypeFunction, Action<RTypeInstruction, IStateAccessor>> RTypeExecutors { get; set; }
        private Dictionary<Opcode, Action<ITypeInstruction, IStateAccessor>> ITypeExecutors { get; set; }
        private Dictionary<CoProcessorFormat, Action<CoProcessorInstruction, IStateAccessor>> CoProcessorExecutors { get; set; }
        private void populateRTypeExecutors()
        {
            RTypeExecutors = new Dictionary<RTypeFunction, Action<RTypeInstruction, IStateAccessor>>
            {
                { RTypeFunction.Add, (r, StateAccessor) => {
                    StateAccessor.WriteRegister(r.Rd, (uint)(((int)StateAccessor.ReadRegister(r.Rs)) + (int)(StateAccessor.ReadRegister(r.Rt))));
                }},
                { RTypeFunction.UnsignedAdd, (r, StateAccessor) => {
                    StateAccessor.WriteRegister(r.Rd, StateAccessor.ReadRegister(r.Rs) + StateAccessor.ReadRegister(r.Rt));
                }},
                { RTypeFunction.BitwiseAnd, (r, StateAccessor) => {
                    StateAccessor.WriteRegister(r.Rd, StateAccessor.ReadRegister(r.Rs) & StateAccessor.ReadRegister(r.Rt));
                }},
                { RTypeFunction.Break, (r, StateAccessor) => {
                    StateAccessor.SetEpc(StateAccessor.GetPc());
                    StateAccessor.SetPc(0x3c);
                    StateAccessor.Skip();
                }},
                { RTypeFunction.Divide, (r, StateAccessor) => {
                    var dividend = (int)StateAccessor.ReadRegister(r.Rs);
                    var divisor = (int)StateAccessor.ReadRegister(r.Rt);
                    StateAccessor.SetHi((uint)(dividend % divisor));
                    StateAccessor.SetLo((uint)(dividend / divisor));
                }},
                { RTypeFunction.UnsignedDivide, (r, StateAccessor) => {
                    var dividend = StateAccessor.ReadRegister(r.Rs);
                    var divisor = StateAccessor.ReadRegister(r.Rt);
                    StateAccessor.SetHi(dividend % divisor);
                    StateAccessor.SetLo(dividend / divisor);
                }},
                { RTypeFunction.JumpAndLinkRegister, (r, StateAccessor) => {
                    StateAccessor.WriteRegister(r.Rd, StateAccessor.GetPc()+4);
                    StateAccessor.SetPc(StateAccessor.ReadRegister(r.Rs));
                }},
                { RTypeFunction.JumpRegister, (r, StateAccessor) => {
                    StateAccessor.SetPc(StateAccessor.ReadRegister(r.Rs));
                }},
                { RTypeFunction.MoveFromHi, (r, StateAccessor) => {
                    StateAccessor.WriteRegister(r.Rd ,StateAccessor.GetHi());
                }},
                { RTypeFunction.MoveFromLo, (r, StateAccessor) => {
                    StateAccessor.WriteRegister(r.Rd ,StateAccessor.GetLo());
                }},
                { RTypeFunction.MoveToHi, (r, StateAccessor) => {
                    StateAccessor.SetHi(StateAccessor.ReadRegister(r.Rs));
                }},
                { RTypeFunction.MoveToLo, (r, StateAccessor) => {
                    StateAccessor.SetLo(StateAccessor.ReadRegister(r.Rs));
                }},
                { RTypeFunction.Multiply, (r, StateAccessor) => {
                    var a = (long)(int)StateAccessor.ReadRegister(r.Rs);
                    var b = (long)(int)StateAccessor.ReadRegister(r.Rt);
                    long result = a * b;
                    StateAccessor.SetLo((uint)((int)result));
                    StateAccessor.SetHi((uint)(result >> 32));
                }},
                { RTypeFunction.UnsignedMultiply, (r, StateAccessor) => {
                    var a = (ulong)StateAccessor.ReadRegister(r.Rs);
                    var b = (ulong)StateAccessor.ReadRegister(r.Rt);
                    var result = a * b;
                    StateAccessor.SetLo((uint)result);
                    StateAccessor.SetHi((uint)(result >> 32));
                }},
                { RTypeFunction.BitwiseNor, (r, StateAccessor) => {
                    StateAccessor.WriteRegister(r.Rd, ~(StateAccessor.ReadRegister(r.Rs) | StateAccessor.ReadRegister(r.Rt)));
                }},
                { RTypeFunction.BitwiseOr, (r, StateAccessor) => {
                    StateAccessor.WriteRegister(r.Rd, (StateAccessor.ReadRegister(r.Rs) | StateAccessor.ReadRegister(r.Rt)));
                }},
                { RTypeFunction.ShiftLeftLogical, (r, StateAccessor) => {
                    var a = StateAccessor.ReadRegister(r.Rt);
                    var b = (int)r.Sa;
                    StateAccessor.WriteRegister(r.Rd, a << b);
                }},
                { RTypeFunction.ShiftLeftLogicalVariable, (r, StateAccessor) => {
                    var a = StateAccessor.ReadRegister(r.Rt);
                    var b = (int)StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rd, a << b);
                }},
                { RTypeFunction.SetOnLessThan, (r, StateAccessor) => {
                    var a = (int)StateAccessor.ReadRegister(r.Rs);
                    var b = (int)StateAccessor.ReadRegister(r.Rt);
                    StateAccessor.WriteRegister(r.Rd, a < b ? 1u : 0u);
                }},
                { RTypeFunction.UnsignedSetOnLessThan, (r, StateAccessor) => {
                    var a = StateAccessor.ReadRegister(r.Rs);
                    var b = StateAccessor.ReadRegister(r.Rt);
                    StateAccessor.WriteRegister(r.Rd, a < b ? 1u : 0u);
                }},
                { RTypeFunction.ShiftRightArithmetic, (r, StateAccessor) => {
                    var a = (int)StateAccessor.ReadRegister(r.Rt);
                    var b = (int)r.Sa;
                    StateAccessor.WriteRegister(r.Rd, (uint)(a >> b));
                }},
                { RTypeFunction.ShiftRightArithmeticVariable, (r, StateAccessor) => {
                    var a = (int)StateAccessor.ReadRegister(r.Rt);
                    var b = (int)StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rd, (uint)(a >> b));
                }},
                { RTypeFunction.ShiftRightLogical, (r, StateAccessor) => {
                    var a = StateAccessor.ReadRegister(r.Rt);
                    var b = (int)r.Sa;
                    StateAccessor.WriteRegister(r.Rd, a >> b);
                }},
                { RTypeFunction.ShiftRightLogicalVariable, (r, StateAccessor) => {
                    var a = StateAccessor.ReadRegister(r.Rt);
                    var b = (int)StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rd, a >> b);
                }},
                { RTypeFunction.Subtract, (r, StateAccessor) => {
                    var a = (int)StateAccessor.ReadRegister(r.Rs);
                    var b = (int)StateAccessor.ReadRegister(r.Rt);
                    StateAccessor.WriteRegister(r.Rd, (uint)(a - b));
                }},
                { RTypeFunction.UnsignedSubtract, (r, StateAccessor) => {
                    var a = (int)StateAccessor.ReadRegister(r.Rs);
                    var b = (int)StateAccessor.ReadRegister(r.Rt);
                    StateAccessor.WriteRegister(r.Rd, (uint)(a - b));
                }},
                { RTypeFunction.Syscall, (r, StateAccessor) => {
                    StateAccessor.SetEpc(StateAccessor.GetPc());
                    StateAccessor.SetPc(0x3c);
                    StateAccessor.Skip();
                }},
                { RTypeFunction.BitwiseExclusiveOr, (r, StateAccessor) => {
                    StateAccessor.WriteRegister(r.Rd, (StateAccessor.ReadRegister(r.Rs) ^ StateAccessor.ReadRegister(r.Rt)));
                }},
            };
        }
        private void populateJTypeExecutors()
        {
            JTypeExecutors = new Dictionary<JumpType, Action<JTypeInstruction, IStateAccessor>>
            {
                {JumpType.Jump, (x, StateAccessor) => {
                    var pc = StateAccessor.GetPc() & 0xFC000000;
                    pc |= x.Target << 2;
                    StateAccessor.SetPc(pc);
                }},
                {JumpType.JumpAndLink, (x, StateAccessor) => {
                    StateAccessor.WriteRegister(31, StateAccessor.GetPc()+4);
                    var pc = StateAccessor.GetPc() & 0xFC000000;
                    pc |= x.Target << 2;
                    StateAccessor.SetPc(pc);
                }},
            };
        }
        private void populateITypeExecutors()
        {
            ITypeExecutors = new Dictionary<Opcode, Action<ITypeInstruction, IStateAccessor>>
            {
                { Opcode.AddImmediate, (r, StateAccessor) => {
                    var a = (int)StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rt, (uint)(a + ((short)r.Immediate)));
                }},
                { Opcode.UnsignedAddImmediate, (r, StateAccessor) => {
                    var a = StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rt, (uint)(a + (short)r.Immediate));
                }},
                { Opcode.BitwiseAndImmediate, (r, StateAccessor) => {
                    var a = StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rt, a & r.Immediate);
                }},
                { Opcode.BranchOnEqual, (r, StateAccessor) => {
                    var rs = StateAccessor.ReadRegister(r.Rs);
                    var rt = StateAccessor.ReadRegister(r.Rt);
                    if (rs == rt)
                        StateAccessor.SetPc((uint)(StateAccessor.GetPc() + (short)(r.Immediate << 2)));
                }},
                { Opcode.Branch, (r, StateAccessor) => {
                    var rs = (int)StateAccessor.ReadRegister(r.Rs);
                    var rt = r.Rt;
                    switch(rt)
                    {
                        case 0:
                            if(rs < 0)
                                StateAccessor.SetPc((uint)(StateAccessor.GetPc() + (short)(r.Immediate << 2)));
                            break;
                        case 1:
                            if(rs >= 0)
                                StateAccessor.SetPc((uint)(StateAccessor.GetPc() + (short)(r.Immediate << 2)));
                            break;
                        case 16:
                            StateAccessor.WriteRegister(31, StateAccessor.GetPc()+4);
                            if(rs < 0)
                                StateAccessor.SetPc((uint)(StateAccessor.GetPc() + (short)(r.Immediate << 2)));
                            break;
                        case 17:
                            StateAccessor.WriteRegister(31, StateAccessor.GetPc()+4);
                            if(rs >= 0)
                                StateAccessor.SetPc((uint)(StateAccessor.GetPc() + (short)(r.Immediate << 2)));
                            break;
                        default:
                            throw new InvalidOperationException(String.Format("Invalid Branch {0}", rt));
                        
                    }
                }},
                { Opcode.BranchOnGreaterThanZero, (r, StateAccessor) => {
                    var rs = (int)StateAccessor.ReadRegister(r.Rs);
                    var rt = StateAccessor.ReadRegister(r.Rt);
                    if (rs > 0)
                        StateAccessor.SetPc((uint)(StateAccessor.GetPc() + (short)(r.Immediate << 2)));
                }},
                { Opcode.BranchOnLessThanOrEqualToZero, (r, StateAccessor) => {
                    var rs = (int)StateAccessor.ReadRegister(r.Rs);
                    var rt = StateAccessor.ReadRegister(r.Rt);
                    if (rs <= 0)
                        StateAccessor.SetPc((uint)(StateAccessor.GetPc() + (short)(r.Immediate << 2)));
                }},
                { Opcode.BranchOnNotEqual, (r, StateAccessor) => {
                    var rs = StateAccessor.ReadRegister(r.Rs);
                    var rt = StateAccessor.ReadRegister(r.Rt);
                    if (rs != rt)
                        StateAccessor.SetPc((uint)(StateAccessor.GetPc() + (short)(r.Immediate << 2)));
                }},
                { Opcode.LoadByte, (r, StateAccessor) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    byte dt;
                    StateAccessor.GetMemoryData(ptr, out dt);
                    var data = (uint)((int)((sbyte)(dt)));
                    StateAccessor.WriteRegister(r.Rt, data);
                }},
                { Opcode.UnsignedLoadByte, (r, StateAccessor) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    byte dt;
                    StateAccessor.GetMemoryData(ptr, out dt);
                    var data = (uint)dt;
                    StateAccessor.WriteRegister(r.Rt, data); 
                }},
                { Opcode.LoadHalfword, (r, StateAccessor) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    UInt16 dt;
                    StateAccessor.GetMemoryData(ptr, out dt);
                    var data = (uint)((int)((short)DataConverterUInt16(dt)));
                    StateAccessor.WriteRegister(r.Rt, data);
                }},
                { Opcode.UnsignedLoadHalfword, (r, StateAccessor) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    UInt16 dt;
                    StateAccessor.GetMemoryData(ptr, out dt);
                    var data = DataConverterUInt16(dt);
                    StateAccessor.WriteRegister(r.Rt, data);
                }},
                { Opcode.LoadUpperImmediate, (r, StateAccessor) => {
                    StateAccessor.WriteRegister(r.Rt, r.Immediate << 16);
                }},
                { Opcode.LoadWord, (r, StateAccessor) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    UInt32 data;
                    StateAccessor.GetMemoryData(ptr, out data);
                    data = DataConverterUInt32(data);
                    StateAccessor.WriteRegister(r.Rt, data);
                }},
                { Opcode.BitwiseOrImmediate, (r, StateAccessor) => {
                    var a = StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rt, a | r.Immediate);
                }},
                { Opcode.StoreByte, (r, StateAccessor) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    var data = (byte)(StateAccessor.ReadRegister(r.Rt));
                    StateAccessor.SetMemoryData(ptr, data);
                }},
                { Opcode.SetOnLessThanImmediate, (r, StateAccessor) => {
                    var a = (int)StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rt, a < (short)r.Immediate ? 1u : 0u);
                }},
                { Opcode.UnsignedSetOnLessThanImmediate, (r, StateAccessor) => {
                    var a = StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rt, a < r.Immediate ? 1u : 0u);
                }},
                { Opcode.StoreHalfword, (r, StateAccessor) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    var dt = (ushort)StateAccessor.ReadRegister(r.Rt);
                    dt = DataConverterUInt16(dt);
                    StateAccessor.SetMemoryData(ptr, (ushort)dt);
                }},
                { Opcode.StoreWord, (r, StateAccessor) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    var data = DataConverterUInt32(StateAccessor.ReadRegister(r.Rt));
                    StateAccessor.SetMemoryData(ptr, data);
                }},
                { Opcode.BitwiseExclusiveOrImmediate, (r, StateAccessor) => {
                    var a = StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rt, a ^ r.Immediate);
                }},
            };
        }
        private void populateCoProcessorExecutors()
        {
            CoProcessorExecutors = new Dictionary<CoProcessorFormat, Action<CoProcessorInstruction, IStateAccessor>>()
            {
                { CoProcessorFormat.MoveFromCoprocessor, (x, StateAccessor) => {
                    StateAccessor.WriteRegister(x.Rt, StateAccessor.GetCoprocessorRegister(x.Rd));
                }},
                { CoProcessorFormat.MoveToCoprocessor, (x, StateAccessor) => {
                    StateAccessor.SetCoprocessorRegister(x.Rt, StateAccessor.ReadRegister(x.Rd));
                }},
            };
        }
        public bool LittleEndian { get; private set; }
        public InstructionsReader(bool littleEndian)
        {
            populateRTypeExecutors();
            populateJTypeExecutors();
            populateITypeExecutors();
            populateCoProcessorExecutors();
            LittleEndian = littleEndian;
            if (littleEndian)
            {
                DataConverterUInt32 = (x) => x;
                DataConverterUInt16 = (x) => x;
            }
            else
            {
                DataConverterUInt32 = Utils.ReverseUInt32;
                DataConverterUInt16 = Utils.ReverseUInt16;
            }
        }

        public IInstruction ReadInstruction(uint inst)
        {
            if (!Enum.IsDefined(typeof(Opcode), inst >> 26))
                return UndefinedInstruction.FromInteger(inst);
            var opcode = (Opcode)(inst >> 26);
            switch (opcode)
            {
                case Opcode.RType:
                    var rIns = RTypeInstruction.FromInteger(inst);
                    rIns.Executor = RTypeExecutors[rIns.Function];
                    return rIns;
                case Opcode.Jump:
                case Opcode.JumpAndLink:
                    var jIns = JTypeInstruction.FromInteger(inst);
                    jIns.Executor = JTypeExecutors[jIns.JumpType];
                    return jIns;
                case Opcode.CoProcessor:
                    var coIns = CoProcessorInstruction.FromInteger(inst);
                    coIns.Executor = CoProcessorExecutors[coIns.Format];
                    return coIns;
                default:
                    var iIns = ITypeInstruction.FromInteger(inst);
                    iIns.Executor = ITypeExecutors[iIns.Opcode];
                    return iIns;
            }
        }

        public IList<IInstruction> ReadInstructions(BinaryReader reader, bool littleEndian=false, uint TextSectionAddress = 0)
        {
            var list = new List<IInstruction>();
            Func<uint> readUInt = () => reader.ReadUInt32();
            if (!littleEndian)
                readUInt = () => Utils.ReverseUInt32(reader.ReadUInt32());
            uint x = TextSectionAddress;
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var inst = readUInt();
                list.Add(ReadInstruction(inst));
                //if (!Enum.IsDefined(typeof(Opcode), inst >> 26))
                //{
                //    list.Add(UndefinedInstruction.FromInteger(inst));
                //}
                //else
                //{
                //    var opcode = (Opcode)(inst >> 26);
                //    switch (opcode)
                //    {
                //        case Opcode.RType:
                //            var rIns = RTypeInstruction.FromInteger(inst);
                //            rIns.Executor = RTypeExecutors[rIns.Function];
                //            list.Add(rIns);
                //            break;
                //        case Opcode.Jump:
                //        case Opcode.JumpAndLink:
                //            var jIns = JTypeInstruction.FromInteger(inst);
                //            jIns.Executor = JTypeExecutors[jIns.JumpType];
                //            list.Add(jIns);
                //            break;
                //        case Opcode.CoProcessor:
                //            var coIns = CoProcessorInstruction.FromInteger(inst);
                //            coIns.Executor = CoProcessorExecutors[coIns.Format];
                //            list.Add(coIns);
                //            break;
                //        default:
                //            var iIns = ITypeInstruction.FromInteger(inst);
                //            iIns.Executor = ITypeExecutors[iIns.Opcode];
                //            list.Add(iIns);
                //            break;
                //    }
                //}
                list.Last().Address = x;
                x += 4;
            }
            return list;
        }
    }
}

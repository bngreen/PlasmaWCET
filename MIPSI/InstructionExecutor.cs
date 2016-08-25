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
    public class InstructionExecutor
    {
        private IStateAccessor StateAccessor { get; set; }
        private Dictionary<RTypeFunction, Action<RTypeInstruction>> RTypeExecutors { get; set; }
        private Dictionary<JumpType, Action<JTypeInstruction>> JTypeExecutors { get; set; }
        private Dictionary<Opcode, Action<ITypeInstruction>> ITypeExecutors { get; set; }
        private Dictionary<CoProcessorFormat, Action<CoProcessorInstruction>> CoProcessorExecutors { get; set; }
        private Func<uint, uint> DataConverterUInt32 { get; set; }
        private Func<UInt16, UInt16> DataConverterUInt16 { get; set; }
        private void populateRTypeExecutors()
        {
            RTypeExecutors = new Dictionary<RTypeFunction, Action<RTypeInstruction>>
            {
                { RTypeFunction.Add, (r) => {
                    StateAccessor.WriteRegister(r.Rd, (uint)(((int)StateAccessor.ReadRegister(r.Rs)) + (int)(StateAccessor.ReadRegister(r.Rt))));
                }},
                { RTypeFunction.UnsignedAdd, (r) => {
                    StateAccessor.WriteRegister(r.Rd, StateAccessor.ReadRegister(r.Rs) + StateAccessor.ReadRegister(r.Rt));
                }},
                { RTypeFunction.BitwiseAnd, (r) => {
                    StateAccessor.WriteRegister(r.Rd, StateAccessor.ReadRegister(r.Rs) & StateAccessor.ReadRegister(r.Rt));
                }},
                { RTypeFunction.Break, (r) => {
                    StateAccessor.SetEpc(StateAccessor.GetPc());
                    StateAccessor.SetPc(0x3c);
                    StateAccessor.Skip();
                }},
                { RTypeFunction.Divide, (r) => {
                    var dividend = (int)StateAccessor.ReadRegister(r.Rs);
                    var divisor = (int)StateAccessor.ReadRegister(r.Rt);
                    StateAccessor.SetHi((uint)(dividend % divisor));
                    StateAccessor.SetLo((uint)(dividend / divisor));
                }},
                { RTypeFunction.UnsignedDivide, (r) => {
                    var dividend = StateAccessor.ReadRegister(r.Rs);
                    var divisor = StateAccessor.ReadRegister(r.Rt);
                    StateAccessor.SetHi(dividend % divisor);
                    StateAccessor.SetLo(dividend / divisor);
                }},
                { RTypeFunction.JumpAndLinkRegister, (r) => {
                    StateAccessor.WriteRegister(r.Rd, StateAccessor.GetPc()+4);
                    StateAccessor.SetPc(StateAccessor.ReadRegister(r.Rs));
                }},
                { RTypeFunction.JumpRegister, (r) => {
                    StateAccessor.SetPc(StateAccessor.ReadRegister(r.Rs));
                }},
                { RTypeFunction.MoveFromHi, (r) => {
                    StateAccessor.WriteRegister(r.Rd ,StateAccessor.GetHi());
                }},
                { RTypeFunction.MoveFromLo, (r) => {
                    StateAccessor.WriteRegister(r.Rd ,StateAccessor.GetLo());
                }},
                { RTypeFunction.MoveToHi, (r) => {
                    StateAccessor.SetHi(StateAccessor.ReadRegister(r.Rs));
                }},
                { RTypeFunction.MoveToLo, (r) => {
                    StateAccessor.SetLo(StateAccessor.ReadRegister(r.Rs));
                }},
                { RTypeFunction.Multiply, (r) => {
                    var a = (long)(int)StateAccessor.ReadRegister(r.Rs);
                    var b = (long)(int)StateAccessor.ReadRegister(r.Rt);
                    long result = a * b;
                    StateAccessor.SetLo((uint)((int)result));
                    StateAccessor.SetHi((uint)(result >> 32));
                }},
                { RTypeFunction.UnsignedMultiply, (r) => {
                    var a = (ulong)StateAccessor.ReadRegister(r.Rs);
                    var b = (ulong)StateAccessor.ReadRegister(r.Rt);
                    var result = a * b;
                    StateAccessor.SetLo((uint)result);
                    StateAccessor.SetHi((uint)(result >> 32));
                }},
                { RTypeFunction.BitwiseNor, (r) => {
                    StateAccessor.WriteRegister(r.Rd, ~(StateAccessor.ReadRegister(r.Rs) | StateAccessor.ReadRegister(r.Rt)));
                }},
                { RTypeFunction.BitwiseOr, (r) => {
                    StateAccessor.WriteRegister(r.Rd, (StateAccessor.ReadRegister(r.Rs) | StateAccessor.ReadRegister(r.Rt)));
                }},
                { RTypeFunction.ShiftLeftLogical, (r) => {
                    var a = StateAccessor.ReadRegister(r.Rt);
                    var b = (int)r.Sa;
                    StateAccessor.WriteRegister(r.Rd, a << b);
                }},
                { RTypeFunction.ShiftLeftLogicalVariable, (r) => {
                    var a = StateAccessor.ReadRegister(r.Rt);
                    var b = (int)StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rd, a << b);
                }},
                { RTypeFunction.SetOnLessThan, (r) => {
                    var a = (int)StateAccessor.ReadRegister(r.Rs);
                    var b = (int)StateAccessor.ReadRegister(r.Rt);
                    StateAccessor.WriteRegister(r.Rd, a < b ? 1u : 0u);
                }},
                { RTypeFunction.UnsignedSetOnLessThan, (r) => {
                    var a = StateAccessor.ReadRegister(r.Rs);
                    var b = StateAccessor.ReadRegister(r.Rt);
                    StateAccessor.WriteRegister(r.Rd, a < b ? 1u : 0u);
                }},
                { RTypeFunction.ShiftRightArithmetic, (r) => {
                    var a = (int)StateAccessor.ReadRegister(r.Rt);
                    var b = (int)r.Sa;
                    StateAccessor.WriteRegister(r.Rd, (uint)(a >> b));
                }},
                { RTypeFunction.ShiftRightArithmeticVariable, (r) => {
                    var a = (int)StateAccessor.ReadRegister(r.Rt);
                    var b = (int)StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rd, (uint)(a >> b));
                }},
                { RTypeFunction.ShiftRightLogical, (r) => {
                    var a = StateAccessor.ReadRegister(r.Rt);
                    var b = (int)r.Sa;
                    StateAccessor.WriteRegister(r.Rd, a >> b);
                }},
                { RTypeFunction.ShiftRightLogicalVariable, (r) => {
                    var a = StateAccessor.ReadRegister(r.Rt);
                    var b = (int)StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rd, a >> b);
                }},
                { RTypeFunction.Subtract, (r) => {
                    var a = (int)StateAccessor.ReadRegister(r.Rs);
                    var b = (int)StateAccessor.ReadRegister(r.Rt);
                    StateAccessor.WriteRegister(r.Rd, (uint)(a - b));
                }},
                { RTypeFunction.UnsignedSubtract, (r) => {
                    var a = (int)StateAccessor.ReadRegister(r.Rs);
                    var b = (int)StateAccessor.ReadRegister(r.Rt);
                    StateAccessor.WriteRegister(r.Rd, (uint)(a - b));
                }},
                { RTypeFunction.Syscall, (r) => {
                    StateAccessor.SetEpc(StateAccessor.GetPc());
                    StateAccessor.SetPc(0x3c);
                    StateAccessor.Skip();
                }},
                { RTypeFunction.BitwiseExclusiveOr, (r) => {
                    StateAccessor.WriteRegister(r.Rd, (StateAccessor.ReadRegister(r.Rs) ^ StateAccessor.ReadRegister(r.Rt)));
                }},
            };
        }
        private void populateJTypeExecutors()
        {
            JTypeExecutors = new Dictionary<JumpType, Action<JTypeInstruction>>
            {
                {JumpType.Jump, (x) => {
                    var pc = StateAccessor.GetPc() & 0xFC000000;
                    pc |= x.Target << 2;
                    StateAccessor.SetPc(pc);
                }},
                {JumpType.JumpAndLink, (x) => {
                    StateAccessor.WriteRegister(31, StateAccessor.GetPc()+4);
                    var pc = StateAccessor.GetPc() & 0xFC000000;
                    pc |= x.Target << 2;
                    StateAccessor.SetPc(pc);
                }},
            };
        }
        private void populateITypeExecutors()
        {
            ITypeExecutors = new Dictionary<Opcode, Action<ITypeInstruction>>
            {
                { Opcode.AddImmediate, (r) => {
                    var a = (int)StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rt, (uint)(a + ((short)r.Immediate)));
                }},
                { Opcode.UnsignedAddImmediate, (r) => {
                    var a = StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rt, (uint)(a + (short)r.Immediate));
                }},
                { Opcode.BitwiseAndImmediate, (r) => {
                    var a = StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rt, a & r.Immediate);
                }},
                { Opcode.BranchOnEqual, (r) => {
                    var rs = StateAccessor.ReadRegister(r.Rs);
                    var rt = StateAccessor.ReadRegister(r.Rt);
                    if (rs == rt)
                        StateAccessor.SetPc((uint)(StateAccessor.GetPc() + (short)(r.Immediate << 2)));
                }},
                { Opcode.Branch, (r) => {
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
                { Opcode.BranchOnGreaterThanZero, (r) => {
                    var rs = (int)StateAccessor.ReadRegister(r.Rs);
                    var rt = StateAccessor.ReadRegister(r.Rt);
                    if (rs > 0)
                        StateAccessor.SetPc((uint)(StateAccessor.GetPc() + (short)(r.Immediate << 2)));
                }},
                { Opcode.BranchOnLessThanOrEqualToZero, (r) => {
                    var rs = (int)StateAccessor.ReadRegister(r.Rs);
                    var rt = StateAccessor.ReadRegister(r.Rt);
                    if (rs <= 0)
                        StateAccessor.SetPc((uint)(StateAccessor.GetPc() + (short)(r.Immediate << 2)));
                }},
                { Opcode.BranchOnNotEqual, (r) => {
                    var rs = StateAccessor.ReadRegister(r.Rs);
                    var rt = StateAccessor.ReadRegister(r.Rt);
                    if (rs != rt)
                        StateAccessor.SetPc((uint)(StateAccessor.GetPc() + (short)(r.Immediate << 2)));
                }},
                { Opcode.LoadByte, (r) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    byte dt;
                    StateAccessor.GetMemoryData(ptr, out dt);
                    var data = (uint)((int)((sbyte)(dt)));
                    StateAccessor.WriteRegister(r.Rt, data);
                }},
                { Opcode.UnsignedLoadByte, (r) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    byte dt;
                    StateAccessor.GetMemoryData(ptr, out dt);
                    var data = (uint)dt;
                    StateAccessor.WriteRegister(r.Rt, data); 
                }},
                { Opcode.LoadHalfword, (r) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    UInt16 dt;
                    StateAccessor.GetMemoryData(ptr, out dt);
                    var data = (uint)((int)((short)DataConverterUInt16(dt)));
                    StateAccessor.WriteRegister(r.Rt, data);
                }},
                { Opcode.UnsignedLoadHalfword, (r) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    UInt16 dt;
                    StateAccessor.GetMemoryData(ptr, out dt);
                    var data = DataConverterUInt16(dt);
                    StateAccessor.WriteRegister(r.Rt, data);
                }},
                { Opcode.LoadUpperImmediate, (r) => {
                    StateAccessor.WriteRegister(r.Rt, r.Immediate << 16);
                }},
                { Opcode.LoadWord, (r) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    UInt32 data;
                    StateAccessor.GetMemoryData(ptr, out data);
                    data = DataConverterUInt32(data);
                    StateAccessor.WriteRegister(r.Rt, data);
                }},
                { Opcode.BitwiseOrImmediate, (r) => {
                    var a = StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rt, a | r.Immediate);
                }},
                { Opcode.StoreByte, (r) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    var data = (byte)(StateAccessor.ReadRegister(r.Rt));
                    StateAccessor.SetMemoryData(ptr, data);
                }},
                { Opcode.SetOnLessThanImmediate, (r) => {
                    var a = (int)StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rt, a < (short)r.Immediate ? 1u : 0u);
                }},
                { Opcode.UnsignedSetOnLessThanImmediate, (r) => {
                    var a = StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rt, a < r.Immediate ? 1u : 0u);
                }},
                { Opcode.StoreHalfword, (r) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    var dt = (ushort)StateAccessor.ReadRegister(r.Rt);
                    dt = DataConverterUInt16(dt);
                    StateAccessor.SetMemoryData(ptr, (ushort)dt);
                }},
                { Opcode.StoreWord, (r) => {
                    var ptr = (uint)(((short)r.Immediate) + (int)StateAccessor.ReadRegister(r.Rs));
                    var data = DataConverterUInt32(StateAccessor.ReadRegister(r.Rt));
                    StateAccessor.SetMemoryData(ptr, data);
                }},
                { Opcode.BitwiseExclusiveOrImmediate, (r) => {
                    var a = StateAccessor.ReadRegister(r.Rs);
                    StateAccessor.WriteRegister(r.Rt, a ^ r.Immediate);
                }},
            };
        }

        private void populateCoProcessorExecutors()
        {
            CoProcessorExecutors = new Dictionary<CoProcessorFormat, Action<CoProcessorInstruction>>()
            {
                { CoProcessorFormat.MoveFromCoprocessor, (x) => {
                    StateAccessor.WriteRegister(x.Rt, StateAccessor.GetCoprocessorRegister(x.Rd));
                }},
                { CoProcessorFormat.MoveToCoprocessor, (x) => {
                    StateAccessor.SetCoprocessorRegister(x.Rt, StateAccessor.ReadRegister(x.Rd));
                }},
            };
        }

        public InstructionExecutor()
        {
            populateRTypeExecutors();
            populateJTypeExecutors();
            populateITypeExecutors();
            populateCoProcessorExecutors();
        }
        public InstructionExecutor(IStateAccessor stateAccessor, bool littleEndian = false) : this()
        {
            StateAccessor = stateAccessor;
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
        public void Execute(IInstruction instruction)
        {
            var jtype = instruction as JTypeInstruction;
            if (jtype != null)
                JTypeExecutors[jtype.JumpType](jtype);
            else
            {
                var itype = instruction as ITypeInstruction;
                if (itype != null)
                {
                    var cl = ITypeExecutors[itype.Opcode];
                    cl(itype);
                }
                else
                {
                    var rtype = instruction as RTypeInstruction;
                    if (rtype != null)
                        RTypeExecutors[rtype.Function](rtype);
                    else
                    {
                        var coins = instruction as CoProcessorInstruction;
                        if (coins != null)
                            CoProcessorExecutors[coins.Format](coins);
                    }
                }
            }
        }

    }
}

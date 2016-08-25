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

namespace MIPSI.UnPipelined
{
    public class StateAccessor : IStateAccessor
    {
        private State State { get; set; }
        public Cpu Cpu { get; set; }
        public StateAccessor(State state, Cpu cpu)
        {
            State = state;
            Cpu = cpu;
        }
        public void WriteRegister(uint address, uint value)
        {
            State.Registers[(int)address] = value;
        }

        public uint ReadRegister(uint address)
        {
            return State.Registers[(int)address];
        }

        public void SetPc(uint value)
        {
            //State.Pc = value;
            Cpu.Branch = (int)value;
        }

        public void SetHi(uint value)
        {
            State.Hi = value;
        }

        public void SetLo(uint value)
        {
            State.Lo = value;
        }

        public void SetEpc(uint value)
        {
            State.Epc = value;
        }

        public uint GetPc()
        {
            return State.Pc;
        }

        public uint GetHi()
        {
            return State.Hi;
        }

        public uint GetLo()
        {
            return State.Lo;
        }

        public uint GetEpc()
        {
            return State.Epc;
        }

        public void GetMemoryData(uint address, out byte data)
        {
            State.GetMemoryData(address, out data);
        }

        public void GetMemoryData(uint address, out ushort data)
        {
            State.GetMemoryData(address, out data);
        }

        public void GetMemoryData(uint address, out uint data)
        {
            State.GetMemoryData(address, out data);
        }

        public void SetMemoryData(uint address, byte data)
        {
            State.SetMemoryData(address, data);
        }

        public void SetMemoryData(uint address, ushort data)
        {
            State.SetMemoryData(address, data);
        }
        public void SetMemoryData(uint address, uint data)
        {
            State.SetMemoryData(address, data);
        }

        public void Skip()
        {
            Cpu.Skip = true;
        }


        public void SetCoprocessorRegister(uint register, uint value)
        {
            Cpu.Status = value;
        }

        public uint GetCoprocessorRegister(uint register)
        {
            if (register == 12)
                return Cpu.Status;
            return State.Epc;// | (Cpu.Status & 1);
        }
    }
}

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
    public class State : MIPSI.IState
    {
        public uint Pc { get; set; }
        public uint Hi { get; set; }
        public uint Lo { get; set; }
        public uint Epc { get; set; }
        public IList<uint> Registers { get; set; }
        private IDictionary<uint, IMemoryData> SpecialMemory { get; set; }
        public byte[] Memory { get; set; }
        public byte[] ExternalRam { get; set; }
        public void GetMemoryData(uint address, out byte data)
        {
            IMemoryData md;
            if (SpecialMemory.TryGetValue(address, out md))
            {
                data = (byte)md.Read();
                return;
            }
            var mem = Memory;
            if (address >= 0x10000000)
            {
                address -= 0x10000000;
                mem = ExternalRam;
            }
            data = mem[(int)address];
        }
        public void GetMemoryData(uint address, out UInt16 data)
        {
            IMemoryData md;
            if (SpecialMemory.TryGetValue(address, out md))
            {
                data = (UInt16)(md.Read() << 8);
                return;
            }
            var mem = Memory;
            if (address >= 0x10000000)
            {
                address -= 0x10000000;
                mem = ExternalRam;
            }
            data = BitConverter.ToUInt16(mem, (int)address);
        }
        public void GetMemoryData(uint address, out UInt32 data)
        {
            IMemoryData md;
            if (SpecialMemory.TryGetValue(address, out md))
            {
                data = (UInt32)(md.Read() << 24);
                return;
            }
            var mem = Memory;
            if (address >= 0x10000000)
            {
                address -= 0x10000000;
                mem = ExternalRam;
            }
            data = BitConverter.ToUInt32(mem, (int)address);
        }
        public void SetMemoryData(uint address, byte data)
        {
            IMemoryData md;
            if (SpecialMemory.TryGetValue(address, out md))
            {
                md.Write(((uint)data));
                return;
            }
            var mem = Memory;
            if (address >= 0x10000000)
            {
                address -= 0x10000000;
                mem = ExternalRam;
            }
            mem[(int)address] = data;
        }
        public void SetMemoryData(uint address, UInt16 data)
        {
            IMemoryData md;
            if (SpecialMemory.TryGetValue(address, out md))
            {
                md.Write(((uint)(data)) >> 8 );
                return;
            }
            var mem = Memory;
            if (address >= 0x10000000)
            {
                address -= 0x10000000;
                mem = ExternalRam;
            }
            var dt = BitConverter.GetBytes(data);
            Array.Copy(dt, 0, mem, (int)address, 2);
        }
        public void SetMemoryData(uint address, UInt32 data)
        {
            IMemoryData md;
            if (SpecialMemory.TryGetValue(address, out md))
            {
                md.Write(((uint)data) >> 24);
                return;
            }
            var addr = address;
            var mem = Memory;
            if (address >= 0x10000000)
            {
                address -= 0x10000000;
                mem = ExternalRam;
            }
            var dt = BitConverter.GetBytes(data);
            Array.Copy(dt, 0, mem, (int)address, 4);
        }
        public State()
        {
            Registers = new uint[32];
            ExternalRam = new byte[64000000];
            SpecialMemory = new Dictionary<uint, IMemoryData>();
            SpecialMemory.Add(0x20000000, new PrintMemoryData());
            SpecialMemory.Add(0x20000010, new MemoryData());
            SpecialMemory.Add(0x20000014, new IRQMaskP4MemoryData());
            SpecialMemory.Add(0x20000020, new IRQMemoryData());
            SpecialMemory.Add(0x20000070, new MemoryData());
            SpecialMemory.Add(0x20000080, new MemoryData());
            SpecialMemory.Add(0x20000090, new MemoryData());
            SpecialMemory.Add(0x200000a0, new MemoryData());
        }
    }
}

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
namespace MIPSI
{
    public interface IState
    {
        byte[] Memory { get; set; }
        uint Epc { get; set; }
        void GetMemoryData(uint address, out byte data);
        void GetMemoryData(uint address, out ushort data);
        void GetMemoryData(uint address, out uint data);
        uint Hi { get; set; }
        uint Lo { get; set; }
        uint Pc { get; set; }
        System.Collections.Generic.IList<uint> Registers { get; set; }
        void SetMemoryData(uint address, byte data);
        void SetMemoryData(uint address, ushort data);
        void SetMemoryData(uint address, uint data);
    }
}

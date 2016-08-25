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
using ELFSharp.ELF;
using ELFSharp.ELF.Sections;

namespace MIPSI
{
    public class ProgramLoader
    {
        public static Program ReadProgram(ELF<uint> elf, bool generateMemory=true)
        {
            var state = new State();
            if(generateMemory)
                state.Memory = new byte[1000000];
            var littleEndian = elf.Endianess == Endianess.LittleEndian;
            var sections = new Dictionary<string, ISection>();
            foreach (var x in elf.Sections)
                sections.Add(x.Name, x);
            var text = sections[".text"] as ProgBitsSection<uint>;

            var textReader = new BinaryReader(new MemoryStream(text.GetContents()));
            var reader = new InstructionsReader(littleEndian);
            var instructions = reader.ReadInstructions(textReader, littleEndian, text.LoadAddress);
            if (generateMemory)
                Array.Copy(text.GetContents(), 0, state.Memory, text.LoadAddress, text.Size);
            
            var names = new string[]{ ".rodata", ".data", ".sdata" };
            if (generateMemory)
            {
                foreach (var n in names)
                {
                    ISection sect;
                    if (sections.TryGetValue(n, out sect))
                    {
                        var section = sect as ProgBitsSection<uint>;
                        Array.Copy(section.GetContents(), 0, state.Memory, section.LoadAddress, section.Size);
                    }
                }
            }
            return new Program() { EntryPoint = elf.EntryPoint, Instructions = instructions, State = state, LittleEndian = littleEndian, TextSectionAddress = text.LoadAddress };
        }
    }
}

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
using System.Threading;

namespace MIPSI.UnPipelined
{
    public class Cpu
    {
        public Program Program { get; set; }
        public int Branch { get; set; }
        public bool Skip { get; set; }
        public uint Status { get; set; }
        public void Run()
        {
            var memory = new byte[Program.State.Memory.Length];
            Program.State.Memory.CopyTo(memory, 0);
            var state = new State() { Memory = memory };
            var stateAccessor = new StateAccessor(state, this);
            state.Pc = Program.EntryPoint >> 2;
            //var instructionExecutor = new InstructionExecutor(stateAccessor, Program.LittleEndian);

            var cnt = 0;
            var hs = new Dictionary<uint, uint>();
            int count = 0;
            while (true)
            {
                var index = state.Pc >> 2;
                var cind = index % 1024;
                if (!hs.ContainsKey(cind))
                {
                    cnt++;
                    hs.Add(cind, index);
                }
                else
                {
                    var t = hs[cind];
                    if (t != index)
                    {
                        hs[cind] = index;
                        cnt++;
                    }
                }


                if (Branch >= 0)
                {
                    state.Pc = (uint)Branch;
                    //state.Epc = state.Pc | 2;
                    Branch = -1;
                }
                else
                {
                    state.Pc += 4;
                    //state.Epc = state.Pc;
                }
                if ((state.Pc & 3) != 0)
                {
                }

                if (Skip)
                {
                    Skip = false;
                    continue;
                }

                var instruction = Program.Instructions[(int)index];

                instruction.Execute(stateAccessor);

                //instructionExecutor.Execute(instruction);
                if (state.Pc - 4 == 0x38)
                {
                }
                if (state.Pc - 4 == 0x914)
                {
                }
                count++;
                //Thread.Sleep(1);
            }
        }
    }
}

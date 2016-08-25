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
using ELFSharp.ELF;
using GraphVizWrapper;
using GraphVizWrapper.Queries;
using GraphVizWrapper.Commands;
using System.IO;
using System.Threading;

namespace WCET
{
    class Program
    {
        static void Main(string[] args)
        {
            var thread = new Thread(new ThreadStart(test), 100000000);
            thread.Start();
            //test();
            while (true) ;


        }

        private static void test()
        {


            /*var c = new Class1(0);
            var list = new List<IInstruction>
            {
                new RTypeInstruction() { Function = RTypeFunction.Add, Rd = 30}, 
                new ITypeInstruction() { Opcode = Opcode.AddImmediate, Rt = 30}, 
                new RTypeInstruction() { Function = RTypeFunction.JumpRegister, Rs = 31},

            };
            c.Parse(list, 0, 8);*/

            var program = ProgramLoader.ReadProgram(ELFReader.Load<uint>("test.axf"), false);
            var c2 = new SubCodeReader(program.TextSectionAddress);
            var startAddress = 0x100004fcu;
            var endAddress = 0x100005e4u;
            var sc = c2.Parse(program.Instructions, startAddress, endAddress);
            var gb = new GraphBuilder();
            var g = gb.Build(sc);
            var gv = new GraphViz();
            Console.WriteLine("Plotting Graph");
            //gv.CreateGraph(g).Save("graph.svg");
            //gv.CreateSVGGraph(g, "graph");
            var gl = new GraphLinearizer();
            //var c = gl.FindCycles(g);
            //var pth = new HashSet<Node<IList<IInstruction>>>();
            //gl.SearchRec(c.ElementAt(7).Item2, c.ElementAt(7).Item1, new HashSet<Node<IList<IInstruction>>>(), pth);
            Console.WriteLine("Linearizing Graph");
            gl.Linearize(g);
            Console.WriteLine("Plotting DAG");
            //gv.CreateGraph(g).Save("DAG.svg");
            //gv.CreateSVGGraph(g, "DAG");
            Console.WriteLine("Computing WCET");


            var stateTransition = new StateTransition();
            var graphHeuristic = new GraphHeuristic();
            var initialState = new State();
            initialState.Cache = new Cache(5);
            initialState.Node = g;
            graphHeuristic.ComputeHeuristic(g);
            var initialCost = stateTransition.ComputeCost(initialState);
            var aStar = new AStar.AStarPrime();
            AStar.Node aStarResult;
            var goalState = new State() { Node = gl.FindNodeByInstAddress(g, endAddress) };
            aStar.Search(stateTransition, initialState, goalState, Heuristic.ComputeHeuristic, out aStarResult, initialCost);
            var cwcet = new CachelessWCET();
            var cWCET = cwcet.CalculateWCET(g);
        }

    }
}

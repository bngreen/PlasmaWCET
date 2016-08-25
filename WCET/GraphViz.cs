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
using GraphVizWrapper;
using GraphVizWrapper.Queries;
using GraphVizWrapper.Commands;
using MIPSI;
using System.IO;
using System.Drawing;

namespace WCET
{
    using InstNode = Node<IList<IInstruction>>;
    public class GraphViz
    {
        private GraphGeneration wrapper { get; set; }
        public GraphViz()
        {
            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);
            wrapper = new GraphGeneration(  getStartProcessQuery,
                                            getProcessStartInfoQuery,
                                            registerLayoutPluginCommand);
            
        }
        public byte[] Example()
        {
            return wrapper.GenerateGraph("digraph{a -> b; b -> c; c -> a;}", Enums.GraphReturnType.Png);
        }

        private int IdC {get;set;}

        class GraphVizNode
        {
            public int Id { get; set; }
            public string Label { get; set; }
        }

        private void ExtractGVNodes(InstNode root, Dictionary<InstNode, GraphVizNode> nodeDict)
        {
            GraphTraversal.PerformTraversal<InstNode>(root, (node, fringeAdder) =>
            {
                if (!nodeDict.ContainsKey(node))
                {
                    var ms = new MemoryStream();
                    var sw = new StreamWriter(ms);
                    PrettyPrinter.PrettyPrint(node.Content, sw);
                    sw.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    var data = new StreamReader(ms).ReadToEnd();
                    nodeDict.Add(node, new GraphVizNode() { Id = IdC++, Label = data });
                    if (node.Left != null)
                        fringeAdder(node.Left);
                    if (node.Right != null)
                        fringeAdder(node.Right);
                }
            });
        }

        private void ExtractGVNodes2(Node<IList<IInstruction>> node, Dictionary<Node<IList<IInstruction>>, GraphVizNode> nodeDict)
        {
            if (!nodeDict.ContainsKey(node))
            {
                var ms = new MemoryStream();
                var sw = new StreamWriter(ms);
                PrettyPrinter.PrettyPrint(node.Content, sw);
                sw.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                var data = new StreamReader(ms).ReadToEnd();
                nodeDict.Add(node, new GraphVizNode() { Id = IdC++, Label = data });
                if (node.Left != null)
                    ExtractGVNodes(node.Left, nodeDict);
                if (node.Right != null)
                    ExtractGVNodes(node.Right, nodeDict);
            }
        }
        public void CreatePDFGraph(Node<IList<IInstruction>> root, string name)
        {
            var nodes = new Dictionary<Node<IList<IInstruction>>, GraphVizNode>();
            ExtractGVNodes(root, nodes);
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("digraph {");
            foreach (var x in nodes)
                builder.AppendLine(String.Format("node{0} [fontname = \"terminal\" label=\"{1}\" shape=box]", x.Value.Id, x.Value.Label.Replace("\r\n", "\\l")));

            GenerateGraphLinks(root, nodes, builder);

            builder.AppendLine("}");
            var dot = builder.ToString();
            using (var st = new BinaryWriter(File.Create(name + ".pdf")))
                st.Write(wrapper.GenerateGraph(dot, Enums.GraphReturnType.Pdf));
        }

        public void CreateSVGGraph(Node<IList<IInstruction>> root, string name)
        {
            var nodes = new Dictionary<Node<IList<IInstruction>>, GraphVizNode>();
            ExtractGVNodes(root, nodes);
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("digraph {");
            foreach (var x in nodes)
                builder.AppendLine(String.Format("node{0} [fontname = \"terminal\" label=\"{1}\" shape=box]", x.Value.Id, x.Value.Label.Replace("\r\n", "\\l")));

            GenerateGraphLinks(root, nodes, builder);

            builder.AppendLine("}");
            var dot = builder.ToString();
            using (var st = new BinaryWriter(File.Create(name + ".svg")))
                st.Write(wrapper.GenerateGraph(dot, Enums.GraphReturnType.Svg));
        }



        public Image CreateGraph(Node<IList<IInstruction>> root)
        {
            var nodes = new Dictionary<Node<IList<IInstruction>>, GraphVizNode>();
            ExtractGVNodes(root, nodes);
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("digraph {");
            foreach (var x in nodes)
                builder.AppendLine(String.Format("node{0} [fontname = \"terminal\" label=\"{1}\" shape=box]", x.Value.Id, x.Value.Label.Replace("\r\n", "\\l")));
            
            GenerateGraphLinks(root, nodes, builder);

            builder.AppendLine("}");
            var dot = builder.ToString();
            var str = new StreamWriter("test.txt");
            str.WriteLine(dot);
            str.Close();
            var graphMS = new MemoryStream(wrapper.GenerateGraph(dot, Enums.GraphReturnType.Png));
            return Image.FromStream(graphMS);
        }
        private void GenerateGraphLinks(Node<IList<IInstruction>> root, Dictionary<InstNode, GraphVizNode> graphVizNodes, StringBuilder builder)
        {
            var visitedNodes = new HashSet<InstNode>();
            GraphTraversal.PerformTraversal<InstNode>(root, (node, fringeAdder) =>
            {
                if (visitedNodes.Contains(node))
                    return;
                visitedNodes.Add(node);
                if (node.Left != null)
                {
                    builder.AppendLine(String.Format("node{0}->node{1}", graphVizNodes[node].Id, graphVizNodes[node.Left].Id));
                    fringeAdder(node.Left);
                }
                if (node.Right != null)
                {
                    builder.AppendLine(String.Format("node{0}->node{1}", graphVizNodes[node].Id, graphVizNodes[node.Right].Id));
                    fringeAdder(node.Right);
                }
            });
        }
        private void GenerateGraphLinks2(Node<IList<IInstruction>> node,Dictionary<Node<IList<IInstruction>>, GraphVizNode> graphVizNodes, Dictionary<Node<IList<IInstruction>>, int> visitedNodes, StringBuilder builder)
        {
            if (visitedNodes.ContainsKey(node))
                return;
            visitedNodes.Add(node, 0);
            if(node.Left != null)
            {
                builder.AppendLine(String.Format("node{0}->node{1}", graphVizNodes[node].Id, graphVizNodes[node.Left].Id));
                GenerateGraphLinks2(node.Left, graphVizNodes, visitedNodes, builder);
            }
            if (node.Right != null)
            {
                builder.AppendLine(String.Format("node{0}->node{1}", graphVizNodes[node].Id, graphVizNodes[node.Right].Id));
                GenerateGraphLinks2(node.Right, graphVizNodes, visitedNodes, builder);
            }
        }

    }
}

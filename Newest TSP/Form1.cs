
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Newest_TSP
{
    public partial class Form1 : Form
    {
        List<Point> nodes = new List<Point>();
        List<Line> lines = new List<Line>();
        List<Line> pathLines = new List<Line>();
        Random random = new Random();
        Graphics graph;
        int amountOfNodes;

        public Form1()
        {
            InitializeComponent();
            Width = 800;
            Height = 600;
            Shown += Form1_Shown;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Setup();
            GenerateNodesAndLines();
            DrawNodesAndLines(lines);
            Console.WriteLine();
            WriteWithLetters(lines);
            GeneratePath();
            Console.WriteLine("actual path");
            WriteWithLetters(pathLines);
            DrawNodesAndLines(pathLines);
            OptimizeWaddup();
            Console.ReadLine();
            DrawNodesAndLines(pathLines);
            if (Console.ReadLine() == "")
                this.Close();
        }

        public void Setup()
        {
            graph = CreateGraphics();
            Console.WriteLine("Enter amount of nodes");
            amountOfNodes = Convert.ToInt32(Console.ReadLine());
        }

        public void GenerateNodesAndLines()
        {
            for (int i = 0; i < amountOfNodes; i++)
            {
                nodes.Add(new Point(random.Next(10, 470), random.Next(10, 470)));
            }/**/

            /*nodes.Add(new Point(10, 10));
            nodes.Add(new Point(30, 60));
            nodes.Add(new Point(50, 20));
            nodes.Add(new Point(60, 40));
            nodes.Add(new Point(70, 70));
            nodes.Add(new Point(80, 10));
            nodes.Add(new Point(50, 80));
            nodes.Add(new Point(20, 90));  /**/

            for (int a = 0; a < amountOfNodes - 1; a++)
            {
                for (int b = a + 1; b < amountOfNodes; b++)
                {
                    lines.Add(new Line(a, b, nodes[a], nodes[b]));
                }
            }
        }

        public void GeneratePath()
        {
            List<Line> foundLines = new List<Line>();
            List<Line> selectionLines = new List<Line>(lines);

            int nodeToFind = 0;

            while (pathLines.Count < amountOfNodes - 1)
            {
                foundLines = selectionLines.FindAll(i => i.node1 == nodeToFind || i.node2 == nodeToFind);
                foundLines.Sort((a, b) => { return a.distanceSquared.CompareTo(b.distanceSquared); });

                Line selected = foundLines[0];
                pathLines.Add(selected);

                nodeToFind = (selected.node1 == nodeToFind) ? selected.node2 : selected.node1;

                for (int i = 0; i < foundLines.Count; i++)
                {
                    selectionLines.Remove(foundLines[i]);
                }
            }
            pathLines.Add(lines.Find(i => i.node1 == 0 && i.node2 == nodeToFind));

        }

        public void OptimizeWaddup()
        {
            List<Line> possibles = new List<Line>(pathLines);
            int nodeA1, nodeA2, nodeB1, nodeB2;
            bool hasChanged = false;
            List<Line> testPath;
            Line testLineA;
            Line testLineB;

            do
            {
                hasChanged = false;
                for (int i = 0; i < pathLines.Count - 1; i++)
                {
                    for (int a = i + 1; a < pathLines.Count; a++)
                    {
                        nodeA1 = pathLines[i].node1;
                        nodeA2 = pathLines[i].node2;
                        nodeB1 = pathLines[a].node1;
                        nodeB2 = pathLines[a].node2;

                        if (nodeA1 != nodeA2 && nodeA1 != nodeB1 && nodeA1 != nodeB2 && nodeA2 != nodeB1 && nodeA2 != nodeB2 && nodeB1 != nodeB2)
                        {

                            testLineA = lines.Find(l => l.node1 == Math.Min(nodeA1, nodeB1) && l.node2 == Math.Max(nodeA1, nodeB1));
                            testLineB = lines.Find(l => l.node1 == Math.Min(nodeA2, nodeB2) && l.node2 == Math.Max(nodeA2, nodeB2));

                            possibles[i] = testLineA;
                            possibles[a] = testLineB;

                            testPath = new List<Line>(possibles);
                            if (FullPath(testPath))
                            {
                                if (pathLines[i].distanceSquared + pathLines[a].distanceSquared > possibles[i].distanceSquared + possibles[a].distanceSquared)
                                {
                                    pathLines[i] = testLineA;
                                    pathLines[a] = testLineB;
                                    hasChanged = true;
                                }
                                continue;
                            }

                            testLineA = lines.Find(l => l.node1 == ((nodeA1 < nodeB2) ? nodeA1 : nodeB2) && l.node2 == ((nodeA1 < nodeB2) ? nodeB2 : nodeA1));
                            testLineB = lines.Find(l => l.node1 == ((nodeA2 > nodeB1) ? nodeB1 : nodeA2) && l.node2 == ((nodeA2 < nodeB1) ? nodeB1 : nodeA2));

                            possibles[i] = testLineA;
                            possibles[a] = testLineB;
                            testPath = new List<Line>(possibles);
                            if (FullPath(testPath))
                            {
                                if (pathLines[i].distanceSquared + pathLines[a].distanceSquared > possibles[i].distanceSquared + possibles[a].distanceSquared)
                                {
                                    pathLines[i] = testLineA;
                                    pathLines[a] = testLineB;
                                    hasChanged = true;
                                    continue;
                                }
                            }

                            possibles = new List<Line>(pathLines);
                        }
                    }
                }
            }
            while (hasChanged);
        }

        public bool FullPath(List<Line> tryPath)
        {
            int nodeToFind = 0;
            int counter = 0;
            List<Line> lol = new List<Line>(tryPath);

            do
            {
                Line curLine = tryPath.Find(lne => lne.node1 == nodeToFind || lne.node2 == nodeToFind);
                if (curLine == null)
                    break;
                nodeToFind = (curLine.node1 != nodeToFind) ? curLine.node1 : curLine.node2;
                tryPath.Remove(curLine);
                counter++;
            }
            while (nodeToFind != 0);

            return (counter == amountOfNodes);
        }

        public void DrawNodesAndLines(List<Line> gg)
        {
            graph.Clear(Color.WhiteSmoke);

            for (int c = 0; c < nodes.Count; c++)
            {
                graph.FillRectangle(Brushes.Red, nodes[c].X - 1, nodes[c].Y - 1, 3, 3);
                //graph.DrawString(/*Convert.ToChar(*/c.ToString()/* + 97).ToString()*/, new Font("Arial", 10), Brushes.Black, nodes[c].X, nodes[c].Y);
            }
            if (gg != null && gg.Count != 0)
            {
                foreach (Line line in gg)
                {
                    line.Draw(graph);
                }
            }
        }

        void Write(List<Line> linesToWrite)
        {
            for (int b = 0; b < linesToWrite.Count; b++)
            {
                Console.WriteLine(linesToWrite[b].node1 + "" + linesToWrite[b].node2 + " " + linesToWrite[b].distanceSquared);
            }
            Console.WriteLine();
            Console.ReadLine();
        }

        void WriteWithLetters(List<Line> linesToWrite)
        {
            for (int b = 0; b < linesToWrite.Count; b++)
            {
                Console.WriteLine(Convert.ToChar(linesToWrite[b].node1 + 97) + "" + Convert.ToChar(linesToWrite[b].node2 + 97) + " " + linesToWrite[b].distanceSquared);
            }
            Console.WriteLine();
            Console.ReadLine();
        }
    }

    public class Line
    {
        public int node1, node2, distanceSquared;
        public Point start, end;

        public Line(int setNode1Index, int setNode2Index, Point setStart, Point setEnd)
        {
            node1 = setNode1Index;
            node2 = setNode2Index;
            start = setStart;
            end = setEnd;
            distanceSquared = (int)Math.Sqrt((int)Math.Pow((end.X - start.X), 2) + (int)Math.Pow((end.Y - start.Y), 2));
        }

        public void Draw(Graphics g)
        {
            g.DrawLine(Pens.Black, start, end);
        }
    }
}
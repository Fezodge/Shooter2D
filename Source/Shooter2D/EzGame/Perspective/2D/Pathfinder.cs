using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace EzGame.Perspective.Planar
{
    public class Pathfinder
    {
        public enum Algorithms { AStar }
        public Node[,] Nodes;

        private Dictionary<Point, Dictionary<Point, Path>> Paths;

        public Pathfinder(int Width, int Height) { Nodes = new Node[Width, Height]; SetAllNodes(true); }

        public void SetNode(Point Position, Node Node) { if ((Position.X < 0) || (Position.Y < 0) || (Position.X >= Nodes.GetLength(0)) || (Position.Y >= Nodes.GetLength(1))) return; Nodes[Position.X, Position.Y] = Node; }
        public void SetAllNodes(bool Walkable) { for (int X = 0; X < Nodes.GetLength(0); X++) { for (int Y = 0; Y < Nodes.GetLength(1); Y++) { Nodes[X, Y] = new Node(Walkable); } } }
        public bool InBounds(Point Node) { if ((Node.X >= 0) && (Node.Y >= 0) && (Node.X < Nodes.GetLength(0)) && (Node.Y < Nodes.GetLength(1))) return true; else return false; }

        private int Heuristic(Point S, Point E) { return (int)(((MathHelper.Max(S.X, E.X) - MathHelper.Min(S.X, E.X)) + (MathHelper.Max(S.Y, E.Y) - MathHelper.Min(S.Y, E.Y))) * 10); }
        private Node GetNode(int X, int Y) { return Nodes[X, Y]; }
        private Node GetNode(Point C) { return Nodes[C.X, C.Y]; }

        public Path Find(Point Start, Point End, bool CutCorners, bool Memorize, Algorithms Algorithm = Algorithms.AStar)
        {
            if (Memorize)
            {
                if (Paths == null) Paths = new Dictionary<Point, Dictionary<Point, Path>>();
                if (Paths.ContainsKey(Start) && Paths[Start].ContainsKey(End)) return Paths[Start][End].Clone;
                else if (Paths.ContainsKey(End) && Paths[End].ContainsKey(Start)) return Reverse(Paths[End][Start].Clone, End);
            }
            switch (Algorithm)
            {
                default:
                case Algorithms.AStar:
                    if ((Start == End) || !InBounds(Start) || !InBounds(End)) return null;
                    Point C = Start; List<Point> Open = new List<Point>(), Closed = new List<Point>();
                    Node CN = GetNode(C);
                    while (!Open.Contains(new Point(End.X, End.Y)))
                    {
                        CN.Position = C; Open.Remove(C); Closed.Add(C);
                        foreach (Point N in Neighbours(C, CutCorners, Algorithm))
                        {
                            Node NN = GetNode(N);
                            bool NOpen = Open.Contains(N), NClosed = Closed.Contains(N);
                            if (NClosed) continue;
                            NN.SetParent(C);
                            int G = (CN.G + (int)CN.Distance(N));
                            NN.Record((G + Heuristic(N, End)), G);
                            if (!NOpen && !NClosed) Open.Add(N);
                            else if (NN.GEx < NN.G) { NN.SetParent(NN.ParentEx); NN.Record(NN.FEx, NN.GEx); }
                        }
                        if (Open.Count < 1) return null;
                        else { int LowF = int.MaxValue; foreach (Point N in Open) { Node NN = GetNode(N); if (NN.F < LowF) { LowF = NN.F; C = N; CN = GetNode(C); } } }
                    }
                    Path Path = new Path(); C = End; CN = GetNode(C);
                    while (!Path.Contains(Start)) { Path.Add(CN.Parent); C = CN.Parent; CN = GetNode(C); }
                    Path.Reverse(); Path.RemoveAt(0); Path.Add(End);
                    if (Memorize) this.Memorize(Start, End, Path);
                    return Path.Clone;
            }
        }

        private static Point[] StraightSides = new Point[] { new Point(0, 1), new Point(0, -1), new Point(1, 0), new Point(-1, 0) };
        private static Point[] DiagnolSides = new Point[] { new Point(1, 1), new Point(1, -1), new Point(-1, 1), new Point(-1, -1), new Point(0, 1), new Point(0, -1), new Point(1, 0), new Point(-1, 0) };
        private List<Point> Neighbours(Point Point, bool CutCorners, Algorithms Algorithm = Algorithms.AStar)
        {
            List<Point> Nodes = new List<Point>();
            switch (Algorithm)
            {
                default:
                case Algorithms.AStar:
                    foreach (Point S in (CutCorners ? DiagnolSides : StraightSides))
                    {
                        Point CS = new Point((Point.X + S.X), (Point.Y + S.Y));
                        if (!IsBlocked(CS))
                        {
                            if (((S.X != 0) && (S.Y != 0)) && (!IsWalkable(new Point(CS.X, Point.Y)) || !IsWalkable(new Point(Point.X, CS.Y)))) continue;
                            Nodes.Add(CS);
                        }
                    }
                    return Nodes;
            }
        }

        public bool IsBlocked(Point P) { if (!InBounds(P) || !IsWalkable(P)) return true; else return false; }
        public bool IsWalkable(Point P) { if (Nodes[P.X, P.Y].Walkable) return true; else return false; }
        public void Memorize(Point Start, Point End, Path Path)
        {
            if (!Paths.ContainsKey(Start)) { Paths.Add(Start, new Dictionary<Point, Path>()); Paths[Start].Add(End, Path); }
            else if (Paths.ContainsKey(Start) && !Paths[Start].ContainsKey(End)) Paths[Start].Add(End, Path);
        }
        public Path Reverse(Path P, Point S) { P = P.ReversePath(); P.RemoveAt(0); P.Add(S); return P; }

        public List<Point> GetWallsAround(Point P)
        {
            List<Point> PW = new List<Point>();
            if (((P.X - 1) >= 0) && (!Nodes[(P.X - 1), P.Y].Walkable)) PW.Add(new Point((P.X - 1), P.Y));
            if (((P.X + 1) <= Nodes.GetLength(0)) && (!Nodes[(P.X + 1), P.Y].Walkable)) PW.Add(new Point((P.X + 1), P.Y));
            if (((P.Y - 1) >= 0) && (!Nodes[P.X, (P.Y - 1)].Walkable)) PW.Add(new Point(P.X, (P.Y - 1)));
            if (((P.Y + 1) <= Nodes.GetLength(1)) && (!Nodes[P.X, (P.Y + 1)].Walkable)) PW.Add(new Point(P.X, (P.Y + 1)));
            return PW;
        }

        public class Node
        {
            public Point Parent, ParentEx, Position;
            public bool Walkable;
            public int F, G, FEx, GEx;

            public Node(bool Walkable) { this.Walkable = Walkable; }

            public void SetParent(Point P) { ParentEx = Parent; Parent = P; }
            public void Record(int F, int G) { FEx = this.F; GEx = this.G; this.F = F; this.G = G; }
            public int Distance(Point T) { return (int)(MathHelper.Clamp(((MathHelper.Max(Position.X, T.X) - MathHelper.Min(Position.X, T.X)) + (MathHelper.Max(Position.Y, T.Y) - MathHelper.Min(Position.Y, T.Y))), 1f, 1.4f) * 10); }
        }

        public class Path : List<Point>
        {
            public Path Clone { get { Path PC = new Path(); foreach (Point P in this) PC.Add(P); return PC; } }
            public Path ReversePath() { List<Point> PC = this; Reverse(); Path PR = new Path(); PR.AddRange(PC); return PR; }
        }
    }
}
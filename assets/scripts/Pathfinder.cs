using Godot;
using System.Collections.Generic;

namespace LabyrinthDeck
{
    public class Pathfinder
    {
        public class Tile : Object, System.IComparable<Tile>
        {
            public Maze.TilePos Pos;
            public double Priority;

            public Tile(Maze.TilePos pos, double priority)
            {
                Pos = pos;
                Priority = priority;
            }

            public int CompareTo(Tile other)
            {
                if (Priority < other.Priority) return -1;
                else if (Priority > other.Priority) return 1;
                else return 0;
            }
        }

        private int[,] _map;
        private int _xTiles;
        private int _yTiles;

        public Pathfinder(int [,] map, int xTiles, int yTiles)
        {
            _map = map;
            _xTiles = xTiles;
            _yTiles = yTiles;
        }

        public Dictionary<Maze.TilePos, Maze.TilePos> FindPath(Maze.TilePos start, Maze.TilePos end)
        {
            PriorityQueue<Tile> frontier = new PriorityQueue<Tile>();
            frontier.Put(new Tile(start, 0));
            Dictionary<Maze.TilePos, Maze.TilePos> cameFrom = new Dictionary<Maze.TilePos, Maze.TilePos>();
            cameFrom.Add(start, null);
            Dictionary<Maze.TilePos, double> costSoFar = new Dictionary<Maze.TilePos, double>();
            costSoFar.Add(start, 0);

            while (frontier.Count > 0)
            {
                Tile current = frontier.Get();
                if (current.Pos.X == end.X && current.Pos.Y == end.Y)
                {
                    break;
                }

                List<Maze.TilePos> neighbors = GetNeighbors(current.Pos);
                if (neighbors.Count > 0)
                {
                    foreach(var next in neighbors)
                    {
                        double newCost = costSoFar[current.Pos] + 1;
                        if (!costSoFar.ContainsKey(next) ||
                            newCost < costSoFar[next])
                        {
                            if(!costSoFar.ContainsKey(next))
                            {
                                costSoFar.Add(next, newCost);
                            }
                            else
                            {
                                costSoFar[next] = newCost;
                            }

                            double priority = newCost + Manhattan(end, next);
                            frontier.Put(new Tile(next, priority));
                            cameFrom[next] = current.Pos;
                        }
                    }
                }
            }

            return cameFrom;
        }

        private List<Maze.TilePos> GetNeighbors(Maze.TilePos tilePos)
        {
            List<Maze.TilePos> neighbors = new List<Maze.TilePos>();

            // UP
            if (tilePos.Y > 0 && _map[tilePos.X, tilePos.Y - 1] == 0)
            {
                neighbors.Add(new Maze.TilePos(tilePos.X, tilePos.Y - 1));
            }

            // BOTTOM
            if (tilePos.Y < _yTiles - 1 &&_map[tilePos.X, tilePos.Y + 1] == 0)
            {
                neighbors.Add(new Maze.TilePos(tilePos.X, tilePos.Y + 1));
            }

            // LEFT
            if (tilePos.X > 0 && _map[tilePos.X - 1, tilePos.Y] == 0)
            {
                neighbors.Add(new Maze.TilePos(tilePos.X - 1, tilePos.Y));
            }

            // RIGHT
            if (tilePos.X < _xTiles - 1 && _map[tilePos.X + 1, tilePos.Y] == 0)
            {
                neighbors.Add(new Maze.TilePos(tilePos.X + 1, tilePos.Y));
            }

            return neighbors;
        }

        private double Manhattan(Maze.TilePos a, Maze.TilePos b)
        {
            return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
        }
    }
}
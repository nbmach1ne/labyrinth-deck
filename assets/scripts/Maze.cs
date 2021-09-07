using Godot;
using System.Collections.Generic;

namespace LabyrinthDeck
{
    public class Maze : Spatial
    {
        [Export]
        private int[] _stepsProbs;
        [Export]
        private int _xTiles = 15;
        [Export]
        private int _yTiles = 19;  

        public enum Direction
        {
            NONE,
            UP,
            DOWN,
            LEFT,
            RIGHT
        }

        public class Movement : Object
        {
            public Direction Direction;
            public int Steps;

            public Movement(Direction direction, int steps)
            {
                Direction = direction;
                Steps = steps;
            }

            public List<Direction> GetStepsList()
            {
                List<Direction> steps = new List<Direction>();
                for (int i = 0; i < Steps; i++)
                {
                    steps.Add(Direction);
                }
                return steps;
            }
        }

        public class TilePos : Object
        {
            public int X;
            public int Y;

            public TilePos(int x, int y)
            {
                X = x;
                Y = y;
            }

            public override string ToString()
            {
                return "(" + X + ", " + Y + ")";
            }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                
                TilePos otherTile = (TilePos)obj;
                if (X == otherTile.X && Y == otherTile.Y)
                {
                    return true;
                }

                return false;
            }
            
            public override int GetHashCode()
            {
                return X * 10 + Y;
            }

            public Vector3 ToWorldPos()
            {
                return new Vector3(X * Game.TILE_SIZE, 
                                   0f,
                                   Y * Game.TILE_SIZE);
            }
        }

        private int[,] _mazeData = {{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ,1 ,1 ,1 ,1 ,1 },
                                    { 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1 },
                                    { 1, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1 },
                                    { 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 0, 1 },
                                    { 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1 },
                                    { 1, 0, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 0, 1 },
                                    { 1, 0, 1, 0, 0, 0, 1, 0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 0, 1 },
                                    { 0, 0, 1, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 1 },
                                    { 1, 0, 1, 0, 0, 0, 1, 0, 1, 1, 1, 0, 1, 0, 0, 0, 1, 0, 1 },
                                    { 1, 0, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 0, 1 },
                                    { 1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1 },
                                    { 1, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 1, 0, 1, 1, 0, 1 },
                                    { 1, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1 },
                                    { 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1 },
                                    { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ,1 ,1 ,1 ,1 ,1 }};

              
        private TilePos _goalPos = new TilePos(7, 1);
        public TilePos GoalPos
        {
            get { return _goalPos; }
        }

        private Pathfinder _pathfinder;
        private RandomNumberGenerator _rng;

        public override void _Ready()
        {
            _pathfinder = new Pathfinder(_mazeData, _xTiles, _yTiles);

            _rng = new RandomNumberGenerator();
            _rng.Randomize();
        }

        public List<Movement> GetPossibleMovements(TilePos startTile)
        {
            List<Movement> movements = new List<Movement>();
            int maxSteps = 5;

            bool foundWall = false;
            int steps = 1;
            // UP
            while (!foundWall && steps <= maxSteps)
            {
                if (startTile.Y - steps < 0 ||
                    _mazeData[startTile.X, startTile.Y - steps] == 1)
                {
                    foundWall = true;
                    break;
                }

                if (_rng.RandiRange(0, 100) <= _stepsProbs[steps - 1])
                {
                    movements.Add(new Movement(Direction.UP, steps));
                }
                steps++;
            }

            foundWall = false;
            steps = 1;
            // DOWN
            while (!foundWall && steps <= maxSteps)
            {
                if (startTile.Y + steps > _yTiles - 1 ||
                    _mazeData[startTile.X, startTile.Y + steps] == 1)
                {
                    foundWall = true;
                    break;
                }

                if (_rng.RandiRange(0, 100) <= _stepsProbs[steps - 1])
                {
                    movements.Add(new Movement(Direction.DOWN, steps));
                }
                steps++;
            }

            foundWall = false;
            steps = 1;
            // LEFT
            while (!foundWall && steps <= maxSteps)
            {
                if (startTile.X - steps < 0 ||
                    _mazeData[startTile.X - steps, startTile.Y] == 1)
                {
                    foundWall = true;
                    break;
                }

                if (_rng.RandiRange(0, 100) <= _stepsProbs[steps - 1])
                {
                    movements.Add(new Movement(Direction.LEFT, steps));
                }
                steps++;
            }

            foundWall = false;
            steps = 1;
            // RIGTH
            while (!foundWall && steps <= maxSteps)
            {
                if (startTile.X + steps > _xTiles - 1 ||
                    _mazeData[startTile.X + steps, startTile.Y] == 1)
                {
                    foundWall = true;
                    break;
                }

                if (_rng.RandiRange(0, 100) <= _stepsProbs[steps - 1])
                {
                    movements.Add(new Movement(Direction.RIGHT, steps));
                }
                steps++;
            }

            return movements;
        }

        public List<Direction> GetSteps(TilePos start, TilePos end)
        {
            List<Direction> steps = new List<Direction>();

            Dictionary<TilePos, TilePos> path = _pathfinder.FindPath(start, end);

            TilePos current = end;
            while (!current.Equals(start))
            {
                TilePos camesFrom = path[current];
                Direction movementDirection = GetMovementDirection(camesFrom, current);
                steps.Add(movementDirection);
                current = camesFrom;
            }

            steps.Reverse();

            // for (int i = 0; i < movements.Count; i++)
            // {
            //     GD.Print("Move " + movements[i].Direction.ToString() + " " + movements[i].Steps + " steps");
            // }

            return steps;;
        }

        private Direction GetMovementDirection(TilePos origin, TilePos target)
        {
            // UP
            if (origin.Y < target.Y) return Direction.DOWN;
            // DOWN
            if (origin.Y > target.Y) return Direction.UP;
            // LEFT
            if (origin.X < target.X) return Direction.RIGHT;
            // RIGHT
            if (origin.X > target.X) return Direction.LEFT;

            return Direction.NONE;
        }
    }
}

using Godot;
using System.Collections.Generic;

namespace LabyrinthDeck
{
    public class EnemiesControl : Node
    {
        [Signal]
        delegate void MovementFinished();

        private Maze _maze;
        private List<Enemy> _enemies;
        private int _enemiesToMove;

        private RandomNumberGenerator _rng;

        public override void _Ready()
        {
            _rng = new RandomNumberGenerator();
            _rng.Randomize();
        }

        public void Init(Maze maze)
        {
            _maze = maze;

            var children = GetChildren();
            _enemies = new List<Enemy>();
            for (int i = 0; i < children.Count; i++)
            {
                Enemy enemy = (Enemy)children[i];
                enemy.Init();
                enemy.Connect("MovementFinished", this, nameof(OnMovementFinished));
                _enemies.Add(enemy);
            }

            _enemiesToMove = 0;
        }

        public void RestartEnemies()
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].Init();
            }
        }

        public void MoveEnemies(Maze.TilePos playerTile)
        {
            _enemiesToMove = _enemies.Count;
            for (int i = 0; i < _enemies.Count; i++)
            {
                Enemy enemy = _enemies[i];
                if (enemy.CanMove())
                {
                    MoveEnemy(enemy, playerTile);
                }
                else
                {
                    enemy.FinishTurn();
                    OnMovementFinished();
                }
            }
        }

        public bool IsEnemyAtLocation(Maze.TilePos tile)
        {
            return _enemies.FindIndex(enemy => enemy.CurrentTile.Equals(tile)) != -1;
        }

        private void MoveEnemy(Enemy enemy, Maze.TilePos playerTile)
        { 
            Maze.TilePos targetTile = enemy.GetTarget(_maze, playerTile);
            List<Maze.Direction> steps = _maze.GetSteps(enemy.CurrentTile, targetTile);
            if (steps != null && steps.Count > 0)
            {
                int nSteps = _rng.RandiRange(enemy.MinMovementSteps, enemy.MaxMovementSteps);
                nSteps = Mathf.Min(nSteps, steps.Count);
                enemy.Move(steps.GetRange(0, nSteps));
            }
        }

        public void StunClosestEnemy(Maze.TilePos playerTile, int stunTurns)
        {
            // sort enemies by distance to the player
            _enemies.Sort(delegate(Enemy a, Enemy b)
            {
                int aSteps = _maze.GetSteps(a.CurrentTile, playerTile).Count;
                int bSteps = _maze.GetSteps(b.CurrentTile, playerTile).Count;

                return aSteps.CompareTo(bSteps);
            });

            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_enemies[i].CanMove())
                {
                    _enemies[i].Stun(stunTurns);
                    return;
                }
            }
        }

        private void OnMovementFinished()
        {
            _enemiesToMove--;

            if (_enemiesToMove == 0)
            {
                EmitSignal(nameof(MovementFinished));
            }
        }
    }
}
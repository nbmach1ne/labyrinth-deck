using Godot;

namespace LabyrinthDeck
{
    public class Enemy : MovingEntity
    {
        public enum EnemyType
        {
            RED,
            PINK
        }

        public enum EnemyState
        {
            CHASE,
            SCATTER,
            STUNNED
        }

        [Export]
        private EnemyType _type;
        public EnemyType Type
        {
            get { return _type; }
        }
        
        [Export]
        private int _minMovementSteps = 1;
        public int MinMovementSteps
        {
            get { return _minMovementSteps; }
        }
        [Export]
        private int _maxMovementSteps = 3;
        public int MaxMovementSteps
        {
            get { return _maxMovementSteps; }
        }

        [Export]
        private int _chaseTurns = 4;
        [Export]
        private int _scatterTurns = 3;

        private EnemyState _state;
        private int _stunTurns;
        private int _turnsCount;

        public override void Init()
        {
            base.Init();

            _state = EnemyState.CHASE;
        }

        public bool CanMove()
        {
            return _state != EnemyState.STUNNED;
        }

        public Maze.TilePos GetTarget(Maze maze, Maze.TilePos playerTile)
        {
            if (_state == EnemyState.CHASE)
            {
                return playerTile;
            }
            else if (_state == EnemyState.SCATTER)
            {
                return InitialTile;
            }

            return null;
        }

        public void Stun(int turns)
        {
            _stunTurns = turns;
            _turnsCount = 0;
            _state = EnemyState.STUNNED;
        }

        public void FinishTurn()
        {
            base.FinishMovement();
            
            _turnsCount++;
            CheckState();
        }

        private void CheckState()
        {
            switch(_state)
            {
                case EnemyState.CHASE:
                    if (_turnsCount == _chaseTurns)
                    {
                        _turnsCount = 0;
                        _state = EnemyState.SCATTER;
                    }
                    break;
                case EnemyState.SCATTER:
                    if (_turnsCount == _scatterTurns ||
                        CurrentTile.Equals(InitialTile))
                    {
                        _turnsCount = 0;
                        _state = EnemyState.CHASE;
                    }
                    break;
                case EnemyState.STUNNED:
                    if (_turnsCount == _stunTurns)
                    {
                        _turnsCount = 0;
                        _state = EnemyState.CHASE;
                    }
                    break;
            }
        }

        protected override void FinishMovement()
        {
            FinishTurn();
        }
    }
}
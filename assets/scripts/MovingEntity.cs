using Godot;
using System.Collections.Generic;

namespace LabyrinthDeck
{
    public abstract class MovingEntity : Spatial
    {
        [Export]
        protected Vector2 _initialTile;
        public Maze.TilePos InitialTile
        {
            get { return new Maze.TilePos((int)_initialTile.x, (int)_initialTile.y); }
        }
        [Export]
        private float _timeBetweenSteps = 0.1f;
        [Export]
        private float _stepDuration = 0.5f;
        [Export]
        private float _rotateDuration = 0.25f;

        [Signal]
        delegate void MovementFinished();
        [Signal]
        delegate void StepFinished();


        private int _currentStep;
        private List<Maze.Direction> _pendingSteps;
        private Tween _stepsTween;
        private Spatial _characterModel;

        private Maze.TilePos _currentTile;
        public Maze.TilePos CurrentTile
        {
            get { return _currentTile; }
            set
            {
                _currentTile = value;
                Translation = _currentTile.ToWorldPos();
            }
        }

        public override void _Ready()
        {
            _stepsTween = GetNode<Tween>("tween");
            _characterModel = GetNode<Spatial>("character");
        }

        public virtual void Init()
        {
            CurrentTile = new Maze.TilePos((int)_initialTile.x, (int)_initialTile.y);
            _characterModel.Rotation = GetRotation(Maze.Direction.UP);
        }

        public void SubscribeToGameEvents(Game game)
        {
            game.Connect("GameOver", this, nameof(OnGameOver));
            game.Connect("WinGame", this, nameof(OnWinGame));
        }

        public void Move(List<Maze.Direction> steps)
        {
            _pendingSteps = steps;
            _currentStep = 0;
            Step(_pendingSteps[_currentStep]);
        }
        protected virtual void FinishMovement() {}

        protected void OnGameOver()
        {
            Stop();
        }   

        protected void OnWinGame()
        {
            Stop();
        } 

        private void Stop()
        {
            _pendingSteps = null;
            _stepsTween.RemoveAll();
        }

        private void Step(Maze.Direction direction)
        {
            _characterModel.Rotation = GetRotation(direction);

            // MOVE
            Vector3 startPos = CurrentTile.ToWorldPos();
            Maze.TilePos targetTile = GetTargetTile(direction);
            Vector3 targetPos = targetTile.ToWorldPos();
            _stepsTween.InterpolateProperty(this, "translation", startPos, targetPos, _stepDuration);

            // CALLBACK
            float callbackTime = _stepDuration + _timeBetweenSteps;
            _stepsTween.InterpolateCallback(this, callbackTime, nameof(FinishStep), targetTile);
            _stepsTween.Start();
        }

        private Vector3 GetRotation(Maze.Direction direction)
        {
            switch(direction)
            {
                case Maze.Direction.NONE:
                    return Rotation;
                case Maze.Direction.UP:
                    return new Vector3(0f, Mathf.Pi, 0f);
                case Maze.Direction.DOWN:
                    return Vector3.Zero;
                case Maze.Direction.LEFT:
                    return new Vector3(0f, -Mathf.Pi/2, 0f);
                case Maze.Direction.RIGHT:
                    return new Vector3(0f, Mathf.Pi/2, 0f);
                default:
                    return Vector3.Zero;
            }
        }

        private Maze.TilePos GetTargetTile(Maze.Direction direction)
        {
            Maze.TilePos targetTile = null;
            switch(direction)
            {
                case Maze.Direction.UP:
                    targetTile = new Maze.TilePos(CurrentTile.X, CurrentTile.Y - 1);
                    break;
                case Maze.Direction.DOWN:
                    targetTile = new Maze.TilePos(CurrentTile.X, CurrentTile.Y + 1);
                    break;
                case Maze.Direction.LEFT:
                    targetTile = new Maze.TilePos(CurrentTile.X - 1, CurrentTile.Y);
                    break;
                case Maze.Direction.RIGHT:
                    targetTile = new Maze.TilePos(CurrentTile.X + 1, CurrentTile.Y);
                    break;
            }

            return targetTile;
        }

        private void FinishStep(Maze.TilePos targetTile)
        {
            CurrentTile = targetTile;
            _currentStep++;
            if (_currentStep == _pendingSteps.Count)
            {
                FinishMovement();
                EmitSignal(nameof(MovementFinished));
            }
            else
            {
                EmitSignal(nameof(StepFinished));

                // Checking is movement was interrupted
                if (_pendingSteps != null)
                {
                    Step(_pendingSteps[_currentStep]);
                }
            }
        }
    
    }
}
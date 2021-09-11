using Godot;
using System.Collections.Generic;

namespace LabyrinthDeck
{
    public class Game : Node
    {
        private enum GameState
        {
            PLAYER_TURN,
            ENEMIES_TURN
        }

        public static int TILE_SIZE = 4;

        [Export]
        private int _specialCardProb = 90;

        [Signal]
        delegate void GameOver();
        [Signal]
        delegate void WinGame();

        private Maze _maze;
        private Player _player;
        private GameState _state;
        private HUD _hud;
        private EnemiesControl _enemies;
        private CameraFollow _camera;

        private RandomNumberGenerator _rng;

        public override void _Ready()
        {
            InitMaze();
            InitPlayer();
            InitEnemies();
            InitCamera();
            InitHUD();

            _rng = new RandomNumberGenerator();
            _rng.Randomize();
        }

        private void InitMaze()
        {
            _maze = GetNode<Maze>("maze");
        }

        private void InitPlayer()
        {
            _player = GetNode<Player>("player");
            _player.Init();
            _player.SubscribeToGameEvents(this);
            _player.Connect("MovementFinished", this, nameof(OnPlayerMovementFinished));
            _player.Connect("StepFinished", this, nameof(OnPlayerStepFinished));
        }

        private void InitHUD()
        {
            _hud = GetNode<HUD>("HUD");
            _hud.SubscribeToGameEvents(this);
            _hud.Connect("UIReady", this, nameof(OnUIReady));
            _hud.Connect("CardSelected", this, nameof(OnCardSelected));
            _hud.Connect("ResetGame", this, nameof(OnResetGame));
        }

        private void InitEnemies()
        {
            _enemies = GetNode<EnemiesControl>("enemies_control");
            _enemies.Connect("MovementFinished", this, nameof(OnEnemiesMovementFinished));
            _enemies.Init(_maze);
        }

        private void InitCamera()
        {
            _camera = GetNode<CameraFollow>("camera_follow");
            _camera.Init(_player);
        }

        private void PlayTurn()
        {
            switch(_state)
            {
                case GameState.PLAYER_TURN: 
                    DrawCards();
                    break;
                case GameState.ENEMIES_TURN:
                    _enemies.MoveEnemies(_player.CurrentTile);
                    break;
            }
        }
        
        private void DrawCards(bool specialCards = true)
        {
            List<Maze.Movement> movements = _maze.GetPossibleMovements(_player.CurrentTile);

            bool forceSpecialCard = false;
            if (movements.Count < 3)
            {
                forceSpecialCard = true;
            }
            if (movements.Count < 2)
            {
                GD.PrintErr("[Game:DrawCards] There's less than 2 possible movements. Can't draw cards!!");
                return;
            }

            List<Card> cards = new List<Card>();
            int remainingCards = 3;

            // Special cards
            if (specialCards || forceSpecialCard)
            {
                if (_rng.RandiRange(0, 100) <= _specialCardProb || forceSpecialCard)
                {
                    remainingCards--;
                    cards.Add(GetSpecialCard());
                }
            }

            // Movement cards
            List<Maze.Movement> shuffledMovements = ShuffleMovements(movements);
            for (int i = remainingCards - 1; i >= 0; i--)
            {
                Maze.Movement movement = shuffledMovements[i];
                cards.Add(MovementToCard(movement));

                // Re-shuffle (might delete this later if I don't see better results)
                shuffledMovements.RemoveAt(i);
                shuffledMovements = ShuffleMovements(shuffledMovements);
                
            }

            // Feed the HUD with these cards
            _hud.DrawCards(cards);
        }

        private Card GetSpecialCard()
        {
            if (_rng.RandiRange(0, 100) <= 75)
            {
                return new Card(Card.CardType.DRAW_AGAIN, 0);
            }
            
            return new Card(Card.CardType.STUN_ENEMY, 0);
        }

        private List<Maze.Movement> ShuffleMovements(List<Maze.Movement> movements)
        {
            int count = movements.Count;
            int last = count - 1;
            for (int i = 0; i < last; i++)
            {
                int r = _rng.RandiRange(0, last);
                Maze.Movement tmp = movements[i];  
                movements[i] = movements[r];  
                movements[r] = tmp;  
            }

            return movements;
        }

        private Card MovementToCard(Maze.Movement movement)
        {
            switch(movement.Direction)
            {
                case Maze.Direction.UP:
                    return new Card(Card.CardType.MOVE_UP, movement.Steps);
                case Maze.Direction.DOWN:
                    return new Card(Card.CardType.MOVE_DOWN, movement.Steps);
                case Maze.Direction.LEFT:
                    return new Card(Card.CardType.MOVE_LEFT, movement.Steps);
                case Maze.Direction.RIGHT:
                    return new Card(Card.CardType.MOVE_RIGHT, movement.Steps);
                default:
                    return new Card(Card.CardType.MOVE_UP, 0);
            }
        }

        private Maze.Movement CardToMovement(Card card)
        {
            switch(card.Type)
            {
                case Card.CardType.MOVE_UP:
                    return new Maze.Movement(Maze.Direction.UP, card.Param);
                case Card.CardType.MOVE_DOWN:
                    return new Maze.Movement(Maze.Direction.DOWN, card.Param);
                case Card.CardType.MOVE_LEFT:
                    return new Maze.Movement(Maze.Direction.LEFT, card.Param);
                case Card.CardType.MOVE_RIGHT:
                    return new Maze.Movement(Maze.Direction.RIGHT, card.Param);
                default:
                    return new Maze.Movement(Maze.Direction.NONE, 0);
            }
        }

        private void OnUIReady()
        {
            _hud.InitCardsPositions();

            _state = GameState.PLAYER_TURN;
            PlayTurn();
        }

        private void OnCardSelected(Card card)
        {
            if (card.IsMovementCard)
            {
                _player.Move(CardToMovement(card).GetStepsList());
            }
            else
            {
                if (card.Type == Card.CardType.DRAW_AGAIN)
                {
                    DrawCards(specialCards: false);
                }
                else if (card.Type == Card.CardType.STUN_ENEMY)
                {
                    _enemies.StunClosestEnemy(_player.CurrentTile, stunTurns: 2);
                    OnPlayerMovementFinished();
                }
            }
        }

        private bool CheckIfGameContinues()
        {
            if (_enemies.IsEnemyAtLocation(_player.CurrentTile))
            {
                DoGameOver();
                return false;
            }

            if (_maze.GoalPos.Equals(_player.CurrentTile))
            {
                DoWinGame();
                return false;
            }

            return true;
        }

        async private void DoGameOver()
        {
            GD.Print("GAME OVER");
            EmitSignal(nameof(GameOver));

            await ToSignal(GetTree().CreateTimer(3f), "timeout");

            OnResetGame();
        }

        async private void DoWinGame()
        {
            GD.Print("YOU WIN!!");
            EmitSignal(nameof(WinGame));

            await ToSignal(GetTree().CreateTimer(3f), "timeout");
            
            OnResetGame();
        }

        private void OnResetGame()
        {
            _player.Init();
            _enemies.RestartEnemies();
            _hud.InitCardsPositions();

            _state = GameState.PLAYER_TURN;
            PlayTurn();
        }

        private void OnPlayerMovementFinished()
        {
            if (CheckIfGameContinues())
            {
                _state = GameState.ENEMIES_TURN;
                PlayTurn();
            }
        }

        private void OnPlayerStepFinished()
        {
            CheckIfGameContinues();
        }

        private void OnEnemiesMovementFinished()
        {
            if (CheckIfGameContinues())
            {
                _state = GameState.PLAYER_TURN;
                PlayTurn();
            }
        }
    }
}
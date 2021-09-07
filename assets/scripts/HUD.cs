using Godot;
using System.Collections.Generic;

namespace LabyrinthDeck
{
    public class HUD : Control
    {
        [Export]
        private Texture[] _cardSprites;

        [Signal]
        delegate void CardSelected(Card card);
        [Signal]
        delegate void ResetGame();

        private Godot.Collections.Array<CardUI> _cardButtons;
        private Control _gameOver;

        public override void _Ready()
        {
            _cardButtons =  new Godot.Collections.Array<CardUI>();
            _cardButtons.Add(GetNode<CardUI>("card_1"));
            _cardButtons.Add(GetNode<CardUI>("card_2"));
            _cardButtons.Add(GetNode<CardUI>("card_3"));

            for (int i = 0; i < _cardButtons.Count; i++)
            {
                _cardButtons[i].Connect("button_down", this, nameof(OnCardPressed), new Godot.Collections.Array { i });
            }

            _gameOver = GetNode<Control>("game_over");
        }

        public void Init()
        {
            _gameOver.Visible = false;
        }

        public void SubscribeToGameEvents(Game game)
        {
            game.Connect("GameOver", this, nameof(OnGameOver));
        }

        public void DrawCards(List<Card> cards)
        {
            if (cards.Count < _cardButtons.Count)
            {
                GD.PrintErr("[ERROR]: not enough cards!");
                return;
            }

            for (int i = 0; i < _cardButtons.Count; i++)
            {
                _cardButtons[i].SetCard(cards[i], GetTexture(cards[i]));
            }

            ShowCards(true);
        }

        public Texture GetTexture(Card card)
        {
            GD.Print(card.Type.ToString());
            switch(card.Type)
            {
                case Card.CardType.MOVE_UP:
                case Card.CardType.MOVE_DOWN:
                case Card.CardType.MOVE_LEFT:
                case Card.CardType.MOVE_RIGHT:
                    return _cardSprites[(int)card.Type * 5 + (card.Param - 1)];
                case Card.CardType.DRAW_AGAIN:
                    return _cardSprites[20];
                case Card.CardType.STUN_ENEMY:
                    return _cardSprites[21];
            }

            return null;
        }

        private void ShowCards(bool visible)
        {
            for (int i = 0; i < _cardButtons.Count; i++)
            {
                _cardButtons[i].Visible = visible;
            }
        }

        private void OnCardPressed(int cardIndex)
        {
            ShowCards(false);
            EmitSignal(nameof(CardSelected), _cardButtons[cardIndex].GetCard());
        }

        private void OnResetPressed()
        {
            EmitSignal(nameof(ResetGame));
        }

        private void OnGameOver()
        {
            GD.Print("OnGameOver");
            _gameOver.Visible = true;
        }
    }
}
